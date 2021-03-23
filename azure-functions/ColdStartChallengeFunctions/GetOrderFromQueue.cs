using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ColdStartChallengeFunctions
{
    public static class GetOrderFromQueue
    {
        [FunctionName("GetOrderFromQueue")]
        public static void Run([QueueTrigger("customer-orders", Connection = "AZURE_STORAGE_CONNECTIONSTRING")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
