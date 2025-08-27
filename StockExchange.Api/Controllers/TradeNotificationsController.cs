using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockExchange.Core.Interfaces;
using StockExchange.Core.Model;

namespace StockExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradeNotificationsController : ControllerBase
    {
        private readonly ITradeNotificationService tradeNotificationService;

        public TradeNotificationsController(ITradeNotificationService tradeNotificationService)
        {
            this.tradeNotificationService = tradeNotificationService;
        }

        [HttpPost]
        [Authorize(Roles = "Write")]
        [Route("trade")]
        public async Task<IActionResult> PostTradeNotificationAsync([FromBody] TradeNotificationModel tradeNotification)
        {
            if (tradeNotification == null)
            {
                return BadRequest(new { error = "Trade notification is null." });
            }

            var (isProcessed, message) = await tradeNotificationService.ProcessTradeNotificationAsync(tradeNotification);
            
            if (!isProcessed)
            {
                return BadRequest(new { error = message });
            }

            return Ok(message);
        }
    }
}
