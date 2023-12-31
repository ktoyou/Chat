using backend.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Controllers;

public class ChatHub : Hub
{
    private const string ReceivePrefix = "_Receive";

    private const string NotInTheRoom = "NotInTheRoom";
    
    private static List<User> _users = new List<User>()
    {
        new User()
        {
            ConnectionId = string.Empty,
            Id = 0,
            Name = "System"
        }
    };

    private static List<Room> _rooms = new List<Room>();

    public async Task SendMessageToRoom(int roomId, string from, string content)
    {
        if(content == string.Empty) return;
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(SendMessageToRoom));
            return;
        }
        
        var user = _users.FirstOrDefault(u => u.Name == from);
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(SendMessageToRoom));
            return;
        }

        var message = new Message()
        {
            Content = content,
            From = user,
            Id = Guid.NewGuid()
        };
        
        room.Messages.Add(message);
        var json = JsonConvert.SerializeObject(message);
        
        await Clients.Group(roomId.ToString()).SendAsync($"{nameof(SendMessageToRoom)}{ReceivePrefix}", json);
    }

    public async Task GetMessagesFromRoom(int roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(GetMessagesFromRoom));
            return;
        }
        
        var jsonMessages = JsonConvert.SerializeObject(room.Messages);
        await Clients.Caller.SendAsync($"{nameof(GetMessagesFromRoom)}{ReceivePrefix}", jsonMessages);
    }
    
    public async Task JoinRoom(string login, int roomId)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(JoinRoom));
            return;
        }
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
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
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Caller.SendAsync($"{nameof(JoinRoom)}{ReceivePrefix}", Errors.WithoutErrors, JsonConvert.SerializeObject(room));

        await SendJoinMessageToRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LeaveRoom(int roomId, string login)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user == null)
        {
            await SendUserNotFoundStatusAsync(nameof(LeaveRoom));
            return;
        }
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await SendRoomNotFoundStatusAsync(nameof(LeaveRoom));
            return;
        }

        room.Users.Remove(user);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId.ToString());
        await Clients.Caller.SendAsync($"{nameof(LeaveRoom)}{ReceivePrefix}", Errors.WithoutErrors);
        
        await SendLeaveMessageFromRoomAsync(room, user);
        await SendRoomsToAllClientsAsync();
    }

    public async Task LoginUser(string login)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user != null)
        {
            await Clients.Caller.SendAsync($"{nameof(LoginUser)}{ReceivePrefix}", (int)Errors.LoginExists);
            return;
        }

        _users.Add(new User()
        {
            Id = GetIncrementedUserId(),
            Name = login,
            ConnectionId = Context.ConnectionId
        });
        await Clients.Caller.SendAsync($"{nameof(LoginUser)}{ReceivePrefix}", (int)Errors.WithoutErrors);
        await Groups.AddToGroupAsync(Context.ConnectionId, NotInTheRoom);
    }

    public async Task CreateRoom(string name)
    {
        if(name == string.Empty) return;
        
        var findRoom = _rooms.FirstOrDefault(r => r.Name == name);
        if (findRoom != null)
        {
            await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", Errors.RoomExists);
            return;
        }
        
        var room = new Room()
        {
            Name = name,
            Id = GetIncrementedRoomId(),
            Messages = new List<Message>(),
            Users = new List<User>()
        };

        _rooms.Add(room);
        await Clients.Caller.SendAsync($"{nameof(CreateRoom)}{ReceivePrefix}", Errors.RoomCreated);
        await SendRoomsToAllClientsAsync();
    }

    public async Task GetRooms() => 
        await Clients.Caller.SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", JsonConvert.SerializeObject(_rooms));

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        if (user == null) return;

        await RemoveUserFromAllGroupsAsync(user);
    }

    private async Task SendRoomFullStatusAsync(string method)
        => await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", Errors.RoomFull);

    private async Task SendRoomsToAllClientsAsync() =>
        await Clients.Group(NotInTheRoom).SendAsync($"{nameof(GetRooms)}{ReceivePrefix}", JsonConvert.SerializeObject(_rooms));
    
    private async Task SendUserNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", Errors.UserNotFound);
    
    private async Task SendRoomNotFoundStatusAsync(string method) =>
        await Clients.Caller.SendAsync($"{method}{ReceivePrefix}", Errors.RoomNotFound);

    private async Task SendJoinMessageToRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, user, $"Пользователь {user.Name} зашел в комнату");

    private async Task SendLeaveMessageFromRoomAsync(Room room, User user) =>
        await SendSystemMessageAsync(room, user, $"Пользователь {user.Name} вышел из комнаты");
    
    private async Task RemoveUserFromAllGroupsAsync(User user)
    {
        _users.Remove(user);
        var roomIds = _rooms.Where(r => r.Users.FirstOrDefault(user) != null).Select(r => r.Id);
        foreach (var roomId in roomIds)
        {
            await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId.ToString());
        }
    }

    private int GetIncrementedUserId()
    {
        int id;
        if (_users.Count == 0) id = 0;
        else
        {
            id = _users.Select(u => u.Id).Max();
            id++;
        }

        return id;
    }

    private int GetIncrementedRoomId()
    {
        int id;
        if (_rooms.Count == 0) id = 0;
        else
        {
            id = _rooms.Select(u => u.Id).Max();
            id++;
        }

        return id;
    }

    private async Task SendSystemMessageAsync(Room room, User user, string msg)
    {
        var message = new Message()
        {
            Content = msg,
            From = _users.FirstOrDefault(u => u.Name == "System"),
            Id = Guid.NewGuid()
        };
        room.Messages.Add(message);
        var json = JsonConvert.SerializeObject(message);
        await Clients.Group(room.Id.ToString()).SendAsync($"SendMessageToRoom{ReceivePrefix}", json);
    }
}