using Microsoft.EntityFrameworkCore;
using KitDistributionAPI.Models;

namespace KitDistributionAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<TerminalUser> TerminalUsers { get; set; }
        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<KitTransaction> KitTransactions { get; set; }
    }
}
