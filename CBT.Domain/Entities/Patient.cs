namespace CBT.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }

        public List<PatientPsychologist> Psychologists { get; set; }

        public List<TaskEntity> Tasks { get; set; }
    }
}
