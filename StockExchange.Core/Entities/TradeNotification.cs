namespace StockExchange.Core.Entities
{
    public class TradeNotification
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public decimal Price { get; set; }
        public decimal NumberOfShares { get; set; }
        public string BrokerName { get; set; }
        public DateTime TransactonTime { get; set; }

        public virtual Stock Stock { get; set; }
    }
}
