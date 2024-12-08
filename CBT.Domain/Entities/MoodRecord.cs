namespace CBT.Domain.Entities
{
    public class MoodRecord
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public DateTime DateTime { get; set; }
        public int Value { get; set; }
        public string? Events { get; set; }

        public Patient Patient { get; set; }
    }
}
