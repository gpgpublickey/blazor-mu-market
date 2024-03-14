using Microsoft.EntityFrameworkCore;
using mumarket.Models;

namespace mumarket
{
    public class MuMarketDbContext : DbContext
    {
        public MuMarketDbContext(DbContextOptions opt) : base(opt)
        {
        }

        public DbSet<Sell> Sells { get; set; }

        public DbSet<Karma> Karmas { get; set; }

        public DbSet<Models.User> Users { get; set; }
    }
}
