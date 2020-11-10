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

        }

        public DbSet<Game> Games { get; set; }
    }
}
