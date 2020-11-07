using Microsoft.EntityFrameworkCore;
using Entities.Global;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Contexts
{
    public class GlobalContext : DbContext
    {
        public GlobalContext(DbContextOptions<GlobalContext> dbContextOptions)
                : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Global");

            modelBuilder.Entity<Session>().HasMany(s => s.Players).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);

            modelBuilder.Entity<Session>().HasIndex(u => u.Password)
                .IsUnique();

            modelBuilder.Entity<Session>().HasOne(s => s.SessionMaster).WithMany().HasForeignKey(s => s.SessionMasterId);
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Player> Players { get; set; }
    }
}
