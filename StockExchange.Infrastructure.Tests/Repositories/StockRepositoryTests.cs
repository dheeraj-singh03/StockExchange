using Microsoft.EntityFrameworkCore;
using StockExchange.Core.Entities;
using StockExchange.Infrastructure.Repositories;

namespace StockExchange.Infrastructure.Tests.Repositories
{
    public class StockRepositoryTests
    {
        [Fact]
        public async Task GetAllStocksAsync_ShouldReturnAllSeededStocks()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new StockRepository(context);

            // Act
            var result = await repository.GetAllStocksAsync();

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(result, s => s.TickerSymbol == "AAPL");
            Assert.Contains(result, s => s.TickerSymbol == "MSFT");
            Assert.Contains(result, s => s.TickerSymbol == "GOOG");
        }

        [Fact]
        public async Task AddStockAsync_ShouldPersistNewStock()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new StockRepository(context);
            var newSymbol = "TSLA";

            // Act
            await repository.AddStockAsync(newSymbol);
            var persisted = context.Stocks.FirstOrDefault(s => s.TickerSymbol == newSymbol);

            // Assert
            Assert.NotNull(persisted);
            Assert.Equal(newSymbol, persisted.TickerSymbol);
        }

        [Fact]
        public async Task GetStockBySymbol_ShouldReturnMatchingStock()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new StockRepository(context);
            var symbol = "AAPL";

            // Act
            var stock = await repository.GetStockBySymbolAsync(symbol);

            // Assert
            Assert.NotNull(stock);
            Assert.Equal(symbol, stock.TickerSymbol);
        }

        [Fact]
        public async Task GetAllStocksRangeAsync_ShouldReturnFilteredStocksWithNotifications()
        {
            // Arrange
            var context = CreateDbContext();
            var apple = context.Stocks.First(s => s.TickerSymbol == "AAPL");

            context.TradeNotifications.Add(new TradeNotification
            {
                StockId = apple.StockId,
                BrokerName = "Broker1",
                Price = 150.25m,
                NumberOfShares = 10,
                TransactonTime = System.DateTime.UtcNow
            });

            context.SaveChanges();

            var repository = new StockRepository(context);
            var symbols = new List<string> { "AAPL", "GOOG" };

            // Act
            var result = await repository.GetAllStocksRangeAsync(symbols);

            // Assert
            Assert.Equal(2, result.Count());
            var appleResult = result.FirstOrDefault(s => s.TickerSymbol == "AAPL");
            Assert.NotNull(appleResult);
            Assert.NotEmpty(appleResult.TradeNotifications);
        }

        private StockExchangeDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<StockExchangeDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new StockExchangeDbContext(options);

            context.Stocks.AddRange(new List<Stock>
            {
                new Stock { StockId = 1, TickerSymbol = "AAPL" },
                new Stock { StockId = 2, TickerSymbol = "MSFT" },
                new Stock { StockId = 3, TickerSymbol = "GOOG" }
            });

            context.SaveChanges();
            return context;
        }
    }
}