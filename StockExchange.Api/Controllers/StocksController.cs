using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockExchange.Core.Interfaces;

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
        public async Task<IActionResult> GetAllStockValues()
        {
            var stocks = await stockService.GetAllStocksAsync();
            if (!stocks.Any())
            {
                return NotFound("No Stock Found");
            }
            return Ok(stocks);
        }

        [HttpGet("get-range")]
        [Authorize(Roles = "Read,Write")]
        public async Task<IActionResult> GetStockValuesByTickerSymbols([FromQuery] string tickerSymbols)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbols))
            {
                return BadRequest("Ticker symbols are required.");
            }

            var symbols = tickerSymbols.Split(',').Select(s => s.Trim()).ToList();
            var stocks = await stockService.GetAllStocksRangeAsync(symbols);
            if(!stocks.Any())
            {
                return NotFound("Stocks Not Found");
            }
            return Ok(stocks);
        }

        [HttpGet("get-single")]
        [Authorize(Roles = "Read,Write")]
        public IActionResult GetStockValuesByTickerSymbol([FromQuery] string tickerSymbol)
        {
            if (string.IsNullOrWhiteSpace(tickerSymbol))
            {
                return BadRequest("Ticker symbols are required.");
            }

            var stock = stockService.GetStock(tickerSymbol);
            if(stock == null  || string.IsNullOrWhiteSpace(stock.StockSymbol))
            {
                return NotFound("Stock Not Found");
            }
            return Ok(stock);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Write")]
        public async Task<IActionResult> AddStock([FromBody] string stock)
        {
            if (stock == null)
            {
                return BadRequest("Stock is null.");
            }

            await stockService.AddStockAsync(stock);
            return Ok("Stock added successfully.");
        }
    }
}
