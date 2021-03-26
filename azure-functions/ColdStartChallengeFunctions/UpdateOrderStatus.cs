using System;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Spatial;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using BingMapsRESTToolkit;
using Point = Microsoft.Azure.Cosmos.Spatial.Point;

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
                    var position = GetLocation(order.fullAddress).Position;
                    order.deliveryPosition = $"{position.Latitude}, {position.Longitude}";

                    container.ReplaceItemAsync(order,order.id);
                }
            }
        }

        private static Point GetLocation(string address)
        {
            var request = new GeocodeRequest()
            {
                Query = address,
                IncludeIso2 = true,
                IncludeNeighborhood = true,
                MaxResults = 5,
                BingMapsKey = Environment.GetEnvironmentVariable("BingMapsKey")
            };

            var response = request.Execute().Result;

            if (response != null &&
                response.ResourceSets != null &&
                response.ResourceSets.Length > 0 &&
                response.ResourceSets[0].Resources != null &&
                response.ResourceSets[0].Resources.Length > 0)
            {
                var result = response.ResourceSets[0].Resources[0] as Location;

                return new Point(result.Point.Coordinates[0], result.Point.Coordinates[1]);
            }

            return new Point(0,0);
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
