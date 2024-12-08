using CBT.Domain.Entities;

namespace CBT.SharedComponents.Blazor.Model
{
    public class MoodDiaryRecordModel
    {
        public int Id { get; set; }

        public DateTime? Date { get; set; }

        public TimeOnly? Time { get; set; }

        public string? Events { get; set; }

        public int? Value { get; set; }


        #region Convert

        public static MoodDiaryRecordModel? Convert(MoodRecord data)
        {
            if (data == null)
                return null;

            return new MoodDiaryRecordModel()
            {
                Id = data.Id,

                Value = data.Value,
                Events = data.Events,

                Date = data.DateTime.Date,
                Time = TimeOnly.FromDateTime(data.DateTime),
            };
        }

        #endregion


        #region ConvertBack

        public MoodRecord ConvertBack(int patientId, MoodRecord? data = null)
        {
            var model = this;

            data ??= new MoodRecord();

            data.PatientId = patientId;

            data.Id = model.Id;
            data.DateTime = new DateTime(
                model.Date!.Value.Year,
                model.Date!.Value.Month,
                model.Date!.Value.Day,
                model.Time!.Value.Hour,
                0,
                0);

            data.Value = model.Value!.Value;
            data.Events = model.Events;

            return data;
        }

        #endregion
    }
}
