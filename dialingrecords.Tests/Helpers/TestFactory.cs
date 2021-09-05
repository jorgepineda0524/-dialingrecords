using dialingrecords.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;
using dialingrecords.Common.Models;
using System.Collections.Generic;

namespace dialingrecords.Tests.Helpers
{
    public class TestFactory
    {
        public static List<MarkingEntity> GetMarkingsEntity()
        {
            List<MarkingEntity> listMarkings = new List<MarkingEntity>();
            listMarkings.Add(new MarkingEntity
            {
                IdEmployee = 1000,
                Date = DateTime.UtcNow,
                ETag = "*",
                Type = 0,
                PartitionKey = "MARKING",
                RowKey = Guid.NewGuid().ToString(),
                Consolidated = false
            });

            return listMarkings;
        }

        public static MarkingEntity GetMarkingEntity()
        {
            return new MarkingEntity
            {
                IdEmployee = 1000,
                Date = DateTime.UtcNow,
                ETag = "*",
                Type = 0,
                PartitionKey = "MARKING",
                RowKey = Guid.NewGuid().ToString(),
                Consolidated = false
            };
        }

        public static DefaultHttpRequest CrateHttpRequest(Guid markingId, Marking markingRequest)
        {
            string request = JsonConvert.SerializeObject(markingRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromSting(request),
                Path = $"/{markingId }"
            };
        }

        public static DefaultHttpRequest CrateHttpRequest(Guid markingId)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/{markingId }"
            };
        }

        public static DefaultHttpRequest CrateHttpRequest(Marking markingRequest)
        {
            string request = JsonConvert.SerializeObject(markingRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromSting(request),
            };
        }

        public static DefaultHttpRequest CrateHttpRequest()
        {
            return new DefaultHttpRequest(new DefaultHttpContext());
        }

        public static Marking GetMarkingRequest()
        {
            return new Marking
            {
                Date = DateTime.Now,
                Consolidated = false,
                IdEmployee = 1000
            };
        }

        private static Stream GenerateStreamFromSting(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }
    }
}
