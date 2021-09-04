using System;
using System.Threading.Tasks;
using dialingrecords.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace dialingrecords.Functions.Functions
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Run(
            [TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, true);
            TableQuery<MarkingEntity> query = new TableQuery<MarkingEntity>().Where(filter);
            TableQuerySegment<MarkingEntity> completedTodos = await markingTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach (MarkingEntity completedTodo in completedTodos)
            {
                await markingTable.ExecuteAsync(TableOperation.Delete(completedTodo));
                deleted++;
            }

            log.LogInformation($"Deleted: {deleted} items at: {DateTime.Now}");
        }
    }
}
