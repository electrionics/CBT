using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using CBT.Domain.Identity;
using CBT.Logic.Services;
using CBT.SharedComponents.Blazor.Model;

namespace CBT.Web.Blazor.Controllers
{
    public class LinkingController(
        UserManager<User> userManager, 
        LinkingService linkingService, 
        PeopleService peopleService) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly LinkingService _linkingService = linkingService;
        private readonly PeopleService _peopleService = peopleService;

        [Route("/api/linking/process")]
        [HttpGet]
        public async Task<CommonResult> LinkWith([FromQuery]string publicId)
        {
            var result = new CommonResult
            {
                Succeeded = false,
                ErrorMessage = null
            };

            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                result.ErrorMessage = "Пользователь не найден";
                return result;
            }

            var link = await _linkingService.GetByPublicId(publicId);
            if (link == null)
            {
                result.ErrorMessage = "Ссылка не найдена";
                return result;
            }

            var currentPatient = await _peopleService.GetPatient(currentUser.Id);
            var currentPsychologist = await _peopleService.GetPsychologist(currentUser.Id);
            var linkPatient = await _peopleService.GetPatient(link.UserId);
            var linkPsychologist = await _peopleService.GetPsychologist(link.UserId);

            if (currentPatient != null && linkPsychologist != null)
            {
                result.Succeeded |= await _peopleService.Connect(currentPatient, linkPsychologist);
            }

            if (linkPatient != null && currentPsychologist != null)
            {
                result.Succeeded |= await _peopleService.Connect(linkPatient, currentPsychologist);
            }

            if (!result.Succeeded)
            {
                result.ErrorMessage = "Ссылка не позволяет соединить вас с Пользователем";
            }

            return result;
        }
    }
}
