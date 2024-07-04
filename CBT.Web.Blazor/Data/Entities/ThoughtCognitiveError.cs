namespace CBT.Web.Blazor.Data.Entities
{
    public class ThoughtCognitiveError
    {
        public int ThoughtId { get; set; }
        public int CognitiveErrorId { get; set; }
        public int? PsychologistId { get; set; }

        public int ReviewerId { get; set; }
        public bool IsReview => PsychologistId != null;

        public AutomaticThought Thought { get; set; }
    }
}
