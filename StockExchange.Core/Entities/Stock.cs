namespace StockExchange.Core.Entities
{
    public class Stock
    {
        public int StockId { get; set; }
        public string TickerSymbol { get; set; }

        public virtual ICollection<TradeNotification> TradeNotifications { get; set; }
    }
}
