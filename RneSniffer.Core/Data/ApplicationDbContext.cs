using Microsoft.EntityFrameworkCore;
using RneSniffer.Core;

namespace Portal.Core.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext()
            : base()
        {
        }

        public DbSet<EntrepriseRne> EntrepriseRne { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"rne.db");
        }
    }
}
