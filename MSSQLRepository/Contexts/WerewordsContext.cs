using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Entities.Werewords;

namespace MSSQLRepository.Contexts
{
    public class WerewordsContext : DbContext
    {
        public WerewordsContext(DbContextOptions<WerewordsContext> dbContextOptions)
            :base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Werewords");

            modelBuilder.Entity<Session>().HasMany(s => s.Players).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);

            modelBuilder.Entity<Session>().HasIndex(u => u.Password)
                .IsUnique();

            modelBuilder.Entity<Session>().HasOne(s => s.SessionMaster).WithMany().HasForeignKey(s => s.SessionMasterId);
            modelBuilder.Entity<Session>().HasOne(s => s.CurrentRound).WithMany().HasForeignKey(s => s.CurrentRoundId);

            modelBuilder.Entity<Round>().HasOne(s => s.Mayor).WithMany().HasForeignKey(s => s.MayorId);
            modelBuilder.Entity<Round>().HasOne(s => s.Session).WithMany().HasForeignKey(s => s.SessionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlayerVote>().HasOne(s => s.Player).WithMany().HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlayerRoundInformation>().HasMany(s => s.Responses).WithOne(r => r.PlayerRoundInformation).HasForeignKey(s => s.PlayerRoundInformationId);
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<PlayerRoundInformation> PlayerRoundInformation { get; set; }
        public DbSet<PlayerVote> PlayerVotes { get; set; }
        public DbSet<PlayerResponse> PlayerResponses { get; set; }
    }
}
