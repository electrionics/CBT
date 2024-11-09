namespace CBT.Domain.Entities.Base
{
    public interface ITrackingCreate
    {
        DateTime? DateCreated { get; set; }

        string? UserCreated { get; set; }
    }
}
