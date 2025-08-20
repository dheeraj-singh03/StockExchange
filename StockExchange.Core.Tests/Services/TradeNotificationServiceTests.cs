using Bogus;
using Microsoft.AspNetCore.Identity;
using Moq;
using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;
using StockExchange.Core.Services;

namespace StockExchange.Core.Tests.Services
{
    public class TradeNotificationServiceTests
    {
        private readonly Mock<ITradeNotificationRepository> tradeNotificationRepoMock;
        private readonly Mock<IStockRepository> stockRepositoryMock;
        private readonly Mock<UserManager<IdentityUser>> userManagerMock;

        private readonly TradeNotificationService tradeNotificationService;
        private readonly Faker faker;

        public TradeNotificationServiceTests()
        {
            tradeNotificationRepoMock = new Mock<ITradeNotificationRepository>();
            stockRepositoryMock = new Mock<IStockRepository>();

            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            userManagerMock = new Mock<UserManager<IdentityUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            tradeNotificationService = new TradeNotificationService(
                tradeNotificationRepoMock.Object,
                stockRepositoryMock.Object,
                userManagerMock.Object);

            faker = new Faker();
        }

        [Fact]
        public async Task ProcessTradeNotificationAsync_WhenStockNotFound_ReturnsFailure()
        {
            // Arrange
            var model = new TradeNotificationModel
            {
                TickerSymbol = "XYZ",
                BrokerName = faker.Internet.UserName(),
                Price = faker.Finance.Amount(),
                ShareCount = faker.Random.Int(1, 100)
            };

            stockRepositoryMock
                .Setup(r => r.GetStockBySymbol(model.TickerSymbol))
                .Returns((Stock)null);

            // Act
            var result = await tradeNotificationService.ProcessTradeNotificationAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Stock Not Found", result.Message);
        }

        [Fact]
        public async Task ProcessTradeNotificationAsync_WhenBrokerNotFound_ReturnsFailure()
        {
            // Arrange
            var stock = new Stock { StockId = faker.Random.Int(1, 1000), TickerSymbol = "AAPL" };
            var model = new TradeNotificationModel
            {
                TickerSymbol = stock.TickerSymbol,
                BrokerName = "unknown_broker",
                Price = faker.Finance.Amount(),
                ShareCount = faker.Random.Int(1, 100)
            };

            stockRepositoryMock
                .Setup(r => r.GetStockBySymbol(model.TickerSymbol))
                .Returns(stock);

            userManagerMock
                .Setup(u => u.FindByNameAsync(model.BrokerName))
                .ReturnsAsync((IdentityUser)null);

            // Act
            var result = await tradeNotificationService.ProcessTradeNotificationAsync(model);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Broker Not Found", result.Message);
        }

        [Fact]
        public async Task ProcessTradeNotificationAsync_WhenValid_ReturnsSuccess()
        {
            // Arrange
            var stock = new Stock { StockId = faker.Random.Int(1, 1000), TickerSymbol = "MSFT" };
            var broker = new IdentityUser { UserName = "valid_broker" };

            var model = new TradeNotificationModel
            {
                TickerSymbol = stock.TickerSymbol,
                BrokerName = broker.UserName,
                Price = faker.Finance.Amount(),
                ShareCount = faker.Random.Int(1, 100)
            };

            stockRepositoryMock
                .Setup(r => r.GetStockBySymbol(model.TickerSymbol))
                .Returns(stock);

            userManagerMock
                .Setup(u => u.FindByNameAsync(model.BrokerName))
                .ReturnsAsync(broker);

            tradeNotificationRepoMock
                .Setup(r => r.AddTradeNotificationAsync(It.IsAny<TradeNotification>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await tradeNotificationService.ProcessTradeNotificationAsync(model);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Processed Successfully", result.Message);

            tradeNotificationRepoMock.Verify(r => r.AddTradeNotificationAsync(It.Is<TradeNotification>(
                t => t.StockId == stock.StockId &&
                     t.BrokerName == model.BrokerName &&
                     t.Price == model.Price &&
                     t.NumberOfShares == model.ShareCount
            )), Times.Once);
        }
    }
}