using Entities.Avalon;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Contexts
{
    public class AvalonContext : DbContext
    {
        public AvalonContext(DbContextOptions<AvalonContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Avalon");
            modelBuilder.Entity<Game>().HasMany(x => x.Roles).WithOne(x => x.Game).HasForeignKey(x => x.GameId);
            modelBuilder.Entity<Game>().HasMany(x => x.PlayerStates).WithOne(x => x.Game).HasForeignKey(x => x.GameId);
            modelBuilder.Entity<Game>().HasMany(x => x.PlayerIntel).WithOne(x => x.Game).HasForeignKey(x => x.GameId);
            modelBuilder.Entity<PlayerState>().HasOne(x => x.GameRole).WithMany().HasForeignKey(x => x.GameRoleId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PlayerState>().HasIndex(x => x.PlayerId);
            modelBuilder.Entity<PlayerIntel>().HasIndex(x => x.PlayerId);            
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<GameRole> GameRoles { get; set; }
        public DbSet<PlayerState> PlayerStates { get; set; }
        public DbSet<PlayerIntel> PlayerIntel { get; set; }
    }
}
