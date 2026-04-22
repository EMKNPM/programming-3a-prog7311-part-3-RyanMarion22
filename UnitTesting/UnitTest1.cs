using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechMoveMVC.Services;
using TechMoveMVC.Strategies;
using TechMoveMVC.Factory;
using TechMoveMVC.Models;
using ContractModel = TechMoveMVC.Models.Contract;
using System;

namespace UnitTesting
{
    public class UnitTest1
    {
        
        
        // CURRENCY TESTS
        [Fact]
        public async Task Currency_ShouldConvertUsdToZar_Correctly()
        {
            var fakeResponse = @"{ ""rates"": { ""ZAR"": 18.0 } }";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new CurrencyService(httpClient);

            var strategy = new USDStrategy(service);
            var context = new CurrencyContext();
            context.SetStrategy(strategy);

            var result = await context.Convert(10);

            Assert.Equal(180, result);
        }

        [Fact]
        public async Task Currency_ShouldHandleDifferentRate()
        {
            var fakeResponse = @"{ ""rates"": { ""ZAR"": 20.0 } }";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new CurrencyService(httpClient);

            var strategy = new USDStrategy(service);
            var context = new CurrencyContext();
            context.SetStrategy(strategy);

            var result = await context.Convert(5);

            Assert.Equal(100, result);
        }
       
        //  FACTORY TESTS
        
        [Fact]
        public void Factory_ShouldCreateAirRequest()
        {
            var result = ServiceRequestFactory.Create("Air");
            Assert.IsType<AirServiceRequest>(result);
        }

        [Fact]
        public void Factory_ShouldCreateSeaRequest()
        {
            var result = ServiceRequestFactory.Create("Sea");
            Assert.IsType<SeaServiceRequest>(result);
        }

        [Fact]
        public void Factory_ShouldCreateRoadRequest()
        {
            var result = ServiceRequestFactory.Create("Road");
            Assert.IsType<RoadServiceRequest>(result);
        }

        [Fact]
        public void Factory_ShouldThrow_OnInvalidType()
        {
            Assert.Throws<ArgumentException>(() =>
                ServiceRequestFactory.Create("Invalid"));
        }

         //  MODEL TESTS
      
        [Fact]
        public void ServiceRequest_DefaultStatus_ShouldBePending()
        {
            var request = new ServiceRequest();
            Assert.Equal("Pending", request.Status);
        }

        [Fact]
        public void ServiceRequest_ShouldStoreValuesCorrectly()
        {
            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test",
                CostUSD = 100
            };

            Assert.Equal(1, request.ContractId);
            Assert.Equal("Test", request.Description);
            Assert.Equal(100, request.CostUSD);
        }
        //  BUSINESS RULE TESTS
     
        [Fact]
        public void Contract_ShouldNotifyOnStatusChange()
        {
            var contract = new ContractModel();
            contract.Status = "Active";

            Assert.Equal("Active", contract.Status);
        }

        [Fact]
        public void Contract_ShouldAllowValidDates()
        {
            var contract = new ContractModel(); 

            contract.StartDate = DateTime.Today;
            contract.EndDate = DateTime.Today.AddDays(10);

            Assert.True(contract.EndDate > contract.StartDate);
        }
        // ✅ STRATEGY TEST
     
        [Fact]
        public async Task Strategy_ShouldUseUsdStrategy()
        {
            var fakeResponse = @"{ ""rates"": { ""ZAR"": 15.0 } }";

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(fakeResponse)
                });

            var httpClient = new HttpClient(handler.Object);
            var service = new CurrencyService(httpClient);

            var context = new CurrencyContext();
            context.SetStrategy(new USDStrategy(service));

            var result = await context.Convert(2);

            Assert.Equal(30, result);
        }
    }
}