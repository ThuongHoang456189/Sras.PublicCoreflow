using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class PaperStatusRepository : EfCoreRepository<PublicCoreflowDbContext, Outsider, Guid>,  IPaperStatusRepository
    {

        private readonly IGuidGenerator _guidGenerator;
        public PaperStatusRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<IEnumerable<object>> GetAllPaperStatus(Guid? conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            IEnumerable<PaperStatus> paperStatusList = await dbContext.PaperStatuses.ToListAsync();
            if (conferenceId != null) paperStatusList = paperStatusList.Where(ps => ps.ConferenceId == conferenceId || ps.ConferenceId == null);
            if (conferenceId == null) paperStatusList = paperStatusList.Where(ps => ps.ConferenceId == null);
            var result = paperStatusList.Select(p => new
            {
                statusId = p.Id,
                statusName = p.Name
            }).ToList();

            return result;
        }

        public async Task<IEnumerable<PaperStatus>> GetPaperStatusesAllField(Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            IEnumerable<PaperStatus> paperStatusList = await dbContext.PaperStatuses.ToListAsync();
            paperStatusList = paperStatusList.Where(ps => ps.ConferenceId == conferenceId || ps.ConferenceId == null).ToList();

            return paperStatusList;
        }

        public async Task<object> CreatePaperStatus(PaperStatusCreateRequest createRequest)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var paperStatusId = _guidGenerator.Create();
                if (createRequest.ConferenceId != null && !dbContext.Conferences.Any(x => x.Id == createRequest.ConferenceId))
                {
                    throw new Exception($"ConferenceId {createRequest.ConferenceId} not existing");
                }

                var newPaperStatus = new PaperStatus(paperStatusId, createRequest.Text, createRequest.ConferenceId, createRequest.VisibleToAuthor, createRequest.ConferenceId == null);
                await dbContext.PaperStatuses.AddAsync(newPaperStatus);
                await dbContext.SaveChangesAsync();

                var paperStatus = await dbContext.PaperStatuses.FirstAsync(x => x.Id == paperStatusId);
                return new
                {
                    id = paperStatus.Id,
                    text = paperStatus.Name,
                    visibleToAuthor = paperStatus.ReviewsVisibleToAuthor,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
