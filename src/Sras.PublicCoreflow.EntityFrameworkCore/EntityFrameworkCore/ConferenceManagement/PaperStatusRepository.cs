using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class PaperStatusRepository : EfCoreRepository<PublicCoreflowDbContext, Outsider, Guid>,  IPaperStatusRepository
    {
        public PaperStatusRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<IEnumerable<object>> GetAllPaperStatus()
        {
            var dbContext = await GetDbContextAsync();
            var result = await dbContext.PaperStatuses.Select(p => new
            {
                statusId = p.Id,
                statusName = p.Name
            }).ToListAsync();

            return result;
        }
    }
}
