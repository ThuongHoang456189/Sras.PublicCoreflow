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

        public async Task<IEnumerable<object>> GetAllPaperStatus(Guid? conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            IEnumerable<PaperStatus> paperStatusList = await dbContext.PaperStatuses.ToListAsync();
            if (conferenceId != null) paperStatusList = paperStatusList.Where(ps => ps.ConferenceId == conferenceId);
            var result = paperStatusList.Select(p => new
            {
                statusId = p.Id,
                statusName = p.Name
            }).ToList();

            return result;
        }
    }
}
