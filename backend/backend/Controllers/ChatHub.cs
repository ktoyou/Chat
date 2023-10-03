using backend.Entities;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Controllers;

public class ChatHub : Hub
{
    private const string ReceivePrefix = "_Receive";

    private const string NotInTheRoom = "NotInTheRoom";

    private readonly UsersRepository _usersRepository;

    private readonly RoomsRepository _roomsRepository;

    private readonly MessagesRepository _messagesRepository;
    
    public ChatHub(UsersRepository usersRepository, RoomsRepository roomsRepository, MessagesRepository messagesRepository)
    {
        _usersRepository = usersRepository;
        _roomsRepository = roomsRepository;
        _messagesRepository = messagesRepository;
    }
    
    public async Task SendMessageToRoom(string roomId, string userId, string content)
    {
        if(content == string.Empty) return;

        var room = await _roomsRepository.GetByGuid(Guid.Parse(roomId));
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(SendMessageToRoom));
            return;
        }

        var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(SendMessageToRoom));
            return;
        }

        var message = new Message()
        {
            Content = content,
            User = user,
            Id = Guid.NewGuid(),
            UnixTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Room = room
        };
        
        await _messagesRepository.Add(message);
        var json = JsonConvert.SerializeObject(message);
        
        await Clients.Group(roomId).SendAsync($"{nameof(SendMessageToRoom)}{ReceivePrefix}", json);
    }

    public async Task GetMessagesFromRoom(string roomId)
    {
        var messages = await _messagesRepository.GetMessagesByRoomGuid(Guid.Parse(roomId));

        var jsonMessages = JsonConvert.SerializeObject(messages);
        await Clients.Caller.SendAsync($"{nameof(GetMessagesFromRoom)}{ReceivePrefix}", jsonMessages);
    }
    
    public async Task JoinRoom(string userId, string roomId)
    {
        var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(JoinRoom));
            return;
        }
        
        var room = await _roomsRepository.GetByGuid(Guid.Parse(roomId));
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(JoinRoom));
            return;
        }

        if (room.Users.Count >= room.MaxUsers)
        {
            await SendRoomFullStatusAsync(nameof(JoinRoom));
            return;
        }
        
        room.Users.Add(user);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Caller.SendAsync($"{nameof(JoinRoom)}{ReceivePrefix}", ResponseType.WithoutErrors, JsonConvert.SerializeObject(room));

        await SendJoinMessageToRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LeaveRoom(string roomId, string userId)
    {
        var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(LeaveRoom));
            return;
        }
        
        var room = await _roomsRepository.GetByGuid(Guid.Parse(roomId));
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(LeaveRoom));
            return;
        }

        room.Users.Remove(user);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId);
        await Clients.Caller.SendAsync($"{nameof(LeaveRoom)}{ReceivePrefix}", ResponseType.WithoutErrors);
        
        await SendLeaveMessageFromRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LoginUser(string login)
    {
        if(login == null || login == string.Empty) return;
        var user = await _usersRepository.GetByName(login);
        
        if (user != null)
        {
            await Clients.Caller.SendAsync($"{nameof(LoginUser)}{ReceivePrefix}", (int)ResponseType.LoginExists);
            return;
        }

        var newUser = new User()
        {
            Id = Guid.NewGuid(),
            Name = login,
            ConnectionId = Context.ConnectionId
        };

        await _usersRepository.Add(newUser);
        await Clients.Caller.SendAsync($"{nameof(LoginUser)}{ReceivePrefix}", (int)ResponseType.WithoutErrors, JsonConvert.SerializeObject(newUser));
        await Groups.AddToGroupAsync(Context.ConnectionId, NotInTheRoom);
    }

    public async Task ConnectUser(string userId)
    {
        if (userId == null)
        {
            await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserNotFound);
            return;
        }
        
        var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
        if (user != null)
        {
            user.ConnectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, NotInTheRoom);
            await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserExists, JsonConvert.SerializeObject(user));
            return;
        }

        await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserNotFound);
    }

    public async Task CreateRoom(string userId, string name)
    {
        if(name == string.Empty) return;
        
        var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(CreateRoom));
            return;
        }
        
        var findRoom = await _roomsRepository.GetByName(name);
        if (findRoom != null)
        {
            await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", ResponseType.RoomExists);
            return;
        }
        
        var room = new Room()
        {
            Name = name,
            Id = Guid.NewGuid(),
            Messages = new List<Message>(),
            Users = new List<User>(),
            User = user
        };

        await _roomsRepository.Add(room);
        await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", ResponseType.RoomCreated);
        await SendRoomsToAllClientsAsync();
    }

    public async Task GetRooms()
    {
        var rooms = await _roomsRepository.GetAll();
        var json = JsonConvert.SerializeObject(rooms);
        await Clients.Caller.SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", json);
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserGuidFromCookies();
        if (userId != null)
        {
            var user = await _usersRepository.GetByGuid(Guid.Parse(userId));
            if (user != null)
            {
                await _usersRepository.UpdateUserConnectionIdByGuid(Guid.Parse(userId), Context.ConnectionId);
            }
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = await _usersRepository.GetByConnectionId(Context.ConnectionId);
        if (user == null) return;

        await RemoveUserFromAllGroupsAsync(user);
        await RemoveUserFromAllRoomsAsync(user);
    }

    private async Task SendRoomFullStatusAsync(string method)
        => await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.RoomFull);

    private async Task SendRoomsToAllClientsAsync() =>
        await Clients.Group(NotInTheRoom).SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", JsonConvert.SerializeObject(await _roomsRepository.GetAll()));
    
    private async Task SendUserNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.UserNotFound);
    
    private async Task SendRoomNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.RoomNotFound);

    private async Task SendJoinMessageToRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, user, $"Пользователь {user.Name} зашел в комнату");

    private async Task SendLeaveMessageFromRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, user, $"Пользователь {user.Name} вышел из комнаты");
    
    private string? GetUserGuidFromCookies() 
        => Context.GetHttpContext()?.Request.Cookies["id"];
    
    private async Task RemoveUserFromAllGroupsAsync(User user)
    {
        var roomIds = await _roomsRepository.GetRoomsIdsByUser(user);
        foreach (var roomId in roomIds)
        {
            await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId.ToString());
        }
    }

    private async Task RemoveUserFromAllRoomsAsync(User user)
    {
        await _roomsRepository.RemoveUserFromAllRooms(user);
        await SendRoomsToAllClientsAsync();
    }

    private async Task SendSystemMessageAsync(Room room, User user, string msg)
    {
        var message = new Message()
        {
            Content = msg,
            User = await _usersRepository.GetSystemUser(),
            Id = Guid.NewGuid(),
            UnixTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Room = room
        };
        
        var json = JsonConvert.SerializeObject(message);
        
        await _messagesRepository.Add(message);
        await Clients.Group(room.Id.ToString()).SendAsync($"SendMessageToRoom{ReceivePrefix}", json);
    }
}