namespace StockExchange.Core.Model
{
    public class TradeNotificationModel
    {
        public string TickerSymbol { get; set; }
        public decimal Price { get; set; }
        public decimal ShareCount { get; set; }
        public string BrokerName { get; set; }
    }
}
