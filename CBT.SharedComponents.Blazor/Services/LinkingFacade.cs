using Microsoft.AspNetCore.Identity;

using CBT.Domain.Identity;
using CBT.SharedComponents.Blazor.Model;
using CBT.Logic.Services;
using CBT.Domain.Entities;

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
            var psychologist = await _peopleService.GetPsychologist(userId);
            var connections = await _peopleService.GetConnectionsFor(patient?.Id, psychologist?.Id);

            var currentUser = await _userManager.FindByIdAsync(userId);

            var result = new ProfileModel
            {
                DisplayName = patient?.DisplayName ?? psychologist?.DisplayName ?? "",
                Email = currentUser?.Email!,
                PublicId = link.PublicId,

                IsPatient = await _userManager.IsInRoleAsync(currentUser!, "Client"),
                IsPsychologist = await _userManager.IsInRoleAsync(currentUser!, "Psychologist"),

                LinkedUsers = connections.Select(x => new UserLinkingModel
                {
                    DisplayName = x.PatientId == patient?.Id
                        ? x.Psychologist.DisplayName
                        : x.Patient.DisplayName,

                    IsPatientForCurrent = x.PsychologistId == psychologist?.Id && x.Enabled,
                    IsPsychologistForCurrent = x.PatientId == patient?.Id && x.Enabled,

                    PatientId = x.PatientId,
                    PsychologistId = x.PsychologistId,
                }).ToList(),
            };

            return result;
        }

        public async Task<string> RecreateLink(string userId)
        {
            var link = await _linkingService.CreateNewLink(userId);

            return link.PublicId;
        }

        public async Task SaveLinkedUsers(string userId, List<UserLinkingModel> linkedUsers)
        {
            var currentPatient = await _peopleService.GetPatient(userId);
            var currentPsychologist = await _peopleService.GetPsychologist(userId);

            foreach (var linkedUser in linkedUsers)
            {
                if (linkedUser.PatientId != null &&
                    currentPsychologist != null)
                {
                    var linkedPatient = await _peopleService.GetPatient(linkedUser.PatientId.Value);

                    await _peopleService.Connect(linkedPatient!, currentPsychologist, linkedUser.IsPatientForCurrent);
                }

                if (linkedUser.PsychologistId != null &&
                    currentPatient != null)
                {
                    var linkedPsychologist = await _peopleService.GetPsychologist(linkedUser.PsychologistId.Value);

                    await _peopleService.Connect(currentPatient, linkedPsychologist!, linkedUser.IsPsychologistForCurrent);
                }
            }
        }

        public async Task SetConnectionWithPatient(string currentUserId, int patientId, bool enable)
        {
            var currentPsychologist = await _peopleService.GetPsychologist(currentUserId);

            if (currentPsychologist != null)
            {
                var linkedPatient = await _peopleService.GetPatient(patientId);

                await _peopleService.Connect(linkedPatient!, currentPsychologist, enable);
            }
        }

        public async Task SetConnectionWithPsychologist(string currentUserId, int psychologistId, bool enable)
        {
            var currentPatient = await _peopleService.GetPatient(currentUserId);

            if (currentPatient != null)
            {
                var linkedPsychologist = await _peopleService.GetPsychologist(psychologistId);

                await _peopleService.Connect(currentPatient, linkedPsychologist!, enable);
            }
        }
    }
}
