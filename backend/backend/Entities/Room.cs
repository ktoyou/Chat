using backend.Interfaces;
using Newtonsoft.Json;

namespace backend.Entities;

public class Room : IConvertibleToJson
{
    [JsonProperty("maxUsers")]
    public int MaxUsers { get => 10; }
    
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("withPassword")]
    public bool WithPassword { get; set; }

    [JsonIgnore] 
    public string Password { get; set; }
    
    [JsonProperty("creator")]
    public User User { get; set; }

    [JsonProperty("users")]
    public List<User> Users { get; set; }
    
    [JsonProperty("messages")] 
    public List<Message> Messages { get; set; }

    public static Room BuildRoom(User user, string name, bool withPassword, string password)
    {
        return new Room()
        {
            Name = name,
            Id = Guid.NewGuid(),
            Messages = new List<Message>(),
            Users = new List<User>(),
            User = user,
            WithPassword = withPassword,
            Password = password == null || password == string.Empty ? "" : password,
        };
    }

    public string ToJson() => JsonConvert.SerializeObject(this);
}