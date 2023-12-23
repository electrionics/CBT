namespace CBT.Web.Data.Entities
{
    public class ThreeColumnsTechnique
    {
        public int Id { get; set; }
        public string Thought { get; set; }
        public string? RationalAnswer { get; set; }

        public int UserId { get; set; }

        public List<ThoughtCognitiveError> ThoughtCognitiveErrors { get; set; }
    }
}