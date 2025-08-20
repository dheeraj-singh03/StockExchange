using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockExchange.Api.Controllers;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Api.Tests.Controllers
{
    public class TradeNotificationsControllerTests
    {
        private readonly Mock<ITradeNotificationService> tradeNotificationServiceMock;
        private readonly TradeNotificationsController tradeNotificationsController;

        private readonly Faker faker;

        public TradeNotificationsControllerTests()
        {
            tradeNotificationServiceMock = new Mock<ITradeNotificationService>();
            tradeNotificationsController = new TradeNotificationsController(tradeNotificationServiceMock.Object);

            faker = new Faker();
        }

        [Fact]
        public async Task PostTradeNotification_WhenValidTrade_ReturnsOk()
        {
            // Arrange
            var tradeNotification = new TradeNotificationModel
            {
                TickerSymbol = faker.Random.String2(5),
                ShareCount = faker.Random.Int(1, 100),
                Price = faker.Finance.Amount(),
                BrokerName = faker.Internet.UserName()
            };

            var expectedMessage = "Trade processed successfully.";

            tradeNotificationServiceMock
                .Setup(s => s.ProcessTradeNotificationAsync(It.IsAny<TradeNotificationModel>()))
                .ReturnsAsync((true, expectedMessage));

            // Act
            var result = await tradeNotificationsController.PostTradeNotification(tradeNotification);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMessage, okResult.Value);
        }

        [Fact]
        public async Task PostTradeNotification_WhenTradeIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await tradeNotificationsController.PostTradeNotification(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Trade notification is null.", badRequest.Value);
        }

        [Fact]
        public async Task PostTradeNotification_WhenProcessingFails_ReturnsBadRequest()
        {
            // Arrange
            var tradeNotification = new TradeNotificationModel
            {
                TickerSymbol = faker.Random.String2(5),
                ShareCount = faker.Random.Int(1, 100),
                Price = faker.Finance.Amount(),
                BrokerName = faker.Internet.UserName()
            };

            var failureMessage = "Trade validation failed.";

            tradeNotificationServiceMock
                .Setup(s => s.ProcessTradeNotificationAsync(It.IsAny<TradeNotificationModel>()))
                .ReturnsAsync((false, failureMessage));

            // Act
            var result = await tradeNotificationsController.PostTradeNotification(tradeNotification);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(failureMessage, badRequest.Value);
        }
    }
}