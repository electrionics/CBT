using CBT.Web.Blazor.Data;
using CBT.Web.Blazor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Services
{
    public class PeopleService
    {
        private readonly CBTDataContext dataContext;

        public PeopleService(CBTDataContext dataContext) 
        {
            this.dataContext = dataContext;
        }

        public async Task<Psychologist> GetPsychoilogist(string userId)
        {
            return await dataContext.Set<Psychologist>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);
        }

        public async Task<Patient> GetPatient(string userId)
        {
            return await dataContext.Set<Patient>().AsNoTracking()
                .FirstAsync(x => x.UserId == userId);
        }
    }
}
