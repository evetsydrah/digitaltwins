using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Newtonsoft.Json;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Azure;
using System.Collections.Generic;
using Azure.Core.Pipeline;
using System.Net.Http;

namespace AssetData
{
    public static class AssetDataAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("AssetDataAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string adtInstanceUrl = "https://adtdemorock.api.sea.digitaltwins.azure.net";            
            
            try
            {
                string userAssignedClientId = "a30353ea-2e52-4146-a6f7-61b68c04729c";

                //var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ManagedIdentityClientId = userAssignedClientId });                

                //var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                var credential = new DefaultAzureCredential();

                //var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential);

                var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential, new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });
                

                string query = "SELECT * FROM digitaltwins";
                AsyncPageable<BasicDigitalTwin> queryResult = client.QueryAsync<BasicDigitalTwin>(query);

                List<AssetData> lstAssetData = new List<AssetData>();
                await foreach (BasicDigitalTwin twin in queryResult)
                {
                    var something = JsonConvert.DeserializeObject(System.Text.Json.JsonSerializer.Serialize(twin));
                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(twin));
                    Console.WriteLine("---------------");
                    if (twin.Id.Contains("Asset"))
                    {
                        AssetData assets = new AssetData();
                        assets = JsonConvert.DeserializeObject<AssetData>(System.Text.Json.JsonSerializer.Serialize(twin));
                        assets.assetId = twin.Id;
                        lstAssetData.Add(assets);                        
                    }
                }

                return new OkObjectResult(JsonConvert.SerializeObject(lstAssetData));
            }
            catch(Exception ex)
            {                
                return new BadRequestObjectResult(ex);
            }           
         
        }
    }
}
