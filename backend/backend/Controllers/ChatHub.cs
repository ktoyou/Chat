using backend.Entities;
using backend.Exceptions;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Controllers;

public class ChatHub : Hub
{
    private const string ReceivePrefix = "_Receive";

    private const string DefaultGroup = "DefaultGroup";

    private readonly UsersRepository _usersRepository;

    private readonly RoomsRepository _roomsRepository;

    private readonly MessagesRepository _messagesRepository;
    
    public ChatHub(UsersRepository usersRepository, RoomsRepository roomsRepository, MessagesRepository messagesRepository)
    {
        _usersRepository = usersRepository;
        _roomsRepository = roomsRepository;
        _messagesRepository = messagesRepository;
    }

    #region HubHandlers

    public async Task SendMessageToRoom(string roomId, string userId, string content)
    {
        if(content == string.Empty) return;
        
        var user = await GetUserIfExists(Guid.Parse(userId), nameof(SendMessageToRoom));
        var room = await GetRoomIfExists(Guid.Parse(roomId), nameof(SendMessageToRoom));
        if(user == null || room == null) return;

        var message = Message.BuildMessageNowTime(user, room, content);
        
        await _messagesRepository.AddAsync(message);
        await Clients.Group(roomId).SendAsync($"{nameof(SendMessageToRoom)}{ReceivePrefix}", message.ToJson());
    }

    public async Task GetMessagesFromRoom(string roomId)
    {
        var messages = await _messagesRepository.GetMessagesByRoomGuid(Guid.Parse(roomId));
        var jsonMessages = JsonConvert.SerializeObject(messages);
        
        await Clients.Caller.SendAsync($"{nameof(GetMessagesFromRoom)}{ReceivePrefix}", jsonMessages);
    }
    
    public async Task JoinRoom(string userId, string roomId, string password)
    {
        var user = await GetUserIfExists(Guid.Parse(userId), nameof(JoinRoom));
        var room = await GetRoomIfExists(Guid.Parse(roomId), nameof(JoinRoom));
        if(user == null || room == null) return;

        if (room.WithPassword)
        {
            if (password != room.Password)
            {
                await SendIncorrectPassword(nameof(JoinRoom));
                return;
            }
        }

        if (room.Users.Count >= room.MaxUsers)
        {
            await SendRoomFullStatusAsync(nameof(JoinRoom));
            return;
        }
        
        room.Users.Add(user);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Caller.SendAsync($"{nameof(JoinRoom)}{ReceivePrefix}", ResponseType.WithoutErrors, room.ToJson());

        await SendJoinMessageToRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LeaveRoom(string roomId, string userId)
    {
        var user = await GetUserIfExists(Guid.Parse(userId), nameof(LeaveRoom));
        var room = await GetRoomIfExists(Guid.Parse(roomId), nameof(LeaveRoom));
        if(user == null || room == null) return;

        room.Users.Remove(user);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId);
        await Clients.Caller.SendAsync($"{nameof(LeaveRoom)}{ReceivePrefix}", ResponseType.WithoutErrors);
        
        await SendLeaveMessageFromRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LoginUser(string? login)
    {
        if(string.IsNullOrEmpty(login)) return;
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
            ConnectionId = Context.ConnectionId,
            Password = string.Empty
        };
        
        await _usersRepository.AddAsync(newUser);
        await Clients.Caller.SendAsync($"{nameof(LoginUser)}{ReceivePrefix}", (int)ResponseType.WithoutErrors, newUser.ToJson());
        await Groups.AddToGroupAsync(Context.ConnectionId, DefaultGroup);
    }

    public async Task ConnectUser(string? userId)
    {
        if (userId == null)
        {
            await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserNotFound);
            return;
        }
        
        var user = await _usersRepository.GetByGuidAsync(Guid.Parse(userId));
        if (user != null)
        {
            user.ConnectionId = Context.ConnectionId;
            await Groups.AddToGroupAsync(Context.ConnectionId, DefaultGroup);
            await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserExists, user.ToJson());
            return;
        }

        await Clients.Caller.SendAsync($"{nameof(ConnectUser)}{ReceivePrefix}", ResponseType.UserNotFound);
    }

    public async Task CreateRoom(string userId, string name, bool withPassword, string password)
    {
        if(name == string.Empty) return;
        
        var user = await GetUserIfExists(Guid.Parse(userId), nameof(CreateRoom));
        if(user == null) return;
        
        var findRoom = await _roomsRepository.GetByName(name);
        if (findRoom != null)
        {
            await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", ResponseType.RoomExists);
            return;
        }

        var room = Room.BuildRoom(user, name, withPassword, password);

        await _roomsRepository.AddAsync(room);
        await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", ResponseType.RoomCreated);
        await SendRoomsToAllClientsAsync();
    }

    public async Task GetRooms()
    {
        var rooms = await _roomsRepository.GetAllAsync();
        var json = JsonConvert.SerializeObject(rooms);
        await Clients.Caller.SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", json);
    }

    #endregion

    #region Overrided Methods

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserGuidFromCookies();
        if (userId != null)
        {
            var user = await _usersRepository.GetByGuidAsync(Guid.Parse(userId));
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

    #endregion

    #region Helper Methods

    private async Task<User?> GetUserIfExists(Guid guid, string methodToSendError)
    {
        var user = await _usersRepository.GetByGuidAsync(guid);
        if (user != null) return user;
        await SendUserNotFoundStatusAsync(methodToSendError);
        return null;
    }

    private async Task<Room?> GetRoomIfExists(Guid guid, string methodToSendError)
    {
        var room = await _roomsRepository.GetByGuidAsync(guid);
        if (room != null) return room;
        await SendRoomNotFoundStatusAsync(methodToSendError);
        return null;
    }

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

    private async Task SendSystemMessageAsync(Room room, string msg)
    {
        var systemUser = await _usersRepository.GetSystemUser();
        if (systemUser == null) throw new SystemUserNotFoundException();
        var message = Message.BuildMessageNowTime(systemUser, room, msg);

        await _messagesRepository.AddAsync(message);
        await Clients.Group(room.Id.ToString()).SendAsync($"SendMessageToRoom{ReceivePrefix}", message.ToJson());
    }

    private async Task SendIncorrectPassword(string method)
        => await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.InvalidPassword);

    private async Task SendRoomFullStatusAsync(string method)
        => await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.RoomFull);

    private async Task SendRoomsToAllClientsAsync() =>
        await Clients.Group(DefaultGroup).SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", JsonConvert.SerializeObject(await _roomsRepository.GetAllAsync()));
    
    private async Task SendUserNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.UserNotFound);
    
    private async Task SendRoomNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", ResponseType.RoomNotFound);

    private async Task SendJoinMessageToRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, $"Пользователь {user.Name} зашел в комнату");

    private async Task SendLeaveMessageFromRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, $"Пользователь {user.Name} вышел из комнаты");
    
    private string? GetUserGuidFromCookies() 
        => Context.GetHttpContext()?.Request.Cookies["id"];
    
    #endregion
}