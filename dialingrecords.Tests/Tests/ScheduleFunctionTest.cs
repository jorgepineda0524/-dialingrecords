using dialingrecords.Functions.Functions;
using dialingrecords.Tests.Helpers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace dialingrecords.Tests.Tests
{
    public class ScheduleFunctionTest
    {
        [Fact]
        public async Task ScheduleFunctionTest_Should_Create_ConsolidatedMarking()
        {
            //Arrange
            MockCloudTableMarking mockMarkings = new MockCloudTableMarking(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            MockConsolidatedMarkingEntity mockConsolidatedMarkings = new MockConsolidatedMarkingEntity(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));

            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act
            await ScheduleFunction.Run(null, mockConsolidatedMarkings, mockMarkings, logger);
            string message = logger.Logs[0];

            //Assert
            Assert.Contains("Recieved consolidated", message);
        }
    }
}
