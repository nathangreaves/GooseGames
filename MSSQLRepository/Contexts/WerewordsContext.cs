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

            modelBuilder.Entity<PlayerRoundInformation>().HasMany(s => s.Responses).WithOne(r => r.PlayerRoundInformation).HasForeignKey(s => s.PlayerRoundInformationId);
        }
        
        public DbSet<Round> Rounds { get; set; }
        public DbSet<PlayerRoundInformation> PlayerRoundInformation { get; set; }
        public DbSet<PlayerVote> PlayerVotes { get; set; }
        public DbSet<PlayerResponse> PlayerResponses { get; set; }
    }
}
