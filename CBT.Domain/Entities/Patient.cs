namespace CBT.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }

        public List<PatientPsychologist> Psychologists { get; set; }


        #region Diary Records

        public List<AutomaticThought> AutomaticThoughts { get; set; }

        public List<AntiProcrastinationRecord> AntiProcrastinationRecords { get; set; }

        public List<MoodRecord> MoodRecords { get; set; }

        #endregion
    }
}
