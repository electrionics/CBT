using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Domain.Entities.Enums;
using CBT.SharedComponents.Blazor.Model;

namespace CBT.SharedComponents.Blazor.Services
{
    public class CognitiveErrorsFacade(CBTDataContext dataContext)
    {
        private readonly CBTDataContext _dataContext = dataContext;

        private const string DemoUserId = "DemoClient";

        #region GetAllCognitiveErrors

#pragma warning disable CA1822 // Mark members as static
        public Dictionary<int, CognitiveErrorModel> GetAllCognitiveErrors() => new()
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
#pragma warning restore CA1822 // Mark members as static

        #endregion

        #region GetErrorsReport

        public async Task<List<CognitiveErrorReportItem>> GetErrorsReport(string? userId = null)
        {
            var allErrors = GetAllCognitiveErrors();

            var userErrors = await _dataContext.Set<ThoughtCognitiveError>()
                .AsNoTracking()
                .Where(x => 
                    x.Thought.Patient.UserId == (userId ?? DemoUserId) &&
                    x.PsychologistId == null)
                .GroupBy(x => x.CognitiveErrorId)
                .ToDictionaryAsync(x => x.Key, x => x.Count());

            var reviewUserErrors = (await _dataContext.Set<ThoughtCognitiveError>()
                .AsNoTracking()
                .Where(x => 
                    x.Thought.Patient.UserId == (userId ?? DemoUserId) && 
                    x.PsychologistId != null) // is review
                .ToListAsync())
                    .GroupBy(x => new { x.ThoughtId, x.CognitiveErrorId })
                    .Select(x => x.First())
                    .GroupBy(x => x.CognitiveErrorId)
                    .ToDictionary(x => x.Key, x => x.Count());

            return allErrors.Select(x => new CognitiveErrorReportItem
            {
                Name = x.Value.Title,
                UserCount = userErrors.GetValueOrDefault(x.Key),
                ReviewCount = reviewUserErrors.GetValueOrDefault(x.Key)
            }).ToList();
        }

        #endregion
    }
}