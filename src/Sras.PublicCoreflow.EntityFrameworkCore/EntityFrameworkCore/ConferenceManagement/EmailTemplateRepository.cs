using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Domain.ConferenceManagement;
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
    public class EmailTemplateRepository : EfCoreRepository<PublicCoreflowDbContext, EmailTemplate, Guid>, IEmailTemplateRepository
    {
        public EmailTemplateRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        private readonly IGuidGenerator _guidGenerator;

        public async Task<object> GetEmailTemplateById(Guid id)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.EmailTemplates.Any(et => et.Id == id)) {
                    throw new Exception("EmailTemplateId not Found");
                } else
                {
                    var queryResult = await dbContext.EmailTemplates.FindAsync(id);
                    return new
                    {
                        subject = queryResult.Subject,
                        body = queryResult.Body
                    };
                }
            } catch (Exception ex)
            {
                throw new Exception("[Repository] GetEmailTemplateById : " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetEmailTemplateByConferenceId(Guid conferenceId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Conferences.Any(c => c.Id == conferenceId))
                {
                    throw new Exception("ConferenceId not exist in DB");
                } else if (!dbContext.EmailTemplates.Any(emt => emt.ConferenceId == conferenceId))
                {
                    return new List<string>();
                }

                var result = dbContext.EmailTemplates.Where(e => e.ConferenceId == conferenceId).Select(em => new
                {
                    templateId = em.Id,
                    templateName = "haha"
                });
                return result;
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][GetEmailTemplateByConferenceId] " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetEmailTemplateByTrackId(Guid trackId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var finalTemplateList = new List<object>();
                if (!dbContext.Tracks.Any(c => c.Id == trackId))
                {
                    throw new Exception("TrackId not exist in DB");
                }

                var resultOne = dbContext.EmailTemplates
                    .Where(e => e.ConferenceId != null)
                    .Where(e => e.TrackId == trackId)
                    .ToList();
                var resultTwo = dbContext.EmailTemplates
                    .Where(em => em.ConferenceId != null && em.TrackId == null)
                    .ToList();  
                resultOne.AddRange(resultTwo);
                resultOne.Select(r => new
                {
                    TemplateId = r.Id,
                    TemplateName = "haha"
                });
                return resultOne;
            }
            catch (Exception ex)
            {
                throw new Exception("[ERROR][GetEmailTemplateByConferenceId] " + ex.Message, ex);
            }
        }

    }
}
