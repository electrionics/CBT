using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Entities.Enums;
using CBT.Web.Blazor.Data.Model;

using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Services
{
    public class AutomaticThoughtsService
    {
        private readonly ILogger<AutomaticThoughtsService> _logger; //TODO: use logger in this class
        private const string DemoUserId = "DemoClient";

        public AutomaticThoughtsService(ILogger<AutomaticThoughtsService> logger)
        {
            _logger = logger;
        }

        #region GetAllCognitiveErrors

        public Dictionary<int, CognitiveErrorModel> GetAllCognitiveErrors()
        {
            return new Dictionary<int, CognitiveErrorModel>()
            {
                { (int)CognitiveErrors.AllOrNothing, new CognitiveErrorModel { Key = (int)CognitiveErrors.AllOrNothing, 
                    Title = "Всё или ничего", 
                    Description = "Этот вид когнитивных искажений описывает склонность к оценке своих личных качеств исключительно в чёрно-белых тонах. Мышление \"всё или ничего\" составляет основу перфекционизма. Оно вызывает страх перед любой ошибкой или несовершенством, потому что тогда вы будете считать себя полным неудачником, почувствуете себя непригодным, никчёмным." } },
                { (int)CognitiveErrors.Overgenersalization, new CognitiveErrorModel { Key = (int)CognitiveErrors.Overgenersalization, 
                    Title = "Сверхобобщение", 
                    Description = "Когда вы обобщаете что-то сверх меры — вы делавете ошибочный вывод, что событие, которое произошло с вами один раз, будет повторяться снова и снова. Поскольку событие неприятное, вы расстраиваетесь." } },
                { (int)CognitiveErrors.NegativeFilter, new CognitiveErrorModel { Key = (int)CognitiveErrors.NegativeFilter, 
                    Title = "Негативный фильтр", 
                    Description = "Находясь в определённой ситуации, вы выбираете негативную деталь и фиксируетесь исключительно на ней, таким образом негативно воспринимая всю ситуацию в целом. Техническое название этого процесса — \"селективное абстрагирование\". Эта дурная привычка подвергает вас излишним мучениям." } },
                { (int)CognitiveErrors.DepreciationOfPositive, new CognitiveErrorModel { Key = (int)CognitiveErrors.DepreciationOfPositive, 
                    Title = "Обесценивание положительного", 
                    Description = "Это одно из самых разрушительных когнитивных искажений. Вы похожи на учёного, который хочет найти доказательства в поддержку излюбленной гипотезы. Гипотеза, которая доминирует при депрессивном мыщлении, обычно является вариацией на тему \"я второсортный\". Всякий раз, когда вы получаете негативный опыт, то фиксируетесь на нём и делаете вывод: \"Это доказывает то, что я знал всё это время\". Напротив, когда вы встречаетесь с положительным опытом, то говорите себе: \"Это была случайность, это не в счёт\". Цена, которую вы платите за такую привычку, — это глубокое ощущение несчастья и неспособность оценить всё хорошее, что с вами происходит." } },
                { (int)CognitiveErrors.HastyCobnclusions, new CognitiveErrorModel { Key = (int)CognitiveErrors.HastyCobnclusions, 
                    Title = "Поспешные выводы", 
                    Description = "В беспокоящей ситуации вы склонны приходить к негативным выводам, не подкреплённым фактами. Есть две разновидности этой ошибки — \"чтение мыслей\" и \"ошибка предсказания\". Чтение мыслей: вы делаете предположение, что другие люди смотрят на вас свысока, и настолько убеждены в этом, что даже не пытаетесь проверить это предположение. Ошибка предсказания: вы предполагаете, что произойдет что-то плохое, и принимаете это предсказание как факт, хотя оно не соответствует реальности." } },
                { (int)CognitiveErrors.ExaggerationOrСatastrophization, new CognitiveErrorModel { Key = (int)CognitiveErrors.ExaggerationOrСatastrophization, 
                    Title = "Преувеличение", 
                    Description = "Обычно проявляется, когда вы смотрите на свои ошибки, страхи или недостатки. Также называется \"катастрофизация\", потому что банальные негативные события превращаются в кошмарных монстров." } },
                { (int)CognitiveErrors.EmotionalJustification, new CognitiveErrorModel { Key = (int)CognitiveErrors.EmotionalJustification, 
                    Title = "Эмоциональное  обоснование", 
                    Description = "Вы принимаете свои эмоции как доводы в пользу некой истины. Такая логика гласит: \"Я чувствую себя дураком — значит, я дурак\". Еще примеры: \"Я чувствую себя виноватым — должно быть, я сделал что-то плохое.\" " } },
                { (int)CognitiveErrors.StatementWithMustWord, new CognitiveErrorModel { Key = (int)CognitiveErrors.StatementWithMustWord, 
                    Title = "Утверждения со словом \"должен\"", 
                    Description = "Вы пытаетесь замотивировать себя, говоря: \"Я должен сделать это\". Эти заявления вызывают ощущение принуждения и обиды. Как ни парадоксально, в итоге вы чувствуете апатию и отсутствие мотивации." } },
                { (int)CognitiveErrors.HangingShortcuts, new CognitiveErrorModel { Key = (int)CognitiveErrors.HangingShortcuts, 
                    Title = "Навешивание ярлыков", 
                    Description = "Это крайняя форма сверхобобщения. Вместо того, чтобы описать свою ошибку, вы вешаете на себя ярлык: \"Я неудачник\". Когда чьё-то еще поведение вас не устраивает, вы прикрепляете ярлык и к этому человеку: \"Какой же он подлец\". Ошибочные ярлыки, кроме этого, описывают событие языком, имеющим яркую эмоциональную окраску." } },
                { (int)CognitiveErrors.Personalization,     new CognitiveErrorModel { Key = (int)CognitiveErrors.Personalization, 
                    Title = "Персонализация", 
                    Description = "Это искажение рождает чувство вины. Вы берете на себя ответственность за всё негативное, даже если для этого нет оснований. По необъяснимым причинам вы делаете заключение, что произошедшее лежит на вашей совести или указывает на ваш недостаток, даже если в этом не было вашей ответственности." } },
                { (int)CognitiveErrors.Understatement, new CognitiveErrorModel { Key = (int)CognitiveErrors.Understatement, 
                    Title = "Преуменьшение", 
                    Description = "Когда вы думаете о своих сильных сторонах, то можете делать противоположное преувеличению — смотреть так, что вещи выглядят маленькими и несущественными." } },
            };
        }

        #endregion


        #region GetAllEmotions

        public async Task<Dictionary<int, string>> GetAllEmotions()
        {
            using var dataContext = new CBTDataContext();
            var emotions = await dataContext.Set<Emotion>()
                .AsNoTracking().ToListAsync();

            return emotions.ToDictionary(x => x.Id, x => x.Name);

            // TODO: sort by frequency
        }

        #endregion


        #region GetAllThoughts

        public async Task<List<ThreeColumnsTechniqueRecordModel>> GetAllThoughts(string? userId = null)
        {
            using var dataContext = new CBTDataContext();
            var threeColumnsTechniques = await dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.ThreeColumnsTechnique)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return threeColumnsTechniques
                .OrderBy(CalculateOrderOfThoughts)
                .Select(new ThreeColumnsTechniqueRecordModel().Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        private static int CalculateOrderOfThoughts(AutomaticThought item)
        {
            var orderAddition = 0;
            if (!item.CognitiveErrors.Any())
            {
                orderAddition += -2_000_000;
            }
            if (string.IsNullOrEmpty(item.RationalAnswer))
            {
                orderAddition += -1_000_000;
            }

            return orderAddition + item.Id;
        }

        #endregion


        #region AddThought

        public async Task<int> AddThought(string thought)
        {
            using var dataContext = new CBTDataContext();
            var data = new AutomaticThought
            {
                PatientId = 1,
                Thought = thought,
                RationalAnswer = null,
                Type = DiaryType.ThreeColumnsTechnique,
                CognitiveErrors = new List<ThoughtCognitiveError>()
            };

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region AddThoughtFull

        public async Task<int> AddThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            using var dataContext = new CBTDataContext();
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique);

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetThought

        public async Task<ThreeColumnsTechniqueRecordModel?> GetThought(int id)
        {
            using var dataContext = new CBTDataContext();
            return new ThreeColumnsTechniqueRecordModel().Convert(await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditThoughtFull

        public async Task EditThoughtFull(ThreeColumnsTechniqueRecordModel model, string? userId)
        {
            using var dataContext = new CBTDataContext();
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .FirstAsync(x => x.Id == model.Id);

            model.ConvertBack(patient.Id, DiaryType.ThreeColumnsTechnique, data);

            await dataContext.SaveChangesAsync();
        }

        #endregion


        #region DeleteThought

        public async Task DeleteThought(int id)
        {
            using var dataContext = new CBTDataContext();
            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .FirstAsync(x => x.Id == id);

            dataContext.Set<AutomaticThought>()
                .Remove(data);

            await dataContext.SaveChangesAsync();
        }

        #endregion


        #region SendThoughtToPsychologist

        public async Task SendThoughtToPsychologist(int id)
        {
            using var dataContext = new CBTDataContext();
            var data = await dataContext.Set<AutomaticThought>()
                .FirstAsync(x => x.Id == id);

            data.Sent = true;

            await dataContext.SaveChangesAsync();
        }

        #endregion




        #region GetPsychologistReview

        public async Task<RecordReviewModel?> GetPsychologistReview(int id)
        {
            using var dataContext = new CBTDataContext();
            var data = await dataContext.Set<ThoughtPsychologistReview>()
                .Include(x => x.Thought).ThenInclude(x => x.CognitiveErrors)
                .FirstOrDefaultAsync(x => x.ThoughtId == id && x.Thought.Sent && x.Thought.SentBack);

            return data == null ? null : new()
            {
                Id = data.ThoughtId,
                RationalAnswerComment = data.RationalAnswerComment,
                ReviewedErrors = data.Thought.CognitiveErrors
                    .Where(x => x.IsReview && x.PsychologistId == data.PsychologistId)
                    .Select(x => x.CognitiveErrorId)
                    .ToList(),
            };
        }

        #endregion


        #region GetAllAutomaticThoughts

        public async Task<List<AutomaticThoughtDiaryRecordModel>> GetAllAutomaticThoughts(string? userId = null)
        {
            using var dataContext = new CBTDataContext();
            var threeColumnsTechniques = await dataContext.Set<AutomaticThought>()
                                .Include(x => x.CognitiveErrors)
                                .Include(x => x.Emotions)
                                .AsNoTracking()
                                .Where(x => x.Type == DiaryType.AutomaticThoughtDiary)
                                .Where(x => x.Patient.UserId == (userId ?? DemoUserId))
                                .ToListAsync();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            return threeColumnsTechniques
                .OrderBy(CalculateOrderOfThoughts)
                .Select(new AutomaticThoughtDiaryRecordModel().Convert)
                .ToList();
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }

        #endregion


        #region AddAutomaticThoughtFull

        public async Task<int> AddAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            using var dataContext = new CBTDataContext();
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary);

            await dataContext
                .Set<AutomaticThought>()
                .AddAsync(data);
            await dataContext.SaveChangesAsync();

            return data.Id;
        }

        #endregion


        #region GetAutomaticThought

        public async Task<AutomaticThoughtDiaryRecordModel?> GetAutomaticThought(int id)
        {
            using var dataContext = new CBTDataContext();
            return new AutomaticThoughtDiaryRecordModel().Convert(await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors)
                .Include(x => x.Emotions)
                .AsNoTracking()
                .FirstAsync(x => x.Id == id));
        }

        #endregion


        #region EditAutomaticThoughtFull

        public async Task EditAutomaticThoughtFull(AutomaticThoughtDiaryRecordModel model, string? userId)
        {
            using var dataContext = new CBTDataContext();
            var patient = await dataContext.Set<Patient>().FirstAsync(x => x.UserId == (userId ?? DemoUserId));

            var data = await dataContext.Set<AutomaticThought>()
                .Include(x => x.CognitiveErrors).Include(x => x.Emotions)
                .FirstAsync(x => x.Id == model.Id);

            model.ConvertBack(patient.Id, DiaryType.AutomaticThoughtDiary, data);

            await dataContext.SaveChangesAsync();
        }

        #endregion
    }
}
