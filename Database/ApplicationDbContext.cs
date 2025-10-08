using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Cine_Critic_AI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public object Users { get; internal set; }
    }
}
