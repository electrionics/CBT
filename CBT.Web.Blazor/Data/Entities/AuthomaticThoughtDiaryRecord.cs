namespace CBT.Web.Blazor.Data.Entities
{
    public class AuthomaticThoughtDiaryRecord
    {
        public int Id { get; set; }

        public string? Situation { get; set; }
        public string Thought { get; set; }
        public string? RationalAnswer { get; set; }

        public int PatientId { get; set; }

        public List<ThoughtCognitiveError> ThoughtCognitiveErrors { get; set; }

        public List<ThoughtEmotion> ThoughtEmotions { get; set; }

        public Patient Patient { get;set; }
    }
}