using Microsoft.AspNetCore.Identity;

using CBT.Domain.Identity;
using CBT.Logic.Services;
using CBT.SharedComponents.Blazor.Model;

namespace CBT.SharedComponents.Blazor.Services
{
    public class LinkingFacade(
        UserManager<User> userManager,
        LinkingService linkingService,
        PeopleService peopleService)
    {
        private readonly UserManager<User> _userManager = userManager;

        private readonly LinkingService _linkingService = linkingService;
        private readonly PeopleService _peopleService = peopleService;

        public async Task<ProfileModel> GetModelByUserId(string userId) 
        {
            var link = await _linkingService.GetByUserId(userId);
            link ??= await _linkingService.CreateNewLink(userId);

            var patient = await _peopleService.GetPatient(userId);
            var psychologist = await _peopleService.GetPsychoilogist(userId);
            var connections = await _peopleService.GetConnectionsFor(patient?.Id, psychologist?.Id);

            var currentUser = await _userManager.FindByIdAsync(userId);

            var result = new ProfileModel
            {
                DisplayName = patient?.DisplayName ?? psychologist?.DisplayName ?? "",
                Email = currentUser?.Email!,
                PublicId = link.PublicId,
                LinkedUsers = connections.Select(x => new UserLinkingModel
                {
                    DisplayName = x.PatientId == patient?.Id
                        ? x.Psychologist.DisplayName
                        : x.Patient.DisplayName,

                    IsPatientForCurrent = x.PsychologistId == psychologist?.Id,
                    IsPsychologistForCurrent = x.PatientId == patient?.Id
                }).ToList(),
            };

            return result;
        }

        public async Task<string> RecreateLink(string userId)
        {
            var link = await _linkingService.CreateNewLink(userId);

            return link.PublicId;
        }
    }
}
