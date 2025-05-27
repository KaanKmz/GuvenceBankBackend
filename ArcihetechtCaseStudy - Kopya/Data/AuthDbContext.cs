using ArcihetechtCaseStudy.Models;
using Microsoft.EntityFrameworkCore;

namespace ArcihetechtCaseStudy.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

    }


}
