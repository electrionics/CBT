namespace CBT.Web.Blazor.Data.Entities
{
    public class ThoughtCognitiveError
    {
        public int ThoughtId { get; set; }
        public int CognitiveErrorId { get; set; }

        public AuthomaticThoughtDiaryRecord Thought { get; set; }
    }
}
