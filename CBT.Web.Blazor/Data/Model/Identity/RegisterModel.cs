using CBT.Web.Blazor.Data.Model.Enums;

namespace CBT.Web.Blazor.Data.Model.Identity
{
    public class RegisterModel
    {
        public bool IsClient { get; set; }

        public bool IsPsychologist { get; set; }

        public RoleType[] RoleTypes
        {
            get
            {
                var result = new List<RoleType>();

                if (IsClient)
                    result.Add(RoleType.Client);
                if (IsPsychologist)
                    result.Add(RoleType.Psychologist);

                return result.ToArray();
            }
        }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
