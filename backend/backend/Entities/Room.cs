using Newtonsoft.Json;

namespace backend.Entities;

public class Room
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
}