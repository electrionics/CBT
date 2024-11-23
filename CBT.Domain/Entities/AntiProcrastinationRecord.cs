namespace CBT.Domain.Entities
{
    public class AntiProcrastinationRecord
    {
        public int Id { get; set; }

        public string Task { get; set; }

        public int? SupposedEffort { get; set; } // warning if null, save anyway

        public int? SupposedPleasure { get; set; } // warning if null, save anyway

        public int? ActualEffort { get; set; }

        public int? ActualPleasure { get; set; }


        public DateTime? DateTimeDone { get; set; }

        public DateTime? PlanDate { get; set; } // if null - it's not planned


        public int PatientId { get; set; }

        public Patient Patient { get; set; }
    }
}
