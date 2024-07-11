using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CBT.Domain
{
    public class CBTDataContextMARS : CBTDataContext
    {
        public CBTDataContextMARS(DbContextOptions<CBTDataContextMARS> options, IHttpContextAccessor contextAccessor) : base(options, contextAccessor)
        {

        }

        public CBTDataContextMARS(string connectionString):base(connectionString)
        {

        }
    }
}
