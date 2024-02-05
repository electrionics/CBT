using CBT.Web.Blazor.Data.Entities;

namespace CBT.Web.Blazor.Data.Model
{
    public interface IThoughtRecordModel<T>
        where T: new()
    {
        int Id { get; set; }

        string Thought { get; set; }
        List<int> Errors { get; set; }
        string? RationalAnswer { get; set; }


        T? Convert(AuthomaticThoughtDiaryRecord data);
        AuthomaticThoughtDiaryRecord ConvertBack(int patientId, AuthomaticThoughtDiaryRecord? data = null);
    }
}
