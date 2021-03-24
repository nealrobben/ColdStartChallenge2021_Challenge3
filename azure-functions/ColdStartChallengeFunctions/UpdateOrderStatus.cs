using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ColdStartChallengeFunctions
{
    public static class UpdateOrderStatus
    {
        [FunctionName("UpdateOrderStatus")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [CosmosDB(databaseName: "orderdatabase", collectionName: "orders", ConnectionStringSetting = "CosmosDBConnection", 
            SqlQuery = "SELECT * FROM c WHERE c.status = 'Accepted'")] ICollector<CosmosDBItem> documents, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            //foreach(var item in documents)
            //{

            //}
        }
    }

    public class CosmosDBItem
    {
        public string MyProperty { get; set; }
    }

    //https://docs.microsoft.com/en-us/previous-versions/sandbox/functions-recipes/cosmos-db?tabs=csharp#save-multiple-documents-to-a-collection
    //https://stackoverflow.com/questions/63336415/how-to-query-cosmosdb-from-inside-azure-functions-method-instead-of-attribute
    //https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp
}
