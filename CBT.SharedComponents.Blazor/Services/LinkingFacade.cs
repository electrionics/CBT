using Microsoft.AspNetCore.Identity;

using CBT.Domain.Identity;
using CBT.SharedComponents.Blazor.Model;
using CBT.Logic.Contracts;

namespace CBT.SharedComponents.Blazor.Services
{
    public class LinkingFacade(
        UserManager<User> userManager,
        ILinkingService linkingService,
        IPeopleService peopleService)
    {
        private readonly UserManager<User> _userManager = userManager;

        private readonly ILinkingService _linkingService = linkingService;
        private readonly IPeopleService _peopleService = peopleService;

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
                IsPsychologist = await _userManager.IsInRoleAsync(currentUser!, "Psychologist")
            };

            result.LinkedUsers = connections
                .SelectMany(connection => new[]
                {
                    new
                    {
                        LinkUserId = result.IsPatient && connection.Psychologist.UserId != currentUser?.Id 
                            ? connection.Psychologist.UserId 
                            : null,
                        Data = connection
                    },
                    new
                    {
                        LinkUserId = result.IsPsychologist && connection.Patient.UserId != currentUser?.Id
                            ? connection.Patient.UserId 
                            : null,
                        Data = connection
                    }
                })
                .Where(link => link.LinkUserId != null)
                .GroupBy(link => link.LinkUserId)
                .Select(paired => new UserLinkingModel
                {
                    DisplayName =
                            paired.FirstOrDefault(x => x.Data.PsychologistId != psychologist?.Id)?.Data.Psychologist.DisplayName ??
                            paired.First(x => x.Data.PatientId != patient?.Id).Data.Patient.DisplayName,

                    IsPatientForCurrent = paired.Any(x => x.Data.PsychologistId == psychologist?.Id && x.Data.Enabled),
                    IsPsychologistForCurrent = paired.Any(x => x.Data.PatientId == patient?.Id && x.Data.Enabled),

                    PatientId = paired.FirstOrDefault(x => x.Data.PatientId != patient?.Id)
                            ?.Data.PatientId,
                    PsychologistId = paired.FirstOrDefault(x => x.Data.PsychologistId != psychologist?.Id)
                            ?.Data.PsychologistId,
                }).ToList();

            return result;
        }

        public async Task<string> RecreateLink(string userId)
        {
            var link = await _linkingService.CreateNewLink(userId);

            return link.PublicId;
        }

        public async Task<bool> SetConnectionWithPatient(string currentUserId, int patientId, bool enable)
        {
            var currentPsychologist = await _peopleService.GetPsychologist(currentUserId);

            if (currentPsychologist != null)
            {
                var linkedPatient = await _peopleService.GetPatient(patientId);

                return await _peopleService.Connect(linkedPatient!, currentPsychologist, enable);
            }

            return false;
        }

        public async Task<bool> SetConnectionWithPsychologist(string currentUserId, int psychologistId, bool enable)
        {
            var currentPatient = await _peopleService.GetPatient(currentUserId);

            if (currentPatient != null)
            {
                var linkedPsychologist = await _peopleService.GetPsychologist(psychologistId);

                return await _peopleService.Connect(currentPatient, linkedPsychologist!, enable);
            }

            return false;
        }

        public async Task<bool> DeleteConnectionWithPatient(string currentUserId, int patientId)
        {
            var currentPsychologist = await _peopleService.GetPsychologist(currentUserId);
            var patient = await _peopleService.GetPatient(patientId);

            return await _peopleService.DeleteConnection(patient, currentPsychologist);
        }

        public async Task<bool> DeleteConnectionWithPsychologist(string currentUserId, int psychologistId)
        {
            var currentPatient = await _peopleService.GetPatient(currentUserId);
            var psychologist = await _peopleService.GetPsychologist(psychologistId);

            return await _peopleService.DeleteConnection(currentPatient, psychologist);
        }
    }
}
