namespace CBT.Web.Blazor.Data.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public int PsychologistId { get; set; }


        public Psychologist Psychologist { get; set; }
    }
}
