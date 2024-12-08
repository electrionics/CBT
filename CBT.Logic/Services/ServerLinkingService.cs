using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;
using CBT.Logic.Contracts;

namespace CBT.Logic.Services
{
    public class ServerLinkingService(
        CBTDataContextMARS dataContext):ILinkingService
    {
        private readonly CBTDataContext _dataContext = dataContext;

        public async Task<Link?> GetByUserId(string userId)
        {
            var link = await _dataContext.Set<Link>().FirstOrDefaultAsync(x => x.UserId == userId);

            return link;
        }

        public async Task<Link?> GetByPublicId(string publicId)
        {
            var link = await _dataContext.Set<Link>().FirstOrDefaultAsync(x => x.PublicId == publicId);

            return link;
        }

        public async Task<Link> CreateNewLink(string userId)
        {
            var existingLink = await _dataContext.Set<Link>().FirstOrDefaultAsync(x => x.UserId == userId);
            if (existingLink == null)
            {
                existingLink = new Link() { UserId = userId };
                _dataContext.Set<Link>().Add(existingLink);
            }
            
            existingLink.PublicId = Guid.NewGuid().ToString();
            await _dataContext.SaveChangesAsync();

            return existingLink;
        }
    }
}
