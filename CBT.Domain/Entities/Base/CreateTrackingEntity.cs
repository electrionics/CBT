namespace CBT.Domain.Entities.Base
{
    public class CreateTrackingEntity : ITrackingCreate
    {
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
    }
}
