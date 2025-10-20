using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;

namespace PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; } = null!;
        public DbSet<Document> Documents { get; set; } = null!;
    }
}