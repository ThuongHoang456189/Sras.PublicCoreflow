using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class ConflictCaseRepository : EfCoreRepository<PublicCoreflowDbContext, ConflictCase, Guid>, IConflictCaseRepository
    {
        public ConflictCaseRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<object>> GetAllConflictCases([Optional] Guid trackId)
        {
            var dbContext = await GetDbContextAsync();

            var result = await dbContext.ConflictCases.Where(x => x.IsDefault || x.TrackId == trackId)
                                                    .Select(c => new
                                                    {
                                                        conflictCaseId = c.Id,
                                                        conflictCaseName = c.Name,
                                                    }).ToListAsync();

            return result;
        }
    }
}
