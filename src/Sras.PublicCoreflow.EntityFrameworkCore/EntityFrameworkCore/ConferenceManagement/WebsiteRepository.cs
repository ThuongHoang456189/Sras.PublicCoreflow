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
    public class WebsiteRepository : EfCoreRepository<PublicCoreflowDbContext, Website, Guid>, IWebsiteRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly string TEMP_FOLDER_NAME = "temp";
        private readonly string FINAL_FOLDER_NAME = "final";

        public WebsiteRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> getNavbarByConferenceId(Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.Conferences.Any(c => c.Id == conferenceId)) throw new Exception("conferenceId not existing");
            if (!dbContext.Websites.Any(c => c.Id == conferenceId))
            {
                throw new Exception("Website doesn't have record with conferenceId yet");
            }
            var website = dbContext.Websites.FindAsync(conferenceId).Result;
            var navbarJson = website.NavBar;
            if (navbarJson != null)
            {
                return new
                {
                    conferenceName = dbContext.Conferences.FindAsync(conferenceId).Result.FullName,
                    navbar = JsonSerializer.Deserialize<NavbarDTO>(navbarJson).navbar
                };
            } else
            {
                NavbarDTO navbar = new NavbarDTO()
                {
                    navbar = new List<ParentNavbarDTO> { }
                };
                website.NavBar = JsonSerializer.Serialize<NavbarDTO>(navbar);
                await dbContext.SaveChangesAsync();
                return new
                {
                    conferenceName = dbContext.Conferences.FindAsync(conferenceId).Result.FullName,
                    navbar.navbar
                };
            }
        }

        public async Task<object> CreateWebtemplate(String rootFilePath)
        {
            var dbContext = await GetDbContextAsync();
            var webtemplateId = _guidGenerator.Create();

            // Modified this
            // await dbContext.WebTemplates.AddAsync(new WebTemplate(webtemplateId, rootFilePath));
            await dbContext.SaveChangesAsync();
            return await dbContext.WebTemplates.FindAsync(webtemplateId);
        }

        public async Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.WebTemplates.Any(w => w.Id == webtemplateId)) throw new Exception("webtemplateId not exist");
            var navbar = new NavbarDTO()
            {
                navbar = new List<ParentNavbarDTO> { }
            };
            var navbarString = JsonSerializer.Serialize<NavbarDTO>(navbar);
            var newWebsite = new Website(conferenceId, navbarString, null, null, null, webtemplateId);
            var template = await dbContext.WebTemplates.FindAsync(webtemplateId);
            newWebsite.WebTemplate = template;
            await dbContext.Websites.AddAsync(newWebsite);
            template.Websites.Add(newWebsite);
            
            await dbContext.SaveChangesAsync();
            return dbContext.Websites.Where(w => w.Id == conferenceId).Select(w => new
            {
                websiteId = w.Id,
                websiteNavbar = w.NavBar,
                webTemplateId = w.WebTemplateId
            }).First();
        }

        public async Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO)
        {
            
            var dbContext = await GetDbContextAsync();
            if (!dbContext.Conferences.Any(w => w.Id == conferenceId)) throw new Exception("ConferenceId is not existing");
            var navbarString = JsonSerializer.Serialize<NavbarDTO>(navbarDTO);
            if (dbContext.Websites.Any(w => w.Id == conferenceId))
            {
                var oldWebsite = dbContext.Websites.FindAsync(conferenceId).Result;
                oldWebsite.NavBar = navbarString;
            } else
            {
                var newWebsite = new Website(conferenceId, navbarString, null, null, null, webTemplateId);
                if (!dbContext.WebTemplates.Any(wt => wt.Id == webTemplateId)) throw new Exception("WebTemplateId is not existing");
                var template = await dbContext.WebTemplates.FindAsync(webTemplateId);
                newWebsite.WebTemplate = template;
                await dbContext.Websites.AddAsync(newWebsite);
                template.Websites.Add(newWebsite);
            }

            await dbContext.SaveChangesAsync();
            var result = dbContext.Websites.Where(w => w.Id == conferenceId).First();
            return new
            {
                Id = result.Id,
                navbar = JsonSerializer.Deserialize<NavbarDTO>(result.NavBar).navbar
            };
        }

        public async void AddContentToWebsite(Guid webId, string fileName)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.Websites.Any(w => w.Id == webId)) throw new Exception("websiteId is not existing");
            var website = dbContext.Websites.FindAsync(webId).Result;
            // add fileName in pages
            if (website.Pages == null)
            {
                website.Pages = fileName;
            } else
            {
                if (!website.Pages.Split(';').Contains(fileName)) website.Pages = website.Pages + ";" + fileName;
            }
            // check if tempPath null, add tempPath is "{webId}/temp"
            if (website.TempFilePath == null)
            {
                website.TempFilePath = webId + "/" + TEMP_FOLDER_NAME;
            }
            // check if finalPath null, add finalPath is "{webId}/final"
            if (website.RootFilePath == null)
            {
                website.RootFilePath = webId + "/" + FINAL_FOLDER_NAME;
            }
            
            dbContext.SaveChanges();
            if (!dbContext.Websites.Find(webId).Pages.Contains(fileName))
            {
                throw new Exception("Save content path to DB fail");
            }
        }

        public async Task<IEnumerable<object>> GetAllWebsite()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Websites.ToList();
        }
  
        public async Task<IEnumerable<string>> GetAllPageNameOfWebsite(Guid webId)
        {
            var dbContext = await GetDbContextAsync();
            if (dbContext.Websites.Any(w => w.Id == webId))
            {
                return dbContext.Websites.FindAsync(webId).Result.Pages.Split(";").ToList();
            } else
            {
                throw new Exception("WebId is not existing");
            }
        }

    }
}
