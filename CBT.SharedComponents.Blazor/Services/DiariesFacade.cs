using System.Text;

using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.SharedComponents.Blazor.Model;
using CBT.Logic.Services;

namespace CBT.SharedComponents.Blazor.Services
{
    public class DiariesFacade
    {
        private readonly AutomaticThoughtsService service;
        private readonly CBTDataContext dataContext;

        private const string DemoUserId = "DemoClient";

        public DiariesFacade(AutomaticThoughtsService service, CBTDataContext dataContext)
        {
            this.service = service;
            this.dataContext = dataContext;
        }

        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueRecordModel>> GetAllThoughts(string? userId = null)
        {
            var method = new ThreeColumnsTechniqueRecordModel().Convert;
            var data = await service.GetAllThoughts(userId);

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
            return await service.AddThought(thought);
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique);

            return await service.AddThoughtFull(data, userId);
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsTechniqueRecordModel?> GetThought(int id)
        {
            var data = await service.GetThought(id);
            return new ThreeColumnsTechniqueRecordModel().Convert(data!);
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            await service.EditThoughtFull((data, patientId) =>
            {
                model.ConvertBack(patientId, DiaryType.ThreeColumnsTechnique, data);
            }, model.Id, userId);
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            await service.DeleteThought(id);
        }

        #endregion


        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            await service.SendThoughtToPsychologist(id);
        }

        #endregion




        #region GetPsychologistReviews

        public async Task<RecordReviewModel[]> GetPsychologistReviews(int thoughtId)
        {
            var data = await service.GetPsychologistReviews(thoughtId);

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
            var data = await service.GetAllAutomaticThoughts(userId);

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
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary);

            return await service.AddAutomaticThoughtFull(data, userId);
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThoughtDiaryRecordModel?> GetAutomaticThought(int id)
        {
            var data = await service.GetAutomaticThought(id);

            return new AutomaticThoughtDiaryRecordModel().Convert(data);
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            await service.EditAutomaticThoughtFull((data, patientId) =>
            {
                model.ConvertBack(patientId, DiaryType.ThreeColumnsTechnique, data);
            }, model.Id, userId);
        }

        #endregion
    }
}
