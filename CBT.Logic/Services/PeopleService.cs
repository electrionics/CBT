using Microsoft.EntityFrameworkCore;

using CBT.Domain;
using CBT.Domain.Entities;

namespace CBT.Logic.Services
{
    public class PeopleService
    {
        private readonly CBTDataContext dataContext;

        public PeopleService(CBTDataContextMARS dataContext) 
        {
            this.dataContext = dataContext;
        }

        public async Task<Psychologist?> GetPsychoilogist(string userId)
        {
            return await dataContext.Set<Psychologist>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<Patient?> GetPatient(string userId)
        {
            return await dataContext.Set<Patient>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
