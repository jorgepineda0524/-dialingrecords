using dialingrecords.Common.Models;
using dialingrecords.Common.Responses;
using dialingrecords.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dialingrecords.Functions.Functions
{
    public static class MarkingApi
    {
        [FunctionName(nameof(CreateMarking))]
        public static async Task<IActionResult> CreateMarking(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "marking")] HttpRequest req,
            [Table("marking", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
            ILogger log)
        {
            log.LogInformation("Recivied a new marking");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Marking marking = JsonConvert.DeserializeObject<Marking>(requestBody);

            if (string.IsNullOrEmpty(marking?.Date.ToString()))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a Date marking."
                });
            }

            MarkingEntity markingEntity = new MarkingEntity
            {
                IdEmployee = marking.IdEmployee,
                Date = DateTime.UtcNow,
                ETag = "*",
                Type = 1,
                PartitionKey = "MARKING",
                RowKey = Guid.NewGuid().ToString(),
                Consolidated = false
            };

            TableOperation addOperation = TableOperation.Insert(markingEntity);
            await markingTable.ExecuteAsync(addOperation);

            string message = "New marking stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = markingEntity
            });
        }

        [FunctionName(nameof(UpdateMarking))]
        public static async Task<IActionResult> UpdateMarking(
           [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "marking/{id}")] HttpRequest req,
           [Table("marking", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
           string id,
           ILogger log)
        {
            log.LogInformation($"Update for marking: {id}, received.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Marking marking = JsonConvert.DeserializeObject<Marking>(requestBody);

            // Validate the marking id
            TableOperation findOperation = TableOperation.Retrieve<MarkingEntity>("MARKING", id);
            TableResult findResult = await markingTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Marking not found."
                });
            }

            // Update marking
            MarkingEntity markingEntity = (MarkingEntity)findResult.Result;
            markingEntity.Consolidated = marking.Consolidated;
            if (!string.IsNullOrEmpty(marking.Date.ToString()))
            {
                markingEntity.Date = marking.Date;
            }

            TableOperation addOperation = TableOperation.Replace(markingEntity);
            await markingTable.ExecuteAsync(addOperation);

            string message = $"Marking: {id}, update in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = markingEntity
            });
        }

        [FunctionName(nameof(GetAllMarkings))]
        public static async Task<IActionResult> GetAllMarkings(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "marking")] HttpRequest req,
           [Table("marking", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
           ILogger log)
        {
            log.LogInformation("Get all markings received.");

            TableQuery<MarkingEntity> query = new TableQuery<MarkingEntity>();
            TableQuerySegment<MarkingEntity> markings = await markingTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all markings.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = markings
            });
        }

        [FunctionName(nameof(GetMarkingById))]
        public static IActionResult GetMarkingById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "marking/{id}")] HttpRequest req,
           [Table("marking", "MARKING", "{id}", Connection = "AzureWebJobsStorage")] MarkingEntity markingEntity,
           string id,
           ILogger log)
        {
            log.LogInformation($"Get marking by id: {id}, recivied.");

            if (markingEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Marking not found."
                });
            }

            string message = $"Marking: {id}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = markingEntity
            });
        }

        [FunctionName(nameof(DeleteMarking))]
        public static async Task<IActionResult> DeleteMarking(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "marking/{id}")] HttpRequest req,
            [Table("marking", "marking", "{id}", Connection = "AzureWebJobsStorage")] MarkingEntity markingEntity,
            [Table("marking", Connection = "AzureWebJobsStorage")] CloudTable markingTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Delete marking: {id}, recivied.");

            if (markingEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Marking not found."
                });
            }

            await markingTable.ExecuteAsync(TableOperation.Delete(markingEntity));
            string message = $"marking: {markingEntity.RowKey}, deleted.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = markingEntity
            });
        }
    }
}
