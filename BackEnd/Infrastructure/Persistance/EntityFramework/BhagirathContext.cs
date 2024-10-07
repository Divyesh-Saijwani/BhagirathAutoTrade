using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace MarketDataSync.Models
{
    public partial class BhagirathContext : DbContext
    {
        public BhagirathContext()
            : base("name=SqlConnection")
        {
        }

        public virtual DbSet<BhagirathAlgoCustomer> BhagirathAlgoCustomers { get; set; }
        public virtual DbSet<Broker> Brokers { get; set; }
        public virtual DbSet<BrokerConfiguration> BrokerConfigurations { get; set; }
        public virtual DbSet<MarketPrice> MarketPrices { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<StockCalculation> StockCalculations { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<UserBroker> UserBrokers { get; set; }
        public virtual DbSet<UserBrokerConfiguration> UserBrokerConfigurations { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BhagirathAlgoCustomer>()
                .Property(e => e.Direction)
                .IsUnicode(false);

            modelBuilder.Entity<BhagirathAlgoCustomer>()
                .Property(e => e.StrikePrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<BhagirathAlgoCustomer>()
                .Property(e => e.Status)
                .IsUnicode(false);

            modelBuilder.Entity<Broker>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Broker>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Broker>()
                .Property(e => e.APIDocumentationUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Broker>()
                .Property(e => e.AuthenticationUrl)
                .IsUnicode(false);

            modelBuilder.Entity<Broker>()
                .HasMany(e => e.BrokerConfigurations)
                .WithRequired(e => e.Broker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BrokerConfiguration>()
                .Property(e => e.ConfigKey)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("UserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.Symbol)
                .IsUnicode(false);

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.Exchange)
                .IsUnicode(false);

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.Type)
                .IsUnicode(false);

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.Instrument)
                .IsUnicode(false);

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.OptionType)
                .IsUnicode(false);

            modelBuilder.Entity<StockCalculation>()
                .Property(e => e.Direction)
                .IsUnicode(false);

            modelBuilder.Entity<UserBroker>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<UserBroker>()
                .Property(e => e.BrokerId)
                .IsUnicode(false);

            modelBuilder.Entity<UserBroker>()
                .HasMany(e => e.UserBrokerConfigurations)
                .WithRequired(e => e.UserBroker)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserBrokerConfiguration>()
                .Property(e => e.ConfigKey)
                .IsUnicode(false);

            modelBuilder.Entity<UserBrokerConfiguration>()
                .Property(e => e.ConfigValue)
                .IsUnicode(false);
        }
    }
}
