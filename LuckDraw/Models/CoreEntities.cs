using Microsoft.EntityFrameworkCore;

namespace LuckDraw.Models
{
    public class CoreEntities : DbContext
    {
        public CoreEntities(DbContextOptions<CoreEntities> options) : base(options) { }
        public DbSet<Sign> Signs { get; set; }
        public DbSet<Draw> Draws { get; set; }
        public DbSet<Luck> Lucks { get; set; }
        public DbSet<LuckDraw> LuckDraws { get; set; }
        public DbSet<Option> Options { get; set; }
    }
}