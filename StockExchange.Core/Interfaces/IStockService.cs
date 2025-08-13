using StockExchange.Core.Model;

namespace StockExchange.Core.Interfaces
{
    public interface IStockService
    {
        Task<IList<StockModel>> GetAllStocksAsync();
        Task<IList<StockModel>> GetAllStocksRangeAsync(IList<string> stockSymbols);
        StockModel GetStock(string stockSymbol);
        Task AddStockAsync(string stockSymbol);
    }
}
