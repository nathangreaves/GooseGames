using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository
{
    public class JustOneContext : DbContext
    {
        public JustOneContext(DbContextOptions<JustOneContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("JustOne");

            modelBuilder.Entity<Game>().HasMany(s => s.Rounds).WithOne(r => r.Game).HasForeignKey(r => r.GameId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Game>().HasOne(s => s.CurrentRound).WithMany().HasForeignKey(s => s.CurrentRoundId);

            modelBuilder.Entity<Response>().HasOne(s => s.Round).WithMany(r => r.Responses).HasForeignKey(r => r.RoundId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Response>().HasMany(s => s.ResponseVotes).WithOne(r => r.Response).HasForeignKey(r => r.ResponseId).OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerStatus> PlayerStatuses { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseVote> ResponseVotes { get; set; }

    }
}
