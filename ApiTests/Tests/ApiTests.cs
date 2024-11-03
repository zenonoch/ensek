using NUnit.Framework;
using EnsekClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace EnsekTests;

public class ApiTests:EnsekTestController
{
    private string GetAuthToken()
    {
        Logger.Info("Getting Auth Token");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["accept"] = "application/json";
        var content = new {username= "test", password= "testing"};  
        var response = ApiClient.ExecutePostRequest(ApiPaths.LoginPath,headerParameters,content);
        
        Logger.Info(response.StatusCode.ToString());
        Logger.Info(response.Content?.ToString());

        JToken jt = JToken.Parse(response.Content?.ToString() ?? "");
        var token = jt["access_token"]?.ToString() ?? "";
        Logger.Info($"token: {token}");
        return token;
    }

    
    public void Reset(string token)
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


    public Dictionary<string, Energy> GetEnergy(string token)
    {
        Logger.Info("Getting Energies");
        var headerParameters = new Dictionary<string, string>();
        headerParameters["Authorization"] = "Bearer " + token;
        headerParameters["accept"] = "application/json";
        var queryParameters = new Dictionary<string, string>();
        var response = ApiClient.ExecuteGetRequest(ApiPaths.EnergyPath,queryParameters,headerParameters);
        Logger.Info(response.StatusCode.ToString());
        Logger.Info(response.Content?.ToString());
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
        Logger.Info(response.StatusCode.ToString());
        Logger.Info(response.Content?.ToString());
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
        Logger.Info(response.StatusCode.ToString());
        var responseContent = response.Content?.ToString() ?? string.Empty;
        Logger.Info(responseContent);
        //string GuidPattern = @"^[a-f0-9]{8}-([a-f0-9]{4}-){3}[a-f0-9]{12}$";
        string GuidPattern= @"(?<=id is ).*(?=[\.])";
        Match m = Regex.Match(responseContent, GuidPattern);
        return  m.Success ? m.Value : string.Empty;
    }

    [Test][HighPriority]
    public void TestScenario()
    {
        
        var token = GetAuthToken();
        Reset(token);
        var energies = GetEnergy(token);
        var orderIds= new List<string>();
        Logger.Info($"Number of Energies: {energies.Count}");
        foreach (var energy in energies)
        {
            Logger.Info($"Energy: {energy.Key}");
            Logger.Info($"EnergyId: {energy.Value.EnergyId}");
            Logger.Info($"PricePerUnit: {energy.Value.PricePerUnit}");
            Logger.Info($"QuantityOfUnits: {energy.Value.QuantityOfUnits}");
            Logger.Info($"UnitType: {energy.Value.UnitType}");
            if (energy.Value.QuantityOfUnits >= 1)
            {
                var orderId = PutOrder(token, energy.Value.EnergyId, 1);
                Logger.Info($"Order id is: {orderId}");
                if (orderId != string.Empty){
                    orderIds.Add(orderId);
                }
                
            }
        }

        foreach (var id in orderIds)
        {
            Logger.Info($"order id: {id}");
        }
        var orders = GetOrders(token);
        Logger.Info($"Number of Orders: {orders.Count}");

        var ordersToday =orders.Where(o => orderIds.Contains(o.Id)).ToList();
        Logger.Info($"Number of Orders in the Test: {ordersToday.Count}");
        foreach (var order in ordersToday)
        {
            Logger.Info($"Id: {order.Id}");
            Logger.Info($"Fuel: {order.Fuel}");
            Logger.Info($"Quantity: {order.Quantity}");
            Logger.Info($"Time: {order.Time}");
        }
        
        // var ordersBeforeToday = orders.Where(o => o.Time < DateTime.Today).ToList();
        // Logger.Info($"Number of Orders before Today: {ordersBeforeToday.Count}");
        // foreach (var order in ordersBeforeToday)
        // {
        //     Logger.Info($"Id: {order.Id}");
        //     Logger.Info($"Fuel: {order.Fuel}");
        //     Logger.Info($"Quantity: {order.Quantity}");
        //     Logger.Info($"Time: {order.Time}");
        // }

        
    }
}

