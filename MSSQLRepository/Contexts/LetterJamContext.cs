using Entities.LetterJam;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository.Contexts
{
    public class LetterJamContext : DbContext
    {
        public LetterJamContext(DbContextOptions<LetterJamContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("LetterJam");

            modelBuilder.Entity<PlayerState>().Property(x => x.Status)
                .HasConversion(p => p.Id, g => new Entities.LetterJam.Enums.PlayerStatusId { Id = g });


            modelBuilder.Entity<Game>().HasOne(x => x.CurrentRound).WithMany().HasForeignKey(x => x.CurrentRoundId);
            modelBuilder.Entity<Game>().HasIndex(x => x.SessionId);

            modelBuilder.Entity<Round>().HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId);

            modelBuilder.Entity<PlayerState>().HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId);
            modelBuilder.Entity<PlayerState>().HasIndex(x => x.PlayerId);

            modelBuilder.Entity<PlayerState>().HasOne(x => x.CurrentLetter).WithMany().HasForeignKey(x => x.CurrentLetterId);
            modelBuilder.Entity<NonPlayerCharacter>().HasOne(x => x.CurrentLetter).WithMany().HasForeignKey(x => x.CurrentLetterId);

            modelBuilder.Entity<LetterCard>().HasOne(x => x.Game).WithMany().HasForeignKey(x => x.GameId);

            modelBuilder.Entity<Clue>().HasOne(c => c.Round).WithMany().HasForeignKey(c => c.RoundId);
            modelBuilder.Entity<ClueVote>().HasOne(c => c.Round).WithMany().HasForeignKey(c => c.RoundId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ClueVote>().HasOne(c => c.Clue).WithMany().HasForeignKey(c => c.ClueId);
            modelBuilder.Entity<ClueLetter>().HasOne(c => c.Clue).WithMany().HasForeignKey(c => c.ClueId);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<PlayerState> PlayerStates { get; set; }
        public DbSet<NonPlayerCharacter> NonPlayerCharacters { get; set; }
        public DbSet<LetterCard> LetterCards { get; set; }
        public DbSet<Clue> Clues { get; set; }
        public DbSet<ClueVote> ClueVotes { get; set; }
        public DbSet<ClueLetter> ClueLetters { get; set; }
    }
}
