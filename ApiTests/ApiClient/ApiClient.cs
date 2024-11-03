using System;
using Framework.Controller;
using Framework.Environment;
using Framework.Logger;
using Newtonsoft.Json;
using RestSharp;

namespace EnsekClient;

public class ApiClient 
    {
        public IEnvironment Environment {get;}
        public ILogger Logger {get;}
        public Configuration Configuration {get;}
        public RestClient RestClient {get;}


        public ApiClient(IEnvironment environment, ILogger logger, Configuration configuration){
            Logger = logger;
            Environment = environment;
            Configuration = configuration;
            var options = new RestClientOptions(Environment.BaseUri);
            RestClient = new RestClient(options);
        }

        
        public RestResponse ExecuteGetRequest(string resourcePath, Dictionary<string, string> queryParameters, Dictionary<string, string> headerParameters)
        {
            var request = new RestRequest(resourcePath, Method.Get);
            foreach (var param in queryParameters)
            {
                request.AddQueryParameter(param.Key, param.Value);
            };
            foreach (var header in headerParameters)
            {
                request.AddHeader(header.Key, header.Value);
            };
            Logger.Debug($"Executing request to: {RestClient.Options.BaseUrl}, method:{request.Method} for resource:{request.Resource}");
            var response = RestClient.Execute(request);
            Logger.Debug($"Response is: {response}");
            return response;
        }

        public RestResponse ExecutePostRequest(string resourcePath, Dictionary<string, string> headerParameters, object content)
        {
            var request = new RestRequest(resourcePath, Method.Post);
            foreach (var header in headerParameters)
            {
                request.AddHeader(header.Key, header.Value);
            };
            string jsonContent = JsonConvert.SerializeObject(content);
            request.AddJsonBody(jsonContent);
            Logger.Debug($"Executing request to: {RestClient.Options.BaseUrl}, method:{request.Method} for resource:{request.Resource}");
            var response = RestClient.Execute(request);
            return response;
        }
        
        public RestResponse ExecutePutRequest(string resourcePath, Dictionary<string, string> headerParameters, object content)
        {
            var request = new RestRequest(resourcePath, Method.Put);
            foreach (var header in headerParameters)
            {
                request.AddHeader(header.Key, header.Value);
            };
            string jsonContent = JsonConvert.SerializeObject(content);
            request.AddJsonBody(jsonContent);
            Logger.Debug($"Executing request to: {RestClient.Options.BaseUrl}, method:{request.Method} for resource:{request.Resource}");
            var response = RestClient.Execute(request);
            return response;
        }
    }

