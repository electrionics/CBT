using System.ComponentModel.DataAnnotations;

namespace CBT.SharedComponents.Blazor.Model.Identity
{
    public class LoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
