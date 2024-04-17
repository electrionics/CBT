using CBT.Web.Blazor.Data.Model.Enums;

namespace CBT.Web.Blazor.Data.Model.Identity
{
    public class RegisterModel
    {
        public RoleType RoleType { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
