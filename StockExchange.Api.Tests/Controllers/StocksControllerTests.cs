using Bogus;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockExchange.Api.Controllers;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Api.Tests.Controllers
{
    public class StocksControllerTests
    {
        private readonly Mock<IStockService> stockServiceMock;
        private readonly StocksController stocksController;

        private readonly Faker faker;

        public StocksControllerTests()
        {
            stockServiceMock = new Mock<IStockService>();
            stocksController = new StocksController(stockServiceMock.Object);

            faker = new Faker();
        }

        [Fact]
        public async Task GetAllStockValues_WhenStocksExist_ReturnsOk()
        {
            // Arrange
            var mockStocks = new List<StockModel>
            {
                new StockModel { StockSymbol = faker.Random.String2(5) },
                new StockModel { StockSymbol = faker.Random.String2(5) }
            };

            stockServiceMock
                .Setup(s => s.GetAllStocksAsync())
                .ReturnsAsync(mockStocks as IList<StockModel>);


            // Act
            var result = await stocksController.GetAllStockValues();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStocks = Assert.IsAssignableFrom<IEnumerable<StockModel>>(okResult.Value);
            Assert.Equal(2, returnedStocks.Count());
        }

        [Fact]
        public async Task GetAllStockValues_WhenNoStocksExist_ReturnsNotFound()
        {
            // Arrange
            stockServiceMock
                .Setup(s => s.GetAllStocksAsync())
                .ReturnsAsync(new List<StockModel>() as IList<StockModel>);

            // Act
            var result = await stocksController.GetAllStockValues();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No Stock Found", notFound.Value);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbols_WhenValidSymbols_ReturnsOk()
        {
            // Arrange
            var mockStocks = new List<StockModel>
            {
                new StockModel { StockSymbol = faker.Random.String2(5) }
            };

            stockServiceMock
                .Setup(s => s.GetAllStocksRangeAsync(It.IsAny<IList<string>>()))
                .ReturnsAsync(mockStocks as IList<StockModel>);

            // Act
            var result = await stocksController.GetStockValuesByTickerSymbols("AAPL");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStocks = Assert.IsAssignableFrom<IEnumerable<StockModel>>(okResult.Value);
            Assert.Single(returnedStocks);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbols_WhenQueryIsEmpty_ReturnsBadRequest()
        {
            // Act
            var result = await stocksController.GetStockValuesByTickerSymbols("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ticker symbols are required.", badRequest.Value);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbols_WhenNoStocksFound_ReturnsNotFound()
        {
            // Arrange
            stockServiceMock
                .Setup(s => s.GetAllStocksRangeAsync(It.IsAny<IList<string>>()))
                .ReturnsAsync(new List<StockModel>() as IList<StockModel>);

            // Act
            var result = await stocksController.GetStockValuesByTickerSymbols("XYZ");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Stocks Not Found", notFound.Value);
        }

        [Fact]
        public void GetStockValuesByTickerSymbol_WhenValidSymbol_ReturnsOk()
        {
            // Arrange
            var mockStock = new StockModel { StockSymbol = faker.Random.String2(5) };

            stockServiceMock
                .Setup(s => s.GetStock(It.IsAny<string>()))
                .Returns(mockStock);

            // Act
            var result = stocksController.GetStockValuesByTickerSymbol(mockStock.StockSymbol);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStock = Assert.IsType<StockModel>(okResult.Value);
            Assert.Equal(mockStock.StockSymbol, returnedStock.StockSymbol);
        }

        [Fact]
        public void GetStockValuesByTickerSymbol_WhenQueryIsEmpty_ReturnsBadRequest()
        {
            // Act
            var result = stocksController.GetStockValuesByTickerSymbol("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ticker symbols are required.", badRequest.Value);
        }

        [Fact]
        public void GetStockValuesByTickerSymbol_WhenStockIsNull_ReturnsNotFound()
        {
            // Arrange
            stockServiceMock
                .Setup(s => s.GetStock(It.IsAny<string>()))
                .Returns((StockModel)null);

            // Act
            var result = stocksController.GetStockValuesByTickerSymbol("XYZ");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Stock Not Found", notFound.Value);
        }

        [Fact]
        public async Task AddStock_WhenValidStock_ReturnsOk()
        {
            // Arrange
            var stock = faker.Random.String2(5);

            stockServiceMock
                .Setup(s => s.AddStockAsync(stock))
                .Returns(Task.CompletedTask);

            // Act
            var result = await stocksController.AddStock(stock);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Stock added successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddStock_WhenStockIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await stocksController.AddStock(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Stock is null.", badRequest.Value);
        }
    }
}
