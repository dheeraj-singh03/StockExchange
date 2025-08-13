using StockExchange.Core.Entities;

namespace StockExchange.Core.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllStocksAsync();
        Task<IEnumerable<Stock>> GetAllStocksRangeAsync(IList<string> symbols);
        Task AddStockAsync (string tickerSymbol);
        Stock GetStockBySymbol (string symbol);
    }
}
