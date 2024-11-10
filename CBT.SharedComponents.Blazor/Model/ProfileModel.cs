namespace CBT.SharedComponents.Blazor.Model
{
    public class ProfileModel
    {
        public string DisplayName { get; set; }

        public string Email { get; set; }

        public string PublicId { get; set; }

        public string UrlForShare {  get; set; }

        public bool IsPatient { get; set; }

        public bool IsPsychologist { get; set; }

        public List<UserLinkingModel> LinkedUsers { get; set; }
    }
}
