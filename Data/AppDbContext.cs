using Microsoft.EntityFrameworkCore;
using IteraCompanyGroups.Models;
using System;


namespace IteraCompanyGroups.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("server=localhost;database=itera;user=root;password=;",
                                    new MySqlServerVersion(new Version(8, 0, 26)));
        }

        public class DbInitializer
        {
            public static void Initialize(AppDbContext context)
            {
                context.Database.EnsureCreated();
            }
        }

    }
}