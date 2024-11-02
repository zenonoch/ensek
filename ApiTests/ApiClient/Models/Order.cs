using Newtonsoft.Json;

namespace EnsekClient;
public class Order
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;
    [JsonProperty("fuel")]
    public string Fuel { get; set; } = string.Empty;
    [JsonProperty("quantity")]
    public int Quantity  { get; set; }
    [JsonProperty("time")]
    public DateTime Time { get; set; }
}