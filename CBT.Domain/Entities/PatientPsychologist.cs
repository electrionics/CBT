using CBT.Domain.Entities.Base;

namespace CBT.Domain.Entities
{
    public class PatientPsychologist : CreateTrackingEntity
    {
        public int PatientId { get; set; }

        public int PsychologistId { get; set; }


        public Patient Patient { get; set; }

        public Psychologist Psychologist { get; set; }
    }
}
