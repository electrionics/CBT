using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;

using CBT.Domain.Entities;
using CBT.Domain.Entities.Base;

namespace CBT.Domain
{
    public class CBTDataContext : DbContext
    {
        private readonly string? _connectionString;
        private readonly IHttpContextAccessor _contextAccessor;

        #region Initialization

        public CBTDataContext(DbContextOptions<CBTDataContext> options, IHttpContextAccessor contextAccessor) : base(options)
        {
            _contextAccessor = contextAccessor;
        }

        public CBTDataContext()
        {

        }

        public CBTDataContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected CBTDataContext(DbContextOptions options, IHttpContextAccessor contextAccessor) : base(options) 
        {
            _contextAccessor = contextAccessor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(_connectionString))
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }

            //optionsBuilder.EnableSensitiveDataLogging();
            //optionsBuilder.AddInterceptors(new TaggedQueryCommandInterceptor());
        }

        #endregion


        #region Mapping

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

            modelBuilder.Entity<AntiProcrastinationRecord>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Patient)
                    .WithMany(x => x.AntiProcrastinationRecords)
                    .HasForeignKey(x => x.PatientId);
            });

            modelBuilder.Entity<ThoughtCognitiveError>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId, x.CognitiveErrorId, x.ReviewerId });

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
                entity.HasOne(x => x.Emotion)
                    .WithMany(x => x.ThoughtEmotions)
                    .HasForeignKey(x => x.EmotionId);
            });

            modelBuilder.Entity<ThoughtPsychologistReview>(entity =>
            {
                entity.HasKey(x => new { x.ThoughtId, x.PsychologistId });

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

            modelBuilder.Entity<PatientPsychologist>(entity =>
            {
                entity.HasKey(x => new { x.PatientId, x.PsychologistId });

                entity.HasOne(x => x.Patient)
                    .WithMany(x => x.Psychologists)
                    .HasForeignKey(x => x.PatientId);

                entity.HasOne(x => x.Psychologist)
                    .WithMany(x => x.Patients)
                    .HasForeignKey(x => x.PsychologistId);
            });

            //modelBuilder.Entity<TaskEntity>(entity =>
            //{
            //    entity.ToTable("Task");

            //    entity.HasKey(x => x.Id);

            //    entity.HasOne(x => x.ParentTask)
            //        .WithMany(x => x.ChildTasks)
            //        .HasForeignKey(x => x.ParentTaskId);

            //    entity.HasOne(x => x.PatientOwner)
            //        .WithMany(x => x.Tasks)
            //        .HasForeignKey(x => x.PatientOwnerId);

            //    entity.HasOne(x => x.ParentThought)
            //        .WithMany(x => x.Tasks)
            //        .HasForeignKey(x => x.ParentThoughtId);
            //});

            modelBuilder.Entity<Link>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            #endregion
        }

        #endregion

        #region Tracking

        public override EntityEntry Add(object entity)
        {
            if (entity is ITrackingCreate trackable)
                CreateEntity(trackable);

            return base.Add(entity);
        }

        public override void AddRange(IEnumerable<object> entities)
        {
            var now = DateTime.Now;
            foreach (object entity in entities)
                if (entity is ITrackingCreate trackable)
                    CreateEntity(trackable, now);

            base.AddRange(entities);
        }

        public override EntityEntry Update(object entity)
        {
            if (entity is ITrackingUpdate trackable)
                UpdateEntity(trackable);

            return base.Update(entity);
        }

        public override void UpdateRange(IEnumerable<object> entities)
        {
            var now = DateTime.Now;
            foreach (object entity in entities)
                if (entity is ITrackingUpdate trackable)
                    UpdateEntity(trackable, now);

            base.UpdateRange(entities);
        }

        #region Helper Methods

        private void CreateEntity(ITrackingCreate trackable, DateTime? now = null)
        {
            trackable.DateCreated = now ?? DateTime.Now;
            trackable.UserCreated = CurrentUserName!;
        }

        private void UpdateEntity(ITrackingUpdate trackable, DateTime? now = null)
        {
            trackable.DateUpdated = now ?? DateTime.Now;
            trackable.UserUpdated = CurrentUserName;
        }

        private string? CurrentUserName => _contextAccessor?.HttpContext?.User.Identity?.Name;

        #endregion

        #endregion
    }
}