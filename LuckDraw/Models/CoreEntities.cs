using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Models
{
    public class CoreEntities : DbContext
    {
        public CoreEntities(DbContextOptions<CoreEntities> options) : base(options) { }
        public DbSet<Sign> Signs { get; set; }
    }
}
