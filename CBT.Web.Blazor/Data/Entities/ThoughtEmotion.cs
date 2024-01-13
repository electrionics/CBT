using CBT.Web.Blazor.Data.Entities.Enums;

namespace CBT.Web.Blazor.Data.Entities
{
    public class ThoughtEmotion
    {
        public int ThoughtId { get; set; }
        public int EmotionId { get; set; }
        public int Value { get; set; }
        public ThoughtEmotionState State { get; set; }

        public AuthomaticThoughtDiaryRecord Thought { get; set; }
    }
}
