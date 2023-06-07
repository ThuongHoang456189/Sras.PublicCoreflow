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
    }
}
