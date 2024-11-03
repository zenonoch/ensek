using NUnit.Framework;
using EnsekClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;


namespace EnsekTests;

public class ApiTests:EnsekTestController
{
    [Test][HighPriority]
    public void ResetTest()
    {
        // Arrange

        // The expected state of energies after 'reset'
        var expectedEnergies = new Dictionary<string, Energy>
        {
            {"gas", new Energy {EnergyId = 1, QuantityOfUnits = 3000}},
            {"nuclear", new Energy {EnergyId = 2, QuantityOfUnits = 0}},
            {"electric", new Energy {EnergyId = 3, QuantityOfUnits = 4322}},
            {"oil", new Energy {EnergyId = 4, QuantityOfUnits = 20}},
        };

        var token = GetAuthToken();

        // Act
        Reset(token);

        // Assert
        var actualEnergies = GetEnergy(token);
        foreach (var ee in expectedEnergies)
        {
            var ae = actualEnergies[ee.Key];
            Assert.Multiple(() =>
            {
                Assert.That(ee.Value.EnergyId, Is.EqualTo(ae.EnergyId), 
                    $"The expected EnergyId for '{ee.Key}' was: {ee.Value.EnergyId}, actually got: {ae.EnergyId}");
                Assert.That(ee.Value.QuantityOfUnits, Is.EqualTo(ae.QuantityOfUnits), 
                    $"The expected QuantityOfUnits for '{ee.Key}' was: {ee.Value.QuantityOfUnits}, actually got: {ae.QuantityOfUnits}");
            }); 
        }

    }

    [Test][MediumPriority]
    public void BuyQuantityOfEachFuel()
    {
        // Arrange
        Random rnd = new Random();

        var token = GetAuthToken();
        Reset(token);
        
        var energies = GetEnergy(token);
        var requestedOrders = new List<(string Id, string Fuel, int Quantity)>();
        Logger.Info($"Number of Energies: {energies.Count}");
        
        var energiesAvailableToBuy = energies.Where(e => e.Value.QuantityOfUnits > 0).ToList();

        // Act
        foreach (var energy in energiesAvailableToBuy)
        {
            Logger.Info($"Energy: {energy.Key}");
            Logger.Info($"EnergyId: {energy.Value.EnergyId}");
            Logger.Info($"Energy - PricePerUnit: {energy.Value.PricePerUnit}");
            Logger.Info($"Energy - QuantityOfUnits: {energy.Value.QuantityOfUnits}");
            Logger.Info($"Energy - UnitType: {energy.Value.UnitType}");
            if (energy.Value.QuantityOfUnits >= 1)
            {
                var amount = rnd.Next(energy.Value.QuantityOfUnits + 1);
                var orderId = PutOrder(token, energy.Value.EnergyId, amount);
                if (orderId != string.Empty){
                    requestedOrders.Add((orderId, energy.Key, amount));

                    Logger.Info($"Requested order:");
                    Logger.Info($"Order - Id: {orderId}");
                    Logger.Info($"Order - Fuel: {energy.Key}");
                    Logger.Info($"Order - Quantity: {amount}");
                }
            }
        }

        //Assert
        Assert.That(requestedOrders.Count, Is.EqualTo(energiesAvailableToBuy.Count), 
            $"The expected order number was {energiesAvailableToBuy.Count}, actually got: {requestedOrders.Count}");
        
        var totalOrders = GetOrders(token);

        foreach (var expectedOrder in requestedOrders)
        {
            var actualOrder =totalOrders.Where(o => o.Id == expectedOrder.Id).FirstOrDefault();
            Assert.That(actualOrder, Is.Not.Null,$"The expected orderId: {expectedOrder.Id} was not found");

            Logger.Info($"Actual Order:");
            Logger.Info($"Id: {actualOrder.Id}");
            Logger.Info($"Actual Fuel: {actualOrder.Fuel} - Expected Fuel: {expectedOrder.Fuel}");
            Logger.Info($"Actual Quantity: {actualOrder.Quantity} - Expected Quantity: {expectedOrder.Quantity}");
            Logger.Info($"Time: {actualOrder.Time}");
            Assert.Multiple(() =>
            {
                Assert.That(actualOrder.Fuel, Is.EqualTo(expectedOrder.Fuel),
                    $"The expected fuel {expectedOrder.Fuel} of orderId: {expectedOrder.Id} does not match the acutal {actualOrder.Fuel}");
                Assert.That(actualOrder.Quantity, Is.EqualTo(expectedOrder.Quantity), 
                    $"The expected quantity {expectedOrder.Quantity} of orderId: {expectedOrder.Id} does not match the acutal {actualOrder.Quantity}");
            });
        }
    }
    [Test][LowPriority]
    public void NumberOfOrdersBeforeToday()
    {
        var token = GetAuthToken();
        var orders = GetOrders(token);
        var ordersBeforeToday = orders.Where(o => o.Time < DateTime.Today).ToList();
        Logger.Info($"Number of Orders before Today: {ordersBeforeToday.Count}");
        foreach (var order in ordersBeforeToday)
        {
            Logger.Info($"Id: {order.Id}");
            Logger.Info($"Fuel: {order.Fuel}");
            Logger.Info($"Quantity: {order.Quantity}");
            Logger.Info($"Time: {order.Time}");
        }
    }



    private string GetAuthToken()
    {
        Logger.Info("Getting Auth Token");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["accept"] = "application/json";
        var content = new {username= "test", password= "testing"};  
        var response = ApiClient.ExecutePostRequest(ApiPaths.LoginPath,headerParameters,content);
        
        int httpCode = (int)response.StatusCode;
        Logger.Info($"HTTP Code is: {httpCode}");
        Logger.Info(response.Content?.ToString());
        Assert.That(httpCode, Is.EqualTo(200), $"The expected HTTP Code was 200, actually got {httpCode}");

        JToken jt = JToken.Parse(response.Content?.ToString() ?? "");
        var token = jt["access_token"]?.ToString() ?? "";
        Logger.Info($"token: {token}");
        return token;
    }

    
    private void Reset(string token)
    {
        Logger.Info("Resetting");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["Authorization"] = "Bearer " + token;
        headerParameters["accept"] = "application/json";

        var content = new {};  
        var response = ApiClient.ExecutePostRequest(ApiPaths.ResetPath,headerParameters,content);
        int httpCode = (int)response.StatusCode;
        Logger.Info($"HTTP Code is: {httpCode}");
        Logger.Info(response.Content?.ToString());
        Assert.That(httpCode, Is.EqualTo(200), $"The expected HTTP Code was 200, actually got {httpCode}");
    }


    private Dictionary<string, Energy> GetEnergy(string token)
    {
        Logger.Info("Getting Energies");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["Authorization"] = "Bearer " + token;
        headerParameters["accept"] = "application/json";
        var queryParameters = new Dictionary<string, string>();
        var response = ApiClient.ExecuteGetRequest(ApiPaths.EnergyPath,queryParameters,headerParameters);
        int httpCode = (int)response.StatusCode;
        Logger.Info($"HTTP Code is: {httpCode}");
        Logger.Info(response.Content?.ToString());
        Assert.That(httpCode, Is.EqualTo(200), $"The expected HTTP Code was 200, actually got {httpCode}");
        string jsonContent = response.Content?.ToString() ?? string.Empty;
        Dictionary<string, Energy> energies = JsonConvert.DeserializeObject<Dictionary<string, Energy>>(jsonContent) ?? [];

        return energies;
    }

    public List<Order> GetOrders(string token)
    {
        Logger.Info("Getting Orders");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["Authorization"] = "Bearer " + token;
        headerParameters["accept"] = "application/json";
        var queryParameters = new Dictionary<string, string>();
        var response = ApiClient.ExecuteGetRequest(ApiPaths.OrdersPath,queryParameters,headerParameters);
        int httpCode = (int)response.StatusCode;
        Logger.Info($"HTTP Code is: {httpCode}");
        Logger.Info(response.Content?.ToString());
        Assert.That(httpCode, Is.EqualTo(200), $"The expected HTTP Code was 200, actually got {httpCode}");

        string jsonContent = response.Content?.ToString() ?? string.Empty;
        List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(jsonContent) ?? [];
        return orders;
    }

    public string PutOrder(string token, int energyId, int quantity)
    {
        Logger.Info("Buying Energy");
        
        var headerParameters = new Dictionary<string, string>();
        headerParameters["Authorization"] = "Bearer " + token;
        headerParameters["accept"] = "application/json";

        var content = new {};  
        string urlPath = $"{ApiPaths.BuyPath}/{energyId}/{quantity}";
        var response = ApiClient.ExecutePutRequest(urlPath,headerParameters,content);
        int httpCode = (int)response.StatusCode;
        Logger.Info($"HTTP Code is: {httpCode}");
        Logger.Info(response.Content?.ToString());
        Assert.That(httpCode, Is.EqualTo(200), $"The expected HTTP Code was 200, actually got {httpCode}");

        var responseContent = response.Content?.ToString() ?? string.Empty;
        Logger.Info(responseContent);
        //string GuidPattern = @"^[a-f0-9]{8}-([a-f0-9]{4}-){3}[a-f0-9]{12}$";
        string GuidPattern= @"(?<=id is ).*(?=[\.])";
        Match m = Regex.Match(responseContent, GuidPattern);
        return  m.Success ? m.Value : string.Empty;
    }
}

