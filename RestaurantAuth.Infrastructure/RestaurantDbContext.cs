using Microsoft.EntityFrameworkCore;
using RestaurantAuth.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAuth.Infrastructure
{
    public class RestaurantDbContext:DbContext
    {
        public RestaurantDbContext(
        DbContextOptions<RestaurantDbContext> options)
        : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");
        }
    }
}
