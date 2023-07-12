using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Security;
using Polly;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using static Volo.Abp.Identity.IdentityPermissions;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ResearcherProfileRepository : EfCoreRepository<PublicCoreflowDbContext, ResearcherProfile, Guid>, IResearcherProfileRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        public ResearcherProfileRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<bool> hasResearchProfile(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.ResearcherProfiles.Include(r => r.Account).Any(r => r.Account.Id == userId);
        }

        public async Task<bool> isPrimaryEmailDuplicate(Guid userId,  string email)
        {
            var dbContext = await GetDbContextAsync();
            var thisRP = dbContext.ResearcherProfiles.Where(r => r.Account.Id == userId);
            if (dbContext.ResearcherProfiles.ToList().Except(thisRP).Any(r => r.PrimaryEmail == email)) return true;
            return false;
        }
        
        public async Task<bool> isAccountExist(Guid userId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Users.Any(u => u.Id == userId);
        }

    }
}
