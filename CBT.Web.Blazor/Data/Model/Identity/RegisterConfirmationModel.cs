// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

namespace CBT.Web.Blazor.Data.Model.Identity
{
    public class RegisterConfirmationModel
    {
        public string Email { get; set; }

        public bool DisplayConfirmAccountLink { get; set; }

        public string EmailConfirmationUrl { get; set; }

        public string RedirectRelativeUrl { get; set; }
    }
}
