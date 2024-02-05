using Microsoft.AspNetCore.Identity;

namespace CBT.Web.Blazor.Data.Model.Identity
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }

        public bool IsLockedOut { get; set; }

        public bool IsNotAllowed { get; set; }

        public bool RequiresTwoFactor { get; set; }
    }
}
