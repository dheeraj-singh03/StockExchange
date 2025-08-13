using Microsoft.AspNetCore.Identity;
using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Core.Services
{
    public class TradeNotificationService : ITradeNotificationService
    {
        private readonly ITradeNotificationRepository tradeNotificationRepository;
        private readonly IStockRepository stockRepository;
        private readonly UserManager<IdentityUser> userManager;

        public TradeNotificationService(
            ITradeNotificationRepository tradeNotificationRepository,
            IStockRepository stockRepository,
            UserManager<IdentityUser> userManager)
        {
            this.tradeNotificationRepository = tradeNotificationRepository;
            this.stockRepository = stockRepository;
            this.userManager = userManager;
        }

        public async Task<(bool Success, string Message)> ProcessTradeNotificationAsync(TradeNotificationModel tradeNotification)
        {
            var stock = stockRepository.GetStockBySymbol(tradeNotification.TickerSymbol);
            if(stock == null)
            {
                return (false, "Stock Not Found");
            }

            var broker = await userManager.FindByNameAsync(tradeNotification.BrokerName).ConfigureAwait(false);

            if (broker == null)
            {
                return (false, "Broker Not Found");
            }

            await tradeNotificationRepository.AddTradeNotificationAsync(new TradeNotification
            {
                StockId = stock.StockId,
                TransactonTime = DateTime.UtcNow,
                Price = tradeNotification.Price,
                NumberOfShares = tradeNotification.ShareCount,
                BrokerName = tradeNotification.BrokerName

            }).ConfigureAwait(false);

            return (true, "Processed Successfully");
        }
    }
}
