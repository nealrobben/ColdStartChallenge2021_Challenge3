using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ColdStartChallengeFunctions
{
    public static class GetOrderFromQueue
    {
        [FunctionName("GetOrderFromQueue")]
        public static void Run([QueueTrigger("customer-orders", Connection = "AZURE_STORAGE_CONNECTIONSTRING")]string myQueueItem,
            [CosmosDB(databaseName: "orderdatabase", collectionName: "orders", ConnectionStringSetting = "CosmosDBConnection")]out dynamic document, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var sourceItem = JsonConvert.DeserializeObject<SourceItem>(myQueueItem);

            document = new
            {
                id = "",
                user = sourceItem?.User,
                date = sourceItem?.Date,
                icecream = new
                {
                    icecreamId = sourceItem?.IcecreamId,
                    name = "",
                    description = "",
                    imageUrl = ""
                },
                status = "Accepted",
                driver = new
                {
                    driverId = sourceItem?.DriverId,
                    name = "",
                    imageUrl = ""
                },
                fullAddress = sourceItem?.FullAddress,
                deliveryPosition = "",
                lastPosition = sourceItem?.LastPosition
            };
        }

        public class SourceItem
        {
            public string Id { get; set; }
            public string User { get; set; }
            public string Date { get; set; }
            public string IcecreamId { get; set; }
            public string Status { get; set; }
            public string DriverId { get; set; }
            public string FullAddress { get; set; }
            public string LastPosition { get; set; }
        }
    }
}
