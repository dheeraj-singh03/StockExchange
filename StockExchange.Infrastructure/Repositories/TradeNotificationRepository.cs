using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;

namespace StockExchange.Infrastructure.Repositories
{
    public class TradeNotificationRepository : ITradeNotificationRepository
    {
        private readonly StockExchangeDbContext stockExchangeDbContext;

        public TradeNotificationRepository(StockExchangeDbContext stockExchangeDbContext)
        {
            this.stockExchangeDbContext = stockExchangeDbContext;
        }
        public async Task AddTradeNotificationAsync(TradeNotification tradeNotification)
        {
            await stockExchangeDbContext.TradeNotifications.AddAsync(tradeNotification).ConfigureAwait(false);
            await stockExchangeDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
