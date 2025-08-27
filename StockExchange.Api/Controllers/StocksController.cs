using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockService stockService;

        public StocksController(IStockService stockService)
        {
            this.stockService = stockService;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetAllStockValuesAsync()
        {
            var stocks = await stockService.GetAllStocksAsync();
            if (!stocks.Any())
            {
                return NotFound(new { error = "No Stock Found" });
            }
            return Ok(stocks);
        }

        [HttpGet("get-range")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetStockValuesByTickerSymbolsAsync([FromQuery] string tickerSymbols)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbols))
            {
                return BadRequest(new { error = "Ticker symbols are required." });
            }

            var symbols = tickerSymbols.Split(',').Select(s => s.Trim()).ToList();
            var stocks = await stockService.GetAllStocksRangeAsync(symbols);
            if(!stocks.Any())
            {
                return NotFound(new { error = "Stocks Not Found" });
            }
            return Ok(stocks);
        }

        [HttpGet("get-single")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetStockValuesByTickerSymbolAsync([FromQuery] string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                return BadRequest(new { error = "Ticker symbol is required." });
            }

            var stock = await stockService.GetStockAsync(tickerSymbol);
            if(stock == null  || string.IsNullOrWhiteSpace(stock.StockSymbol))
            {
                return NotFound(new { error = "Stock Not Found" });
            }
            return Ok(stock);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Write")]
        public async Task<IActionResult> AddStockAsync([FromBody] StockTickerSymbolModel stock)
        {
            if (string.IsNullOrWhiteSpace(stock.StockTickerSymbol))
            {
                return BadRequest(new { error = "Stock is null." });
            }

            await stockService.AddStockAsync(stock.StockTickerSymbol);
            return Ok("Stock added successfully.");
        }
    }
}
