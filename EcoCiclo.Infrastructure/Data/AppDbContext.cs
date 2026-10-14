using EcoCiclo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoCiclo.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PontoColeta> PontosColeta { get; set; }
        public DbSet<AgendamentoColeta> AgendamentosColeta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PontoColeta>().HasKey(p => p.Id);
            modelBuilder.Entity<AgendamentoColeta>().HasKey(a => a.Id);
        }
    }
}
