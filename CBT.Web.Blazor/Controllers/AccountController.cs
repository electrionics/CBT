using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

using FluentValidation;

using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Identity;
using CBT.Web.Blazor.Data.Model.Enums;
using CBT.Web.Blazor.Data.Model.Identity;

using NavigationManager = Microsoft.AspNetCore.Components.NavigationManager;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using CBT.Web.Blazor.Common;


namespace CBT.Web.Blazor.Controllers
{
    public class AccountController : Controller
    {
        #region Dependencies

        private readonly CBTDataContext _dataContext;
        private readonly ILogger<AccountController> _logger;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IValidator<RegisterModel> _registerValidator;
        private readonly IValidator<ResendConfirmationModel> _resendConfirmationValidator;
        private readonly IValidator<ResetPasswordModel> _resetPasswordValidator;

        #endregion

        #region ctor
        public AccountController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IUserStore<User> userStore,
            CBTDataContext dataContext,
            ILogger<AccountController> logger,
            IEmailSender emailSender,
            IHttpContextAccessor httpContextAccessor,
            IValidator<RegisterModel> registerValidator,
            IValidator<ResendConfirmationModel> resendConfirmationValidator,
            IValidator<ResetPasswordModel> resetPasswordValidator)
        {
            _dataContext = dataContext;
            _logger = logger;

            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = (IUserEmailStore<User>)userStore;

            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;

            _registerValidator = registerValidator;
            _resendConfirmationValidator = resendConfirmationValidator;
            _resetPasswordValidator = resetPasswordValidator;
        }
        #endregion

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

        [HttpGet]
        [Route("/api/account/register/confirmation")]
        public async Task<RegisterConfirmationModel> RegisterConfirmation([FromQuery]string email, [FromQuery]string? returnUrl = null)
        {
            var indexRelativeUrl = "/";

            if (email == null)
            {
                return new() 
                { 
                    RedirectRelativeUrl = indexRelativeUrl
                };
            }

            returnUrl = returnUrl ?? indexRelativeUrl;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new() 
                { 
                    RedirectRelativeUrl = "/error/notfound"
                };
            }

            var confirmationRelativeUrl = await GetConfirmationUrl(user, returnUrl);
            var model = new RegisterConfirmationModel
            {
                Email = email,
                DisplayConfirmAccountLink = true, //_emailSender is not implemented
                EmailConfirmationUrl = _httpContextAccessor
                    .ToAbsoluteUri(confirmationRelativeUrl)
            };

            if (model.DisplayConfirmAccountLink)
            {
                return model;
            }
            else
            {
                var callbackUrl = model.EmailConfirmationUrl;

                try
                {
                    await _emailSender.SendEmailAsync(
                        model.Email,
                        "Подтверждение регистрации на psyonic.ru",
                            $"<html>" +
                            $"  <head></head>" +
                            $"  <body>" +
                            $"    <h1>ПСИОНИК - Подтверждение регистрации.</h1>" +
                            $"    <a href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">Нажмите, чтобы завершить регистрацию.</a>" +
                            $"  </body>" +
                            $"</html>"
                        );

                    model.RedirectRelativeUrl = "/account/login";
                }
                catch (Exception ex)
                {
                    model.RedirectRelativeUrl = null;

                    _logger.LogError(ex, "Письмо не отправлено.");
                }

                model.EmailConfirmationUrl = null;
            }

            return model;
        }

        private async Task<string> GetConfirmationUrl(User user, string? returnUrl)
        {
            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var result = $"/api/account/confirmEmail?userId={userId}&code={code}&returnUrl={returnUrl}";

            return result;
        }

        [HttpPost]
        [Route("/api/account/register/confirmation/resend")]
        public async Task<ResendConfirmationResult> ResendEmailConfirmation([FromBody]ResendConfirmationModel model)
        {
            var result = new ResendConfirmationResult { Success = false };

            var validationResult = await _resendConfirmationValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                result.ErrorMessage = validationResult.Errors.First().ErrorMessage;
                return result;
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                result.ErrorMessage = "Пользователь не найден. Проверьте правильность написания.";
                return result;
            }

            var indexRelativeUrl = "/";
            var confirmationRelativeUrl = await GetConfirmationUrl(user, indexRelativeUrl);
            var callbackUrl = _httpContextAccessor
                    .ToAbsoluteUri(confirmationRelativeUrl);

            try
            {
                #region Send Email

                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Подтвердите адрес электронной почты.",
                        $"<html>" +
                        $"  <head></head>" +
                        $"  <body>" +
                        $"    <h1>ПСИОНИК - Подтверждение учетной записи.</h1>" +
                        $"    <a href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">Нажмите, чтобы завершить регистрацию.</a>" +
                        $"  </body>" +
                        $"</html>"
                    );

                #endregion

                #region Email Confirmation Logic
                //TODO: place to separate methods, when email will be setup

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                result.Success = await ConfirmEmail(user.Id, token);
                result.ErrorMessage = result.Success
                    ? null
                    : "Не удалось подтвердить адрес электронной почты.";

                #endregion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Письмо не отправлено.");

                result.ErrorMessage = "Письмо не отправлено. Повторите попытку позже.";
            }
            
            return result;
        }

        //[HttpGet]
        //[Route("api/account/resetpassword")]
        //public Task<ResetPasswordModel?> GetResetPassword([FromQuery]string? code = null)
        //{
        //    ResetPasswordModel? result = null;

        //    if (code != null)
        //    {
        //        result = new()
        //        {
        //            Code = code// Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
        //        };
        //    }

        //    return Task.FromResult(result);
        //}

        [HttpPost]
        [Route("api/account/resetpassword")]
        public async Task<ResetPasswordResult> SendResetPassword([FromBody]ResetPasswordModel model)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return new ()
                {
                    Success = false,
                    ErrorMessage = validationResult.Errors.First().ErrorMessage
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            var redirectRelativeUrl = "/account/resetpassword/confirmation";
            if (user == null)
            {
                return new()
                {
                    RedirectRelativeUrl = redirectRelativeUrl,
                    Success = false,
                    ErrorMessage = validationResult.Errors.First().ErrorMessage
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            return new()
            {
                RedirectRelativeUrl = result.Succeeded 
                    ? redirectRelativeUrl
                    : null,
                Success = result.Succeeded,
                ErrorMessage = validationResult
                        .Errors
                        .FirstOrDefault()?
                        .ErrorMessage ?? ""
            };
        }

        [HttpGet]
        [Route("api/account/confirmEmail")]
        public async Task<bool> ConfirmEmail(string userId, string code, string? returnUrl = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var confirmResult = await _userManager.ConfirmEmailAsync(user, code);

            return confirmResult.Succeeded;
        }
    }
}
