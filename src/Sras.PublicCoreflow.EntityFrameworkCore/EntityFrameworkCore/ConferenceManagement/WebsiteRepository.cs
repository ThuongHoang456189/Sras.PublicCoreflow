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

        public async Task<object> UpdateNavbarByConferenceId(Guid conferenceId, NavbarDTO navbarDTO)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.Conferences.Any(w => w.Id == conferenceId)) throw new Exception("ConferenceId is not existing");
            if (!dbContext.Websites.Any(w => w.Id == conferenceId)) throw new Exception("Website of conferenceId is not existing");
            var navbarString = JsonSerializer.Serialize<NavbarDTO>(navbarDTO);
            var oldWebsite = dbContext.Websites.FindAsync(conferenceId).Result;
            oldWebsite.NavBar = navbarString;

            await dbContext.SaveChangesAsync();
            var result = dbContext.Websites.Where(w => w.Id == conferenceId).First(); 
            return new
            {
                Id = result.Id,
                navbar = JsonSerializer.Deserialize<NavbarDTO>(result.NavBar).navbar
            };
        }

    }
}
