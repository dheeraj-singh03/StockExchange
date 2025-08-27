using Bogus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Moq;
using Newtonsoft.Json;
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
            var result = await stocksController.GetAllStockValuesAsync();

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
            var result = await stocksController.GetAllStockValuesAsync();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonConvert.SerializeObject(notFound.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("No Stock Found", (string)obj.error);
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
            var result = await stocksController.GetStockValuesByTickerSymbolsAsync("AAPL");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStocks = Assert.IsAssignableFrom<IEnumerable<StockModel>>(okResult.Value);
            Assert.Single(returnedStocks);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbols_WhenQueryIsEmpty_ReturnsBadRequest()
        {
            // Act
            var result = await stocksController.GetStockValuesByTickerSymbolsAsync("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequest.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Ticker symbols are required.", (string)obj.error);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbols_WhenNoStocksFound_ReturnsNotFound()
        {
            // Arrange
            stockServiceMock
                .Setup(s => s.GetAllStocksRangeAsync(It.IsAny<IList<string>>()))
                .ReturnsAsync(new List<StockModel>() as IList<StockModel>);

            // Act
            var result = await stocksController.GetStockValuesByTickerSymbolsAsync("XYZ");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonConvert.SerializeObject(notFound.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Stocks Not Found", (string)obj.error);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbol_WhenValidSymbol_ReturnsOk()
        {
            // Arrange
            var mockStock = new StockModel { StockSymbol = faker.Random.String2(5) };

            stockServiceMock
                .Setup(s => s.GetStockAsync(It.IsAny<string>()))
                .ReturnsAsync(mockStock);

            // Act
            var result = await stocksController.GetStockValuesByTickerSymbolAsync(mockStock.StockSymbol);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStock = Assert.IsType<StockModel>(okResult.Value);
            Assert.Equal(mockStock.StockSymbol, returnedStock.StockSymbol);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbol_WhenQueryIsEmpty_ReturnsBadRequest()
        {
            // Act
            var result = await stocksController.GetStockValuesByTickerSymbolAsync("");

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequest.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Ticker symbol is required.", (string)obj.error);
        }

        [Fact]
        public async Task GetStockValuesByTickerSymbol_WhenStockIsNull_ReturnsNotFound()
        {
            // Arrange
            stockServiceMock
                .Setup(s => s.GetStockAsync(It.IsAny<string>()))
                .ReturnsAsync((StockModel)null);

            // Act
            var result = await stocksController.GetStockValuesByTickerSymbolAsync("XYZ");

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var json = JsonConvert.SerializeObject(notFound.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Stock Not Found", (string)obj.error);
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
            var result = await stocksController.AddStockAsync(new StockTickerSymbolModel { StockTickerSymbol = stock });

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Stock added successfully.", okResult.Value);
        }

        [Fact]
        public async Task AddStock_WhenStockIsNull_ReturnsBadRequest()
        {
            // Act
            var result = await stocksController.AddStockAsync(new StockTickerSymbolModel { StockTickerSymbol = "" });

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var json = JsonConvert.SerializeObject(badRequest.Value);
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);

            Assert.Equal("Stock is null.", (string)obj.error);
        }
    }
}
