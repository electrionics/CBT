using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using CBT.Domain.Identity;
using CBT.Logic.Contracts;
using CBT.SharedComponents.Blazor.Model;
using CBT.SharedComponents.Blazor.Model.ResultData;

namespace CBT.Web.Blazor.Controllers
{
    public class LinkingController(
        UserManager<User> userManager, 
        ILinkingService linkingService, 
        IPeopleService peopleService) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ILinkingService _linkingService = linkingService;
        private readonly IPeopleService _peopleService = peopleService;

        [Route("/api/linking/process")]
        [HttpGet]
        public async Task<CommonResult<LinkProcessingResultData>> LinkWith([FromQuery]string publicId)
        {
            var result = new CommonResult<LinkProcessingResultData>
            {
                Succeeded = false,
                ErrorMessage = null,
                Data = null,
                RedirectUrl = null
            };

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                result.RedirectUrl = "/account/login";
            }

            var link = await _linkingService.GetByPublicId(publicId);
            if (link == null)
            {
                result.ErrorMessage = "Ссылка не найдена";
                return result;
            }
            else
            {
                result.Data = new LinkProcessingResultData { LinkPublicId = publicId };
            }

            if (currentUser != null)
            {
                if (link.UserId == currentUser.Id)
                {
                    result.ErrorMessage = "Ссылка не может быть обработана.";
                    return result;
                }

                var currentPatient = await _peopleService.GetPatient(currentUser.Id);
                var currentPsychologist = await _peopleService.GetPsychologist(currentUser.Id);
                var linkPatient = await _peopleService.GetPatient(link.UserId);
                var linkPsychologist = await _peopleService.GetPsychologist(link.UserId);

                result.Succeeded |= await _peopleService.Connect(currentPatient, linkPsychologist);
                result.Succeeded |= await _peopleService.Connect(linkPatient, currentPsychologist);

                if (!result.Succeeded)
                {
                    var isPsychologist = await _userManager.IsInRoleAsync(currentUser, "Psychologist");
                    result.ErrorMessage = $"Cоединение с пользователем невозможно. Пользователь не является ";
                    result.ErrorMessage += isPsychologist ? "клиентом." : "психологом.";
                }
            }

            return result;
        }
    }
}
