using Microsoft.EntityFrameworkCore;
using StockExchange.Core.Entities;
using StockExchange.Core.Interfaces;

namespace StockExchange.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockExchangeDbContext stockExchangeDbContext;

        public StockRepository(StockExchangeDbContext stockExchangeDbContext)
        {
            this.stockExchangeDbContext = stockExchangeDbContext;
        }

        public async Task<IEnumerable<Stock>> GetAllStocksAsync()
        {
            return await stockExchangeDbContext.Stocks.ToListAsync().ConfigureAwait(false);
        }

        public async Task AddStockAsync(string tickerSymbol)
        {
            await stockExchangeDbContext.Stocks.AddAsync(new Stock
            {
                TickerSymbol = tickerSymbol
            }).ConfigureAwait(false);

            await stockExchangeDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Stock GetStockBySymbol(string symbol)
        {
            return stockExchangeDbContext.Stocks.FirstOrDefault(x => x.TickerSymbol == symbol);
        }

        public async Task<IEnumerable<Stock>> GetAllStocksRangeAsync(IList<string> symbols)
        {
            return await stockExchangeDbContext.Stocks
                .Where(s => symbols.Contains(s.TickerSymbol))
                .Include(s => s.TradeNotifications)
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
