using Entities.Fuji;
using Entities.Fuji.Cards;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSSQLRepository
{
    public class FujiContext : DbContext
    {
        public FujiContext(DbContextOptions<FujiContext> options)
               : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Fuji");

            modelBuilder.Entity<Game>().HasMany(s => s.DeckCards).WithOne(s => s.Game).HasForeignKey(r => r.GameId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Game>().HasMany(s => s.DiscardedCards).WithOne(s => s.Game).HasForeignKey(r => r.GameId).OnDelete(DeleteBehavior.NoAction);
                       
            //modelBuilder.Entity<PlayerInformation>().HasMany(s => s.Cards);
            modelBuilder.Entity<PlayerInformation>().HasOne(s => s.PlayedCard).WithMany().HasForeignKey(s => s.PlayedCardId);
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerInformation> PlayerInformation { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }
        public DbSet<DiscardedCard> DiscardedCards { get; set; }
        public DbSet<HandCard> HandCards { get; set; }
    }
}
