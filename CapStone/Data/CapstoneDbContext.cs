using CapStone.Models;
using Microsoft.EntityFrameworkCore;

namespace CapStone.Data
{
    public class CapstoneDbContext : DbContext
    {
        public CapstoneDbContext(DbContextOptions<CapstoneDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<InsuranceProduct> InsuranceProducts { get; set; }

        public DbSet<PolicyPlan> PolicyPlans { get; set; }

        public DbSet<Policy> Policies { get; set; }

        public DbSet<PremiumPayment> PremiumPayments { get; set; }

        public DbSet<Claim> Claims { get; set; }

        public DbSet<ClaimDocument> ClaimDocuments { get; set; }

        public DbSet<ClaimHistory> ClaimHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Customer (One-to-One)

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId);

            // InsuranceProduct -> PolicyPlan

            modelBuilder.Entity<PolicyPlan>()
                .HasOne(pp => pp.InsuranceProduct)
                .WithMany(ip => ip.PolicyPlans)
                .HasForeignKey(pp => pp.ProductId);

            // PolicyPlan -> Policy

            modelBuilder.Entity<Policy>()
                .HasOne(p => p.PolicyPlan)
                .WithMany(pp => pp.Policies)
                .HasForeignKey(p => p.PlanId);

            // Customer -> Policy

            modelBuilder.Entity<Policy>()
                .HasOne(p => p.Customer)
                .WithMany(c => c.Policies)
                .HasForeignKey(p => p.CustomerId);

            // Policy -> PremiumPayment

            modelBuilder.Entity<PremiumPayment>()
                .HasOne(pp => pp.Policy)
                .WithMany(p => p.PremiumPayments)
                .HasForeignKey(pp => pp.PolicyId);

            // Policy -> Claim

            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Policy)
                .WithMany(p => p.Claims)
                .HasForeignKey(c => c.PolicyId);

            // Claim -> ClaimDocument

            modelBuilder.Entity<ClaimDocument>()
                .HasOne(cd => cd.Claim)
                .WithMany(c => c.ClaimDocuments)
                .HasForeignKey(cd => cd.ClaimId);

            // Claim -> ClaimHistory

            modelBuilder.Entity<ClaimHistory>()
                .HasOne(ch => ch.Claim)
                .WithMany(c => c.ClaimHistories)
                .HasForeignKey(ch => ch.ClaimId);

            // User -> ClaimHistory

            modelBuilder.Entity<ClaimHistory>()
                .HasOne(ch => ch.User)
                .WithMany(u => u.ClaimHistories)
                .HasForeignKey(ch => ch.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}