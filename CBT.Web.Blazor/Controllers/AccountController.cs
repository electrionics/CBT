using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Data.Model.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CBT.Web.Blazor.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;

        public AccountController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("/api/account/login")]
        [ValidateAntiForgeryToken]
        public async Task<SignInResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            return result;
        }

        [HttpPost]
        [Route("/api/account/logout")]
        [ValidateAntiForgeryToken]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
