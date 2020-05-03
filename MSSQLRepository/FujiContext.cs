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

            modelBuilder.Entity<Session>().HasMany(s => s.Players).WithOne(s => s.Session).HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Session>().HasMany(s => s.DeckCards).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);
            modelBuilder.Entity<Session>().HasMany(s => s.DiscardedCards).WithOne(s => s.Session).HasForeignKey(r => r.SessionId);
                       
            modelBuilder.Entity<Session>().HasOne(s => s.SessionMaster).WithMany().HasForeignKey(s => s.SessionMasterId);
            modelBuilder.Entity<Session>().HasOne(s => s.ActivePlayer).WithMany().HasForeignKey(s => s.ActivePlayerId);

            modelBuilder.Entity<Player>().HasMany(s => s.Cards).WithOne(s => s.Player).HasForeignKey(r => r.PlayerId);
            modelBuilder.Entity<Player>().HasOne(s => s.PlayedCard).WithMany().HasForeignKey(s => s.PlayedCardId);
        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }
        public DbSet<DiscardedCard> DiscardedCards { get; set; }
        public DbSet<HandCard> HandCards { get; set; }
    }
}
