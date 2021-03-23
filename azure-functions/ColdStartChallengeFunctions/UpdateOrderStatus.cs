using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ColdStartChallengeFunctions
{
    public static class UpdateOrderStatus
    {
        [FunctionName("UpdateOrderStatus")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}