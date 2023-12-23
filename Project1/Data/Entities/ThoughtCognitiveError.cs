namespace CBT.Web.Data.Entities
{
    public class ThoughtCognitiveError
    {
        public int ThoughtId { get; set; }
        public int CognitiveErrorId { get; set; }

        public ThreeColumnsTechnique Thought { get; set; }
    }
}
