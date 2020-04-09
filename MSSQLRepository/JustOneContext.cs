using Entities.JustOne;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository
{
    public class JustOneContext : DbContext
    {
        public JustOneContext(DbContextOptions options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("JustOne");

            modelBuilder.Entity<Session>().HasMany(s => s.Rounds).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);
            modelBuilder.Entity<Session>().HasMany(s => s.Players).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);
            modelBuilder.Entity<Session>().HasOne(s => s.CurrentRound).WithMany().HasForeignKey(s => s.CurrentRoundId);
            modelBuilder.Entity<Session>().HasOne(s => s.SessionMaster).WithMany().HasForeignKey(s => s.SessionMasterId);

            modelBuilder.Entity<Round>().HasOne(s => s.ActivePlayer).WithMany().HasForeignKey(r => r.ActivePlayerId);

            modelBuilder.Entity<PlayerStatus>().HasOne(s => s.Player).WithOne(s => s.PlayerStatus).HasForeignKey<PlayerStatus>(s => s.PlayerId);
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerStatus> PlayerStatuses { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<ResponseVote> ResponseVotes { get; set; }

    }
}
