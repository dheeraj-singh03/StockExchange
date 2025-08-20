using Bogus;
using Moq;
using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Services;

namespace StockExchange.Core.Tests.Services
{
    public class StockServiceTests
    {
        private readonly Mock<IStockRepository> stockRepositoryMock;

        private readonly StockService stockService;
        private readonly Faker faker;

        public StockServiceTests()
        {
            stockRepositoryMock = new Mock<IStockRepository>();
            stockService = new StockService(stockRepositoryMock.Object);
            faker = new Faker();
        }

        [Fact]
        public async Task AddStockAsync_WhenCalled_InvokesRepository()
        {
            // Arrange
            var symbol = faker.Random.String2(5);

            stockRepositoryMock
                .Setup(r => r.AddStockAsync(symbol))
                .Returns(Task.CompletedTask);

            // Act
            await stockService.AddStockAsync(symbol);

            // Assert
            stockRepositoryMock.Verify(r => r.AddStockAsync(symbol), Times.Once);
        }

        [Fact]
        public async Task GetAllStocksAsync_WhenStocksExist_ReturnsCalculatedModels()
        {
            // Arrange
            var stockId = faker.Random.Int(1, 1000);
            var trades = new List<TradeNotification>
            {
                new TradeNotification { StockId = stockId, Price = 100, NumberOfShares = 2 },
                new TradeNotification { StockId = stockId, Price = 200, NumberOfShares = 4 }
            };

            var stocks = new List<Stock>
            {
                new Stock { StockId = stockId, TickerSymbol = "AAPL", TradeNotifications = trades }
            };

            stockRepositoryMock
                .Setup(r => r.GetAllStocksAsync())
                .ReturnsAsync(stocks);

            // Act
            var result = await stockService.GetAllStocksAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("AAPL", result[0].StockSymbol);
            Assert.Equal(50, result[0].StockPrice);
        }

        [Fact]
        public async Task GetAllStocksAsync_WhenNoStocksExist_ReturnsEmptyList()
        {
            // Arrange
            stockRepositoryMock
                .Setup(r => r.GetAllStocksAsync())
                .ReturnsAsync((IList<Stock>)null);

            // Act
            var result = await stockService.GetAllStocksAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllStocksRangeAsync_WhenStocksExist_ReturnsCalculatedModels()
        {
            // Arrange
            var stockId = faker.Random.Int(1, 1000);
            var trades = new List<TradeNotification>
            {
                new TradeNotification { StockId = stockId, Price = 300, NumberOfShares = 3 }
            };

            var stocks = new List<Stock>
            {
                new Stock { StockId = stockId, TickerSymbol = "MSFT", TradeNotifications = trades }
            };

            var symbols = new List<string> { "MSFT" };

            stockRepositoryMock
                .Setup(r => r.GetAllStocksRangeAsync(symbols))
                .ReturnsAsync(stocks);

            // Act
            var result = await stockService.GetAllStocksRangeAsync(symbols);

            // Assert
            Assert.Single(result);
            Assert.Equal("MSFT", result[0].StockSymbol);
            Assert.Equal(100, result[0].StockPrice);
        }

        [Fact]
        public async Task GetAllStocksRangeAsync_WhenNoStocksExist_ReturnsEmptyList()
        {
            // Arrange
            var symbols = new List<string> { "XYZ" };

            stockRepositoryMock
                .Setup(r => r.GetAllStocksRangeAsync(symbols))
                .ReturnsAsync((IList<Stock>)null);

            // Act
            var result = await stockService.GetAllStocksRangeAsync(symbols);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetStock_WhenStockExists_ReturnsCalculatedModel()
        {
            // Arrange
            var stockId = faker.Random.Int(1, 1000);
            var trades = new List<TradeNotification>
            {
                new TradeNotification { StockId = stockId, Price = 500, NumberOfShares = 5 }
            };

            var stock = new Stock
            {
                StockId = stockId,
                TickerSymbol = "GOOG",
                TradeNotifications = trades
            };

            stockRepositoryMock
                .Setup(r => r.GetStockBySymbol("GOOG"))
                .Returns(stock);

            // Act
            var result = stockService.GetStock("GOOG");

            // Assert
            Assert.Equal("GOOG", result.StockSymbol);
            Assert.Equal(100, result.StockPrice);
        }

        [Fact]
        public void GetStock_WhenStockIsNull_ReturnsEmptyModel()
        {
            // Arrange
            stockRepositoryMock
                .Setup(r => r.GetStockBySymbol("XYZ"))
                .Returns((Stock)null);

            // Act
            var result = stockService.GetStock("XYZ");

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.StockSymbol);
            Assert.Equal(0, result.StockPrice);
        }
    }
}