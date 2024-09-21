using CBT.Domain.Entities.Base;

namespace CBT.Domain.Entities
{
    public class TaskEntity: TrackingEntity
    {
        public int Id { get; set; }

        #region References

        public int? ParentThoughtId { get; set; }

        public int? ParentTaskId {  get; set; }

        public int PatientOwnerId { get; set; }

        public string? NonOwnerCreatorId { get; set; }


        public AutomaticThought? ParentThought { get; set; }

        public TaskEntity? ParentTask { get; set; }

        public Patient PatientOwner { get; set; }


        public List<TaskEntity> ChildTasks { get; set; }

        #endregion

        public string? Category { get; set; }

        public string Text { get; set; }

        public string? Comment { get; set; }

        public int CompletionPercent { get; set; }
    }
}
