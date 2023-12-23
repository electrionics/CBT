using CBT.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Data
{
    public class CBTDataContext : DbContext
    {
        public CBTDataContext(DbContextOptions<CBTDataContext> options) : base(options)
        {
        }

        public CBTDataContext()
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=.;database=CBT;trusted_connection=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Three Columns Technique

            modelBuilder.Entity<CognitiveError>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ThreeColumnsTechnique>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ThoughtCognitiveError>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId, x.CognitiveErrorId });
                entity.HasOne(x => x.Thought)
                    .WithMany(x => x.ThoughtCognitiveErrors)
                    .HasForeignKey(x => x.ThoughtId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion
        }
    }

    
}