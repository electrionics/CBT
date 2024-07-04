using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using FluentValidation;

using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Data.Model.Enums;
using CBT.Web.Blazor.Data.Model.Identity;

using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CBT.Web.Blazor.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        private readonly IValidator<RegisterModel> _registerValidator;

        private readonly CBTDataContext _dataContext;

        private readonly ILogger<AccountController> _logger;

        public AccountController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            CBTDataContext dataContext,
            IValidator<RegisterModel> registerValidator,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<User>)userStore;
            _dataContext = dataContext;
            _registerValidator = registerValidator;
            _logger = logger;
        }

        [HttpPost]
        [Route("/api/account/login")]
        public async Task<SignInResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            return result;
        }

        [HttpPost]
        [Route("/api/account/logout")]
        public async Task<bool> Logout()
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        [HttpPost]
        [Route("/api/account/register")]
        public async Task<RegisterResult> Register([FromBody] RegisterModel model)
        {
            var validationResult = await _registerValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    ErrorMessage = validationResult.Errors.First().ErrorMessage
                };
            }

            var user = new User();

            await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
            
            var result = await _userManager.CreateAsync(user, model.Password);
            
            if (result.Succeeded)
            {
                try
                {
                    if (model.RoleTypes.Contains(RoleType.Client))
                    {
                        await _userManager.AddToRoleAsync(user, "Client");
                        _dataContext.Set<Patient>().Add(new Patient
                        {
                            DisplayName = model.Name,
                            UserId = user.Id
                        });
                    }
                    if (model.RoleTypes.Contains(RoleType.Psychologist))
                    {
                        await _userManager.AddToRoleAsync(user, "Psychologist");
                        _dataContext.Set<Psychologist>().Add(new Psychologist
                        {
                            DisplayName = model.Name,
                            UserId = user.Id,
                        });
                    }
                    await _dataContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при регистрации.");

                    return new RegisterResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Ошибка на сервере.",
                    };
                }
                
                // roles
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                return new RegisterResult
                {
                    Succeeded = true
                };
            }
            else
            {
                return new RegisterResult
                {
                    Succeeded = false,
                    ErrorMessage = result.Errors.First().Description,
                };
            }
        }
    }
}
