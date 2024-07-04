﻿namespace CBT.Web.Blazor.Data.Entities.Base
{
    public interface ITrackingUpdate
    {
        DateTime? DateUpdated { get; set; }

        string? UserUpdated { get; set; }
    }
}
