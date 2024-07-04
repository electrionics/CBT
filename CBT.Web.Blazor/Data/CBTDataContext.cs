using Microsoft.EntityFrameworkCore;

using CBT.Web.Blazor.Data.Entities;

namespace CBT.Web.Blazor.Data
{
    public class CBTDataContext : DbContext
    {
        private readonly string? _connectionString;

        public CBTDataContext(DbContextOptions<CBTDataContext> options) : base(options)
        {
            
        }

        public CBTDataContext()
        {

        }

        public CBTDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected CBTDataContext(DbContextOptions options) : base(options) 
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Three Columns Technique

            modelBuilder.Entity<Emotion>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<CognitiveError>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<AutomaticThought>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<ThoughtCognitiveError>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId, x.CognitiveErrorId, x.IsReview });
                entity.HasOne(x => x.Thought)
                    .WithMany(x => x.CognitiveErrors)
                    .HasForeignKey(x => x.ThoughtId);
            });

            modelBuilder.Entity<ThoughtEmotion>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId, x.EmotionId, x.State });
                entity.HasOne(x => x.Thought)
                    .WithMany(x => x.Emotions)
                    .HasForeignKey(x => x.ThoughtId);
            });

            modelBuilder.Entity<ThoughtPsychologistReview>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId });

                entity.HasOne(x => x.Thought)
                    .WithMany(x => x.PsychologistReviews)
                    .HasForeignKey(x => x.ThoughtId);

                entity.HasOne(x => x.Psychologist)
                    .WithMany(x => x.ThoughtReviews)
                    .HasForeignKey(x => x.PsychologistId);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Psychologist>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion
        }
    }
}