using CleanArc.Core.Entites;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;



namespace CleanArc.Infrastructure.Persistence.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public DbSet<Animal> Animals { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<VaccinationNeeded> VaccinationNeededs { get; set; }
        public DbSet<Request> Requests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
