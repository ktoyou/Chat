using backend.Interfaces;
using Newtonsoft.Json;

namespace backend.Entities;

public class User : IConvertibleToJson
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [JsonProperty("connectionId")]
    public string ConnectionId { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);
}