using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ColdStartChallengeFunctions
{
    public static class GetOrderFromQueue
    {
        [FunctionName("GetOrderFromQueue")]
        public static void Run([QueueTrigger("customer-orders", Connection = "AZURE_STORAGE_CONNECTIONSTRING")]string myQueueItem,
            [CosmosDB(databaseName: "orderdatabase", collectionName: "orders", ConnectionStringSetting = "CosmosDBConnection")]out dynamic document, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            document = new
            {
                id = "",
                user = "test",
                date = "test",
                icecream = new
                {
                    icecreamId = "",
                    name = "",
                    description = "",
                    imageUrl = ""
                },
                status = "",
                driver = new
                {
                    driverId = "null",
                    name = "null",
                    imageUrl = "null"
                },
                fullAddress = "",
                deliveryPosition = "",
                lastPosition = ""
            };
        }
    }
}
