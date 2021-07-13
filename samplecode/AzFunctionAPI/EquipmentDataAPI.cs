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

namespace DigitalTwin
{
    public static class EquipmentDataAPI
    {
        private static readonly HttpClient httpClient = new HttpClient();

        [FunctionName("EquipmentDataAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string adtInstanceUrl = "<<INSERT YOUR ADT API HERE>>";  //TODO : you can change this to retrieve from AppSetting           
            
            try
            {                
                var credential = new DefaultAzureCredential();

                var client = new DigitalTwinsClient(new Uri(adtInstanceUrl), credential, new DigitalTwinsClientOptions { Transport = new HttpClientTransport(httpClient) });

                string query = @"SELECT * FROM digitaltwins WHERE IS_OF_MODEL('dtmi:twinlogic:equipment;1')";
                
                AsyncPageable<BasicDigitalTwin> queryResult = client.QueryAsync<BasicDigitalTwin>(query);

                List<Equipment> lstEquipmentData = new List<Equipment>();
                await foreach (BasicDigitalTwin twin in queryResult)
                {                    
                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(twin));
                    Console.WriteLine("---------------");

                    Response<BasicDigitalTwin> getBasicDtResponse = await client.GetDigitalTwinAsync<BasicDigitalTwin>(twin.Id);
                    BasicDigitalTwin basicDt = getBasicDtResponse.Value;                    

                    if (basicDt != null)
                    {
                        Equipment equip = new Equipment();
                        equip = JsonConvert.DeserializeObject<Equipment>(System.Text.Json.JsonSerializer.Serialize(basicDt));
                        equip.assetId = basicDt.Id;

                        equip.relationships = await GetRelationship(client, twin.Id);

                        lstEquipmentData.Add(equip);                        
                    }
                }

                return new OkObjectResult(JsonConvert.SerializeObject(lstEquipmentData));
            }
            catch(Exception ex)
            {                
                return new BadRequestObjectResult(ex);
            }           
         
        }

        static async Task<List<Relationship>> GetRelationship(DigitalTwinsClient client, string twinId)
        {
            List<Relationship> rels = new List<Relationship>();
            AsyncPageable<BasicRelationship> relationships = client.GetRelationshipsAsync<BasicRelationship>(twinId);
            await foreach (BasicRelationship relationship in relationships)
            {                
                Relationship rel = new Relationship();
                rel.Id = relationship.Id;
                rel.SourceId = relationship.SourceId;
                rel.TargetId = relationship.TargetId;
                rel.Name = relationship.Name;
                rels.Add(rel);
            }

            return rels;
        }
    }
}
