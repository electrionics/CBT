using System.ComponentModel.DataAnnotations;

namespace CBT.Web.Blazor.Data.Model.Identity
{
    public class LoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
