using StockExchange.Core.Model;

namespace StockExchange.Core.Interfaces
{
    public interface ITradeNotificationService
    {
        Task<(bool Success, string Message)> ProcessTradeNotificationAsync(TradeNotificationModel tradeNotification);
    }
}
