using Microsoft.EntityFrameworkCore;
using System;

namespace BhagirathAPI.Models
{
    public class BhagirathDBContext : DbContext
    {
        public BhagirathDBContext(DbContextOptions<BhagirathDBContext> options)
        : base(options)
        {
        }

        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockData> StockData { get; set; }
    }
}
