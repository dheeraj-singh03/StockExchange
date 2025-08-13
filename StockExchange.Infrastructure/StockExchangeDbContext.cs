using Microsoft.EntityFrameworkCore;
using StockExchange.Core.Entities;

namespace StockExchange.Infrastructure
{
    public class StockExchangeDbContext : DbContext
    {
        public StockExchangeDbContext(DbContextOptions<StockExchangeDbContext> options)
            : base(options)
        {
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<TradeNotification> TradeNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stock");

                entity.HasKey(x => x.StockId);

                entity.Property(x => x.TickerSymbol)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.HasIndex(x => x.TickerSymbol)
                    .IsUnique();

            });

            modelBuilder.Entity<TradeNotification>(entity =>
            {
                entity.ToTable("TradeNotification");

                entity.HasKey(x => x.Id);

                entity.Property(x => x.StockId)
                    .IsRequired();

                entity.Property(x => x.Price)
                    .IsRequired();

                entity.Property(x => x.BrokerName)
                    .IsRequired();

                entity.Property(x => x.TransactonTime)
                    .IsRequired();

                entity.HasOne(x => x.Stock)
                    .WithMany(s => s.TradeNotifications)
                    .HasForeignKey(x => x.StockId);
            });
        }
    }
}
