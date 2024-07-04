namespace CBT.Web.Blazor.Data.Entities.Base
{
    public interface ITrackingCreate
    {
        DateTime? DateCreated { get; set; }

        string? UserCreated { get; set; }
    }
}
