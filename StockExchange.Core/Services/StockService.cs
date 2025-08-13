using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Core.Services
{
    public class StockService : IStockService
    {
        public readonly IStockRepository stockRepository;

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
                    var trades = stock.TradeNotifications;
                    var price = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.Price);
                    var soldShares = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.NumberOfShares);
                    var model = new StockModel
                    {
                        StockSymbol = stock.TickerSymbol,
                        StockPrice = soldShares == 0 ? 0 : price / soldShares
                    };
                    stockModels.Add(model);
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
                    var trades = stock.TradeNotifications;
                    var price = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.Price);
                    var soldShares = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.NumberOfShares);
                    var model = new StockModel
                    {
                        StockSymbol = stock.TickerSymbol,
                        StockPrice = soldShares == 0 ? 0 : price / soldShares
                    };
                    stockModels.Add(model);
                }
            }
            return stockModels;
        }

        public StockModel GetStock(string stockSymbol)
        {
            var stockModel = new StockModel();

            var stock = stockRepository.GetStockBySymbol(stockSymbol);
            if (stock != null)
            {
                var trades = stock.TradeNotifications;
                var price = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.Price);
                var soldShares = trades.Where(x => x.StockId == stock.StockId).Sum(x => x.NumberOfShares);
                stockModel.StockSymbol = stock.TickerSymbol;
                stockModel.StockPrice = soldShares == 0 ? 0 : price / soldShares;
            }

            return stockModel;
        }
    }
}
