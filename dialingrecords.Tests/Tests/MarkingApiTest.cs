using dialingrecords.Common.Models;
using dialingrecords.Functions.Entities;
using dialingrecords.Functions.Functions;
using dialingrecords.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace dialingrecords.Tests.Tests
{
    public class MarkingApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateMarking_Should_Return_200()
        {
            // Arrenge cuando se prepara
            MockCloudTableMarking mockMarkings = new MockCloudTableMarking(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            
            Marking markingRequest = TestFactory.GetMarkingRequest();
            DefaultHttpRequest request = TestFactory.CrateHttpRequest(markingRequest);

            // Act cuando se ejecuta
            IActionResult response = await MarkingApi.CreateMarking(request, mockMarkings, logger);

            // Assert si tuvo la respuesta correcta
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateMarking_Should_Return_200()
        {
            // Arrenge cuando se prepara
            MockCloudTableMarking mockMarkings = new MockCloudTableMarking(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            Marking markingRequest = TestFactory.GetMarkingRequest();
            Guid markingId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CrateHttpRequest(markingId, markingRequest);

            // Act cuando se ejecuta
            IActionResult response = await MarkingApi.UpdateMarking(request, mockMarkings, markingId.ToString(), logger);

            // Assert si tuvo la respuesta correcta
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetAllMarkings_Should_Return_List()
        {
            // Arrenge cuando se prepara
            MockCloudTableMarking mockMarkings = new MockCloudTableMarking(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"),null);
            Marking markingRequest = TestFactory.GetMarkingRequest();
            DefaultHttpRequest request = TestFactory.CrateHttpRequest(markingRequest);

            // Act cuando se ejecuta
            var response = await MarkingApi.GetAllMarkings(request, mockMarkings, logger);

            // Assert si tuvo la respuesta correcta
            OkObjectResult result = (OkObjectResult)response;
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMarkingById_Should_Return_MarkingEntity()
        {
            // Arrenge cuando se prepara
            MockCloudTableMarking mockMarkings = new MockCloudTableMarking(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"), null);
            MarkingEntity markingRequest2 = TestFactory.GetMarkingEntity();
            Marking markingRequest = TestFactory.GetMarkingRequest();
            string markingId = Guid.NewGuid().ToString();
            DefaultHttpRequest request = TestFactory.CrateHttpRequest(markingRequest);

            // Act cuando se ejecuta
            var response = MarkingApi.GetMarkingById(request, markingRequest2, markingId, logger);

            // Assert si tuvo la respuesta correcta
            OkObjectResult result = (OkObjectResult)response;
            Assert.NotNull(result);
        }
    }
}
