namespace CBT.Web.Blazor.Data.Entities
{
    public class AuthomaticThoughtDiaryRecord
    {
        public int Id { get; set; }

        public string? Situation { get; set; }
        public string Thought { get; set; }
        public string? RationalAnswer { get; set; }

        public int PatientId { get; set; }
        public bool Sent { get; set; }
        public bool SentBack { get; set; }


        public List<ThoughtCognitiveError> CognitiveErrors { get; set; }

        public List<ThoughtEmotion> Emotions { get; set; }

        public List<ThoughtPsychologistReview> PsychologistReviews { get; set; }

        public Patient Patient { get;set; }
    }
}