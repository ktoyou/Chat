using backend.Interfaces;
using Newtonsoft.Json;

namespace backend.Entities;

public class Message : IConvertibleToJson
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("from")]
    public User User { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
    
    [JsonProperty("unixTime")] 
    public long UnixTime { get; set; }

    [JsonIgnore]
    public Room Room { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);

    public static Message BuildMessageNowTime(User from, Room room, string content)
    {
        return new Message()
        {
            Content = content,
            User = from,
            Id = Guid.NewGuid(),
            UnixTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Room = room
        };
    }
}