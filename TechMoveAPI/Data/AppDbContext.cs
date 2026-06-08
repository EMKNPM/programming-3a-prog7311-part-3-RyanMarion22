using Microsoft.EntityFrameworkCore;
using TechMoveMVC.Models;

namespace TechMoveMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceRequest>()
                .HasDiscriminator<string>("ServiceType")
                .HasValue<ServiceRequest>("Base")
                .HasValue<AirServiceRequest>("Air")
                .HasValue<SeaServiceRequest>("Sea")
                .HasValue<RoadServiceRequest>("Road");
        }
    }
}