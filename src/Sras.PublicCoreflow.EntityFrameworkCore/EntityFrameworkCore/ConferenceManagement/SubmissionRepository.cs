using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class SubmissionRepository : EfCoreRepository<PublicCoreflowDbContext, Outsider, Guid>, ISubmissionRepository
    {
        private readonly IGuidGenerator _guidGenerator;

        public SubmissionRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetNumberOfSubmission(Guid trackId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Submissions.Any(s => s.TrackId == trackId))
                {
                    throw new Exception("There Is no Submission for TrackID=" + trackId);
                }
                var totalSubmission = dbContext.Submissions.Where(s => s.TrackId == trackId).Count();
                return new
                {
                    numOfSubmission = totalSubmission
                };
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][GetNumberOfSubmission] " + ex.Message, ex);
            }
        }
    }
}
