using StockExchange.Core.Model;

namespace StockExchange.Core.Interfaces
{
    public interface IStockService
    {
        Task<IList<StockModel>> GetAllStocksAsync();
        Task<IList<StockModel>> GetAllStocksRangeAsync(IList<string> stockSymbols);
        Task<StockModel> GetStockAsync(string stockSymbol);
        Task AddStockAsync(string stockSymbol);
    }
}
