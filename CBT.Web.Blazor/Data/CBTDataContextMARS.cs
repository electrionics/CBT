﻿using Microsoft.EntityFrameworkCore;

namespace CBT.Web.Blazor.Data
{
    public class CBTDataContextMARS : CBTDataContext
    {
        public CBTDataContextMARS(DbContextOptions<CBTDataContextMARS> options, IHttpContextAccessor contextAccessor) : base(options, contextAccessor)
        {

        }
    }
}
