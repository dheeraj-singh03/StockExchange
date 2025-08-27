using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Core.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository stockRepository;

        public StockService(IStockRepository stockRepository)
        {
            this.stockRepository = stockRepository;
        }

        public async Task AddStockAsync(string stockSymbol)
        {
            await stockRepository.AddStockAsync(stockSymbol).ConfigureAwait(false);
        }

        public async Task<IList<StockModel>> GetAllStocksAsync()
        {
            var stockModels = new List<StockModel>();

            var stocks = await stockRepository.GetAllStocksAsync().ConfigureAwait(false);
            if (stocks != null)
            {
                foreach (var stock in stocks)
                {
                    var stockModel = GetStockModelFromStock(stock);
                    stockModels.Add(stockModel);
                }
            }
            return stockModels;
        }

        public async Task<IList<StockModel>> GetAllStocksRangeAsync(IList<string> stockSymbols)
        {
            var stockModels = new List<StockModel>();

            var stocks = await stockRepository.GetAllStocksRangeAsync(stockSymbols).ConfigureAwait(false);
            if (stocks != null)
            {
                foreach (var stock in stocks)
                {
                    var stockModel = GetStockModelFromStock(stock);
                    stockModels.Add(stockModel);
                }
            }
            return stockModels;
        }

        public async Task<StockModel> GetStockAsync(string stockSymbol)
        {
            var stockModel = new StockModel();

            var stock = await stockRepository.GetStockBySymbolAsync(stockSymbol).ConfigureAwait(false);
            if (stock != null)
            {
                stockModel = GetStockModelFromStock(stock);
            }

            return stockModel;
        }

        private StockModel GetStockModelFromStock(Stock stock)
        {
            var trades = stock.TradeNotifications;
            var totalPrice = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.Price * x.NumberOfShares);
            var totalSoldShares = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.NumberOfShares);
            return new StockModel
            {
                StockSymbol = stock.TickerSymbol,
                StockPrice = totalSoldShares == 0 ? 0 : (totalPrice / totalSoldShares)
            };
        }
    }
}
