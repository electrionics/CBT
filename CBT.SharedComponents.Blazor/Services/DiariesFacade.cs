using System.Text;

using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.Logic.Services;
using CBT.SharedComponents.Blazor.Model;

namespace CBT.SharedComponents.Blazor.Services
{
    public class DiariesFacade(AutomaticThoughtsService automaticThoughtService, AntiProcrastinationRecordService antiProcrastinationRecordService, CBTDataContext dataContext)
    {
        private readonly AutomaticThoughtsService _automaticThoughtService = automaticThoughtService;
        private readonly AntiProcrastinationRecordService _antiProcrastinationRecordService = antiProcrastinationRecordService;
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";

        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueRecordModel>> GetAllThoughts(string? userId = null)
        {
            var method = new ThreeColumnsTechniqueRecordModel().Convert;
            var data = await _automaticThoughtService.GetAllThoughts(userId);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return data
                .Select(method)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddThought

        public async Task<int> AddThought(string thought)
        {
            return await _automaticThoughtService.AddThought(thought);
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique);

            return await _automaticThoughtService.AddThoughtFull(data);
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsTechniqueRecordModel?> GetThought(int id)
        {
            var data = await _automaticThoughtService.GetThought(id);
            return new ThreeColumnsTechniqueRecordModel().Convert(data!);
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            await _automaticThoughtService.EditThoughtFull((data, patientId) =>
            {
                model.ConvertBack(patientId, DiaryType.ThreeColumnsTechnique, data);
            }, model.Id, userId);
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            await _automaticThoughtService.DeleteThought(id);
        }

        #endregion




        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            await _automaticThoughtService.SendThoughtToPsychologist(id);
        }

        #endregion


        #region GetPsychologistReviews

        public async Task<RecordReviewModel[]> GetPsychologistReviews(int thoughtId)
        {
            var data = await _automaticThoughtService.GetPsychologistReviews(thoughtId);

            return data.Select(d =>
                new RecordReviewModel
                {
                    Id = d.ThoughtId,
                    RationalAnswerComment = d.RationalAnswerComment,
                    ReviewedErrors = d.Thought.CognitiveErrors
                        .Where(x => x.PsychologistId == d.PsychologistId)
                        .Select(x => x.CognitiveErrorId)
                        .ToList(),
                    PsychologistDisplayName = d.Psychologist.DisplayName,
                }).ToArray();
        }

        #endregion




        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticThoughtDiaryRecordModel>> GetAllAutomaticThoughts(string? userId = null)
        {
            var data = await _automaticThoughtService.GetAllAutomaticThoughts(userId);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return data
                .Select(new AutomaticThoughtDiaryRecordModel().Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary);

            return await _automaticThoughtService.AddAutomaticThoughtFull(data, userId);
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThoughtDiaryRecordModel?> GetAutomaticThought(int id)
        {
            var data = await _automaticThoughtService.GetAutomaticThought(id);

            return new AutomaticThoughtDiaryRecordModel().Convert(data);
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            await _automaticThoughtService.EditAutomaticThoughtFull((data, patientId) =>
            {
                model.ConvertBack(patientId, DiaryType.ThreeColumnsTechnique, data);
            }, model.Id, userId);
        }

        #endregion




        #region GetAllAntiprocrastinationRecords

        public async Task<List<AntiProcrastinationDiaryRecordModel>> GetAllAntiprocrastinationRecords(string? userId = null)
        {
            var data = await _antiProcrastinationRecordService.GetAllAntiProcrastinationRecords(userId);

#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return data
                .Select(AntiProcrastinationDiaryRecordModel.Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddAntiprocrastinationRecordFull

        public async Task<int> AddAntiprocrastinationRecordFull(AntiProcrastinationDiaryRecordModel model, string? userId)
        {
            var patient = await _dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id);

            return await _antiProcrastinationRecordService.AddAntiProcrastinationRecordFull(data, userId);
        }

        #endregion


        #region GetAntiprocrastinationRecord

        public async Task<AntiProcrastinationDiaryRecordModel?> GetAntiprocrastinationRecord(int id)
        {
            var data = await _antiProcrastinationRecordService.GetAntiProcrastinationRecord(id);

            return AntiProcrastinationDiaryRecordModel.Convert(data);
        }

        #endregion


        #region EditAntiprocrastinationRecordFull

        public async Task EditAntiprocrastinationRecordFull(AntiProcrastinationDiaryRecordModel model, string? userId)
        {
            await _antiProcrastinationRecordService.EditAntiProcrastinationRecordFull((data, patientId) =>
            {
                model.ConvertBack(patientId, data);
            }, model.Id, userId);
        }

        #endregion
    }
}
