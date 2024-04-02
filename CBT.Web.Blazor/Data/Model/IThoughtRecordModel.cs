using CBT.Web.Blazor.Data.Entities;
using CBT.Web.Blazor.Data.Entities.Enums;

namespace CBT.Web.Blazor.Data.Model
{
    public interface IThoughtRecordModel<T>
        where T: new()
    {
        int Id { get; set; }

        string Thought { get; set; }
        List<int> Errors { get; set; }
        string? RationalAnswer { get; set; }


        T? Convert(AutomaticThought data);
        AutomaticThought ConvertBack(int patientId, DiaryType type, AutomaticThought? data = null);
    }
}
