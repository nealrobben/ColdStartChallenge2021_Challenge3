using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ColdStartChallengeFunctions
{
    public static class UpdateOrderStatus
    {
        private static readonly string databaseName = "orderdatabase";
        private static readonly string collectionName = "orders";

        private static readonly string endpointUrl = Environment.GetEnvironmentVariable("CosmosDBUri");
        private static readonly string authorizationKey = Environment.GetEnvironmentVariable("CosmosDBKey");

        [FunctionName("UpdateOrderStatus")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            using (CosmosClient cosmosClient = new CosmosClient(endpointUrl, authorizationKey))
            {
                Container container = cosmosClient.GetContainer(databaseName, collectionName);

                QueryDefinition query = new QueryDefinition("select * from orders s where s.status = 'Accepted' ");
                var acceptedOrders = new List<Order>();

                using (FeedIterator<Order> resultSet = container.GetItemQueryIterator<Order>(
                    query,
                    requestOptions: new QueryRequestOptions()
                    {
                        MaxItemCount = 1
                    }))
                {
                    while (resultSet.HasMoreResults)
                    {
                        acceptedOrders.AddRange(resultSet.ReadNextAsync().Result);
                    }
                }

                foreach(var order in acceptedOrders)
                {
                    order.status = "Ready";
                    container.ReplaceItemAsync(order,order.id);
                }
            }
        }
    }

    public class Order
    {
        public string id { get; set; }
        public string user { get; set; }
        public string date { get; set; }
        public IceCream icecream { get; set; }
        public string status { get; set; }
        public Driver driver { get; set; }
        public string fullAddress { get; set; }
        public string deliveryPosition { get; set; }
        public string lastPosition { get; set; }
    }

    public class IceCream
    {
        public string icecreamId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string imageUrl { get; set; }

    }

    public class Driver
    {
        public string driverId { get; set; }
        public string name { get; set; }
        public string imageUrl { get; set; }
    }
}
