using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;

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
            var iceCream = GetIceCream(sourceItem?.IcecreamId);

            document = new
            {
                id = "",
                user = sourceItem?.User,
                date = sourceItem?.Date,
                icecream = new
                {
                    icecreamId = sourceItem?.IcecreamId,
                    name = iceCream.Name,
                    description = iceCream.Description,
                    imageUrl = iceCream.ImageUrl
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

        private static IceCream GetIceCream(string icecreamId)
        {
            var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var text = $"SELECT Id, Name, Description, ImageUrl FROM [dbo].[Icecreams] WHERE Id = {icecreamId}";

                using (SqlCommand cmd = new SqlCommand(text, conn))
                {
                    var reader = cmd.ExecuteReader();

                    try
                    {
                        reader.Read();

                        return new IceCream
                        {
                            IcecreamId = icecreamId,
                            Name = (string)reader["Name"],
                            Description = (string)reader["Description"],
                            ImageUrl = (string)reader["ImageUrl"]
                        };
                    }
                    finally
                    {
                        reader.Close();
                    }
                }
            }
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

        public class IceCream
        {
            public string IcecreamId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
        }
    }
}
