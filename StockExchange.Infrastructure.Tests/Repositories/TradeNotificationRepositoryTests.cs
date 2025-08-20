using Microsoft.EntityFrameworkCore;
using StockExchange.Core.Entities;
using StockExchange.Infrastructure.Repositories;

namespace StockExchange.Infrastructure.Tests.Repositories
{
    public class TradeNotificationRepositoryTests
    {
        [Fact]
        public async Task AddTradeNotificationAsync_ShouldPersistNotification()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new TradeNotificationRepository(context);

            var notification = new TradeNotification
            {
                StockId = 1,
                BrokerName = "BrokerX",
                Price = 250.75m,
                NumberOfShares = 100,
                TransactonTime = DateTime.UtcNow
            };

            // Act
            await repository.AddTradeNotificationAsync(notification);

            var persisted = context.TradeNotifications.FirstOrDefault();

            // Assert
            Assert.NotNull(persisted);
            Assert.Equal(notification.StockId, persisted.StockId);
            Assert.Equal(notification.BrokerName, persisted.BrokerName);
            Assert.Equal(notification.Price, persisted.Price);
            Assert.Equal(notification.NumberOfShares, persisted.NumberOfShares);
            Assert.True((DateTime.UtcNow - persisted.TransactonTime).TotalSeconds < 5);
        }

        private StockExchangeDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<StockExchangeDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new StockExchangeDbContext(options);

            // Seed a stock for foreign key reference
            context.Stocks.Add(new Stock { StockId = 1, TickerSymbol = "AAPL" });
            context.SaveChanges();

            return context;
        }
    }
}