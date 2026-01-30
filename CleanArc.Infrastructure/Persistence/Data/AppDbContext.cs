using CleanArc.Core.Entites;
using CleanArc.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CleanArc.Infrastructure.Persistence.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<VaccinationNeeded> VaccinationNeededs { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<AdoptionAuditLog> AdoptionAuditLogs { get; set; }
        public DbSet<UserConnection> UserConnections { get; set; }
        public DbSet<Message> Messages { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
