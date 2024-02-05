namespace CBT.Web.Blazor.Data.Entities
{
    public class Psychologist
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }


        public List<ThoughtPsychologistReview> ThoughtReviews { get; set; }
    }
}