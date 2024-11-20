namespace CBT.Domain.Entities
{
    public class Emotion
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool Positive { get; set; }
        public string? Description { get; set; }

        public List<ThoughtEmotion> ThoughtEmotions { get; set; }
    }
}
