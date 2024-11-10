namespace CBT.SharedComponents.Blazor.Model
{
    public class UserLinkingModel
    {
        public string DisplayName { get; set; }

        public bool IsPatientForCurrent { get; set; }

        public bool IsPsychologistForCurrent { get; set; }


        public int? PatientId { get; set; }

        public int? PsychologistId { get; set; }
    }
}
