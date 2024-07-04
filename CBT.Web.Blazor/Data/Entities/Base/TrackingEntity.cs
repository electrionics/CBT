
namespace CBT.Web.Blazor.Data.Entities.Base
{
    public class TrackingEntity : ITrackingCreate, ITrackingUpdate
    {
        public DateTime? DateCreated { get; set; }
        public string? UserCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string? UserUpdated { get; set; }
    }
}
