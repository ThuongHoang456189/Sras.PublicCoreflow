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

        public async Task<IEnumerable<TemplateResponseDTO>> GetListWebTemplateName()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.WebTemplates.ToList().Select(w =>
            new TemplateResponseDTO()
            {
                Id = w.Id,
                FileName = w.RootFilePath.Split('/').Last(),
                FilePath = w.RootFilePath,
                Name = w.Name,
                Description = w.Description
            });
        }

        public void CreateTemplate(Guid webTemplateId, string name, string description, string rootFilePath)
        {
            var dbContext = GetDbContextAsync().Result;

            // Modified this
            WebTemplate webTemplate = new WebTemplate(webTemplateId, name, description, null, rootFilePath);
            dbContext.WebTemplates.Add(webTemplate);
            dbContext.SaveChanges();
        }

        public TemplateResponseDTO GetTemplateById(Guid id)
        {
            var dbContext = GetDbContextAsync().Result;
            if (dbContext.WebTemplates.Any(w => w.Id == id))
            {
                var result = dbContext.WebTemplates.Find(id);
                return new TemplateResponseDTO()
                {
                    Id = id,
                    Name = result.Name,
                    FileName = result.RootFilePath.Split("/").Last(),
                    FilePath = result.RootFilePath,
                    Description = result.Description
                };
            }
            else
            {
                throw new Exception("TemplateId not eixsting");
            }
        }

        public async Task<IEnumerable<string>> GetConferenceUsedByTemplateId(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            var conference = dbContext.Conferences;
            var conNames = dbContext.Websites.Where(w => w.WebTemplateId == id).Select(w => conference.Where(c => c.Id == w.Id).First().FullName).ToList();
            return conNames;
        }

    }
}
