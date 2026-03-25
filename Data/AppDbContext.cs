using Microsoft.EntityFrameworkCore;
using WpfAppT.Models;

namespace WpfAppT.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Для міграцій без фабрики
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=localhost,1436;Database=AutoServiceDb;User Id=autoservice_user;Password=NewDb_Init_77!;TrustServerCertificate=True;"
                );
            }
        }

        public DbSet<Specialist> Specialists { get; set; }
        public DbSet<Record> Records { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>().HasKey(c => c.LicensePlate);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Records)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Records)
                .HasForeignKey(r => r.LicensePlate)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Record>()
                .HasOne(r => r.Specialist)
                .WithMany(s => s.Records)
                .HasForeignKey(r => r.SpecialistId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}