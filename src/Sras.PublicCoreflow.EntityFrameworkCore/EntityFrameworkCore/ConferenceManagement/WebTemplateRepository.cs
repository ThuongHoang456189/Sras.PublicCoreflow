using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using static Volo.Abp.Identity.IdentityPermissions;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class WebTemplateRepository : EfCoreRepository<PublicCoreflowDbContext, WebTemplate, Guid>, IWebTemplateRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        public WebTemplateRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<IEnumerable<object>> GetListWebTemplateName()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.WebTemplates.ToList().Select(w =>
            new {
                id = w.Id,
                name = w.RootFilePath.Split('/').Last()
            });
        }

        public async void CreateTemplate(Guid webTemplateId, string rootFilePath)
        {
            var dbContext = await GetDbContextAsync();
            WebTemplate webTemplate = new WebTemplate(webTemplateId, rootFilePath);
            dbContext.WebTemplates.Add(webTemplate);
            dbContext.SaveChanges();
        }

    }
}
