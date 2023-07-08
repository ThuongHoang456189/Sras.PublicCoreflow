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
    public class CameraReadyRepository : EfCoreRepository<PublicCoreflowDbContext, CameraReady, Guid>, ICameraReadyRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        public CameraReadyRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<CameraReady> GetCameraReadyById(Guid id)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.CameraReadies.Any(c => c.Id == id)) throw new Exception("CameraReadyId is not existing");
            return dbContext.CameraReadies.Where(c => c.Id == id).First();
        }

        public async void UpdateRootFilePath(Guid id, string path)
        {
            var dbContext = await GetDbContextAsync();
            if (!dbContext.CameraReadies.Any(c => c.Id == id)) throw new Exception("CameraReadyId is not existing");
            var cam = dbContext.CameraReadies.Where(c => c.Id == id).First();
            cam.RootCameraReadyFilePath = path;
            dbContext.SaveChanges();
        }

    }
}
