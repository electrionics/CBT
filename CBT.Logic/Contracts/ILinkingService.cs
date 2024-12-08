using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface ILinkingService
    {
        Task<Link?> GetByUserId(string userId);
        Task<Link?> GetByPublicId(string publicId);
        Task<Link> CreateNewLink(string userId);
    }
}
