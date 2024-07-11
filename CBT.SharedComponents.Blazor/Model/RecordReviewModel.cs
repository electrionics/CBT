namespace CBT.SharedComponents.Blazor.Model
{
    public class RecordReviewModel
    {
        public int Id { get; set; }

        public string PsychologistDisplayName { get; set; }

        public string RationalAnswerComment { get; set; }

        public List<int> ReviewedErrors { get; set; }
    }
}
