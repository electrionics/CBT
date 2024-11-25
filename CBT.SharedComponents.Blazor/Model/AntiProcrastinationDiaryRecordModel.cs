using CBT.Domain.Entities;

namespace CBT.SharedComponents.Blazor.Model
{
    public class AntiProcrastinationDiaryRecordModel
    {
        public int Id { get; set; }

        public string Task { get; set; }

        public int? SupposedEffort { get; set; } // warning if null, save anyway

        public int? SupposedPleasure { get; set; } // warning if null, save anyway

        public int? ActualEffort {  get; set; }

        public int? ActualPleasure { get; set; }


        public DateTime? DateTimeDone { get; set; }

        public DateTime? PlanDate { get; set; } // if null - it's not planned


        #region Convert

        public static AntiProcrastinationDiaryRecordModel? Convert(AntiProcrastinationRecord data)
        {
            if (data == null)
                return null;

            return new AntiProcrastinationDiaryRecordModel()
            {
                Id = data.Id,
                Task = data.Task,

                SupposedEffort = data.SupposedEffort,
                SupposedPleasure = data.SupposedPleasure,

                ActualEffort = data.ActualEffort,
                ActualPleasure = data.ActualPleasure,

                PlanDate = data.PlanDate,
                DateTimeDone = data.DateTimeDone
            };
        }

        #endregion


        #region ConvertBack

        public AntiProcrastinationRecord ConvertBack(int patientId, AntiProcrastinationRecord? data = null)
        {
            var model = this;

            data ??= new AntiProcrastinationRecord();

            data.PatientId = patientId;

            data.Id = model.Id;
            data.Task = model.Task;

            data.SupposedEffort = model.SupposedEffort;
            data.SupposedPleasure = model.SupposedPleasure;

            data.ActualEffort = model.ActualEffort;
            data.ActualPleasure = model.ActualPleasure;

            data.PlanDate = model.PlanDate;
            data.DateTimeDone = model.DateTimeDone;

            return data;
        }

        #endregion
    }
}
