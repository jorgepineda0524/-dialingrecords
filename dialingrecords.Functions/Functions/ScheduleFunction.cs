using System;
using System.Collections.Generic;
using System.Linq;
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
            [Table("consolidatedmarking", Connection = "AzureWebJobsStorage")] CloudTable consolidatedmarkingTable,
            [Table("marking", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");

            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);
            TableQuery<MarkingEntity> query = new TableQuery<MarkingEntity>().Where(filter);
            TableQuerySegment<MarkingEntity> completedMarkings = await markingTable.ExecuteQuerySegmentedAsync(query, null);
            List<MarkingEntity> orderMarkings = completedMarkings.Results.OrderBy(e => e.IdEmployee).ThenBy(d => d.Date).ToList();

            double consolidatedTime;
            for (int i = 0; i < orderMarkings.Count(); i++)
            {
                if (orderMarkings[i + 1].IdEmployee == orderMarkings[i].IdEmployee)
                {
                    consolidatedTime = (orderMarkings[i + 1].Date - orderMarkings[i].Date).TotalMinutes;

                    TableOperation findOperation = TableOperation.Retrieve<MarkingEntity>("MARKING", orderMarkings[i].RowKey);
                    TableResult findResult = await markingTable.ExecuteAsync(findOperation);

                    TableOperation findOperation2 = TableOperation.Retrieve<MarkingEntity>("MARKING", orderMarkings[i + 1].RowKey);
                    TableResult findResult2 = await markingTable.ExecuteAsync(findOperation2);

                    if (findResult.Result != null)
                    {
                        MarkingEntity markingEntity = (MarkingEntity)findResult.Result;
                        markingEntity.Consolidated = true;

                        TableOperation updateOperation = TableOperation.Replace(markingEntity);
                        await markingTable.ExecuteAsync(updateOperation);
                    }

                    if (findResult2.Result != null)
                    {
                        MarkingEntity markingEntity2 = (MarkingEntity)findResult2.Result;
                        markingEntity2.Consolidated = true;

                        TableOperation updateOperation2 = TableOperation.Replace(markingEntity2);
                        await markingTable.ExecuteAsync(updateOperation2);
                    }

                    ConsolidatedMarkingEntity consolidatedMarkingEntity = new ConsolidatedMarkingEntity
                    {
                        IdEmployee = orderMarkings[i].IdEmployee,
                        Date = DateTime.UtcNow,
                        ETag = "*",
                        PartitionKey = "CONSOLIDATEDMARKING",
                        RowKey = Guid.NewGuid().ToString(),
                        MinutesWorked = consolidatedTime
                    };

                    TableOperation addOperation = TableOperation.Insert(consolidatedMarkingEntity);
                    await consolidatedmarkingTable.ExecuteAsync(addOperation);

                    i = i + 2;
                }

            }

            log.LogInformation($"Recieved consolidated employees: {DateTime.Now}");
        }
    }
}
