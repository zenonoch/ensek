
using Newtonsoft.Json;

namespace EnsekClient;
public class Energy
{
    [JsonProperty("energy_id")]
    public int EnergyId { get; set; }
    [JsonProperty("price_per_unit")]
    public double PricePerUnit { get; set; }
    [JsonProperty("quantity_of_units")]
    public int QuantityOfUnits  { get; set; }
    [JsonProperty("unit_type")]
    public string UnitType { get; set; } = "";
}