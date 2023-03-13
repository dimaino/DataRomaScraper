using Microsoft.EntityFrameworkCore;
using MySQLScrapper.Models;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace MySQLScrapper.Data

{
    public class DataContext : DbContext
    {
        public DbSet<Company> Companys { get; set; }
        public DbSet<CompanyHolding> CompanyHoldings { get; set; }
        public DbSet<CompanyHoldingPage> CompanyHoldingPages { get; set; }

        private static IConfigurationRoot Configuration { get; set; }

        public DataContext() : base()
        {

        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();

            optionsBuilder.UseMySql(
                Configuration["ConnectionStrings:Default"], 
                new MySqlServerVersion(new Version(8, 0, 23))
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.CompanyId);
            });

            modelBuilder.Entity<CompanyHolding>(entity => 
            {
                entity.HasKey(e => e.CompanyHoldingId);
                entity.HasOne(d => d.Company)
                    .WithMany(c => c.CompanyHoldings);
            });

            modelBuilder.Entity<CompanyHoldingPage>(entity => 
            {
                entity.HasKey(e => e.CompanyHoldingPageId);
                entity.HasOne(d => d.Company)
                    .WithMany(c => c.CompanyHoldingPages);
            });
        }
    }
}