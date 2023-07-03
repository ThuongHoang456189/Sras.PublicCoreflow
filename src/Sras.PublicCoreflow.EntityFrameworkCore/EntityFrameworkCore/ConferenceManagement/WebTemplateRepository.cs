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
            new TemplateResponseDTO() {
                Id = w.Id,
                FileName = w.RootFilePath.Split('/').Last(),
                FilePath = w.RootFilePath,
                Name = w.Name,
                Description = w.Description
            });
        }

        public void CreateTemplate(Guid webTemplateId, string name, string description, string rootFilePath, NavbarDTO defaultNavbar)
        {
            var dbContext = GetDbContextAsync().Result;

            // Modified this
            if (defaultNavbar.navbar == null)
            {
                defaultNavbar = new NavbarDTO()
                {
                    navbar = new List<ParentNavbarDTO>()
                {
                    new ParentNavbarDTO()
                    {
                        parentId = "45883dda-2725-4934-a3f8-60108b1dae61",
                        parentLabel = "Home",
                        href = "home.html",
                        childs = new List<ChildNavbarDTO>(){ }
                    },
                    new ParentNavbarDTO()
                    {
                        parentId = "c8dccf7e-6237-4d07-99b1-d28be9e66cbe",
                        parentLabel = "About",
                        href = "about.html",
                        childs = new List<ChildNavbarDTO>(){ }
                    }
                }
                };
            }
            WebTemplate webTemplate = new WebTemplate(webTemplateId, name, description, JsonSerializer.Serialize<NavbarDTO>(defaultNavbar), rootFilePath);
            dbContext.WebTemplates.Add(webTemplate);
            dbContext.SaveChanges();
        }

        public async Task<object> UpdateTemplate(Guid webTemplateId, NavbarDTO navbarDTO)
        {
            var dbContext = GetDbContextAsync().Result;
            dbContext.WebTemplates.Where(w => w.Id == webTemplateId).First().NavBar = JsonSerializer.Serialize<NavbarDTO>(navbarDTO);
            dbContext.SaveChanges();
            return dbContext.WebTemplates.Where(w => w.Id == webTemplateId).First();
        }

        public TemplateResponseDTO GetTemplateById(Guid id)
        {
            var dbContext = GetDbContextAsync().Result;
            if (dbContext.WebTemplates.Any(w => w.Id == id))
            {
                dbContext.WebTemplates.Include(t => t.Websites).ThenInclude(w => w.Conference);
                var result = dbContext.WebTemplates.Find(id);
                return new TemplateResponseDTO()
                {
                    Id = result.Id,
                    Name = result.Name,
                    conferenceHasUsed = dbContext.Conferences.Where(c => result.Websites.Select(w => w.Id).Contains(c.Id)).Select(c => c.FullName).ToList(),
                    Description = result.Description,
                    Navbar = JsonSerializer.Deserialize<NavbarDTO>(result.NavBar)
                };
            } else
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

        public async Task<IEnumerable<object>> GetListWebTemplate()
        {
            var dbContext = await GetDbContextAsync();
            dbContext.WebTemplates.Include(t => t.Websites).ThenInclude(w => w.Conference);
            var templates = dbContext.WebTemplates.Include(w => w.Websites).ToList().Select(t => new
            {
                id = t.Id,
                name = t.Name,
                conferenceHasUsed = dbContext.Conferences.Where(c => t.Websites.Select(w => w.Id).Contains(c.Id)).Select(c => c.FullName).ToList(),
                description = t.Description,
                navbars = JsonSerializer.Deserialize<NavbarDTO>(t.NavBar).navbar
            });
            
            return templates;
        }

        public async Task<Guid> getTemplateIdByWebId(string websiteId)
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Websites.Where(w => w.Id.ToString() == websiteId).First().WebTemplateId;
        }
    }
}
