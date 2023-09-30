using Newtonsoft.Json;

namespace backend.Entities;

public class Message
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("from")]
    public User From { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}