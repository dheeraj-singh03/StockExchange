using StockExchange.Core.Entities;

namespace StockExchange.Core.Interfaces
{
    public interface ITradeNotificationRepository
    {
        Task AddTradeNotificationAsync(TradeNotification tradeNotification);
    }
}
