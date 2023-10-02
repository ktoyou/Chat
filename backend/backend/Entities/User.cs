using Newtonsoft.Json;

namespace backend.Entities;

public class User
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("connectionId")]
    public string ConnectionId { get; set; }
}