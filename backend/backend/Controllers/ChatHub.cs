using backend.Entities;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace backend.Controllers;

public class ChatHub : Hub
{
    private const string ReceivePrefix = "_Receive";
    
    private static List<User> _users = new List<User>();

    private static List<Room> _rooms = new List<Room>()
    {
        new Room()
        {
            Id = 0,
            Name = "Room #1",
            Users = new List<User>(),
            Messages = new List<Message>()
        },
        new Room()
        {
            Id = 1,
            Name = "Room #2",
            Users = new List<User>(),
            Messages = new List<Message>()
        },
        new Room()
        {
            Id = 2,
            Name = "Room #3",
            Users = new List<User>(),
            Messages = new List<Message>()
        },
        new Room()
        {
            Id = 3,
            Name = "Room #4",
            Users = new List<User>(),
            Messages = new List<Message>()
        },
        new Room()
        {
            Id = 4,
            Name = "Room #5",
            Users = new List<User>(),
            Messages = new List<Message>()
        }
    };

    public async Task SendMessageToRoom(int roomId, string from, string content)
    {
        if(content == string.Empty) return;
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync($"SendMessageToRoom{ReceivePrefix}", Errors.RoomNotFound);
            return;
        }
        
        var user = _users.FirstOrDefault(u => u.Name == from);
        if (user == null)
        {
            await Clients.Caller.SendAsync($"SendMessageToRoom{ReceivePrefix}", Errors.UserNotFound);
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
        
        await Clients.Group(roomId.ToString()).SendAsync($"SendMessageToRoom{ReceivePrefix}", json);
    }

    public async Task GetMessagesFromRoom(int roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync($"GetMessagesFromRoom{ReceivePrefix}", Errors.RoomNotFound);
            return;
        }
        
        var jsonMessages = JsonConvert.SerializeObject(room.Messages);
        await Clients.Caller.SendAsync($"GetMessagesFromRoom{ReceivePrefix}", jsonMessages);
    }
    
    public async Task JoinRoom(string login, int roomId)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user == null)
        {
            await Clients.Caller.SendAsync($"JoinRoom{ReceivePrefix}", Errors.UserNotFound);
            return;
        }
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync($"JoinRoom{ReceivePrefix}", Errors.RoomNotFound);
            return;
        }

        room.Users.Add(user);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        await Clients.Caller.SendAsync($"JoinRoom{ReceivePrefix}", Errors.WithoutErrors, JsonConvert.SerializeObject(room));
    }

    public async Task LeaveRoom(int roomId, string login)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user == null)
        {
            await Clients.Caller.SendAsync($"LeaveRoom{ReceivePrefix}", Errors.UserNotFound);
            return;
        }
        
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null)
        {
            await Clients.Caller.SendAsync($"LeaveRoom{ReceivePrefix}", Errors.RoomNotFound);
            return;
        }

        room.Users.Remove(user);
        await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId.ToString());
        await Clients.Caller.SendAsync($"LeaveRoom{ReceivePrefix}", Errors.WithoutErrors);
    }
    
    public async Task GetRooms()
    {
        var json = JsonConvert.SerializeObject(_rooms);
        await Clients.Caller.SendAsync($"GetRooms{ReceivePrefix}", json);
    }

    public async Task LoginUser(string login)
    {
        var user = _users.FirstOrDefault(u => u.Name == login);
        if (user != null)
        {
            await Clients.Caller.SendAsync($"LoginUser{ReceivePrefix}", (int)Errors.LoginExists);
            return;
        }

        int id;
        if (_users.Count == 0) id = 0;
        else
        {
            id = _users.Select(u => u.Id).Max();
            id++;
        }
        
        _users.Add(new User()
        {
            Id = id,
            Name = login,
            ConnectionId = Context.ConnectionId
        });
        await Clients.Caller.SendAsync($"LoginUser{ReceivePrefix}", (int)Errors.WithoutErrors);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var user = _users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
        if (user == null) return;

        await RemoveUserFromAllGroupsAsync(user);
    }

    private async Task RemoveUserFromAllGroupsAsync(User user)
    {
        _users.Remove(user);
        var roomIds = _rooms.Where(r => r.Users.FirstOrDefault(user) != null).Select(r => r.Id);
        foreach (var roomId in roomIds)
        {
            await Groups.RemoveFromGroupAsync(user.ConnectionId, roomId.ToString());
        }
    }
}