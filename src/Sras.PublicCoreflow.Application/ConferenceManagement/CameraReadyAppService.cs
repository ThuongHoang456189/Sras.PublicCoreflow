using AutoMapper.Internal.Mappers;
using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class CameraReadyAppService : PublicCoreflowAppService, ICameraReadyAppService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IBlobContainer<CameraReadyContainer> _cameraReadyContainer;
        private readonly ICameraReadyRepository _cameraReadyRepository;

        public CameraReadyAppService(IGuidGenerator guidGenerator, IBlobContainer<CameraReadyContainer> cameraReadyContainer, ICameraReadyRepository cameraReadyRepository)
        {
            _guidGenerator = guidGenerator;
            _cameraReadyContainer = cameraReadyContainer;
            _cameraReadyRepository = cameraReadyRepository;
        }

        private void CreateCameraReadyFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
             _cameraReadyContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        public async Task<byte[]> GetCameraReadyFiles(string rootFilePath)
        {
            return await _cameraReadyContainer.GetAllBytesOrNullAsync(rootFilePath);
        }

        //public ResponseDto CreateWebTemplateFiles(string filePath, RemoteStreamContent file)
        //{
        //    ResponseDto response = new();

        //    // Check valid submission

        //    try
        //    {
        //        // Assume that the file extension is exactly matched its file name extension
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            CreateTemplateFilesAsync(filePath, file, true);
        //        }

        //        response.IsSuccess = true;
        //        response.Message = "Create webTemplate files successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = "Exception";
        //        throw new Exception("Error to save file");
        //    }

        //    return response;
        //}

        //public object CreateTemplate(string name, string description, NavbarDTO navbarDTO)
        //{
        //    var webTemplateId = _guidGenerator.Create();
        //    var filePath = ORIGINAL_TEMPLATE_ROOT_FILE_PATH;
        //    try
        //    {
        //        _websiteRepository.CreateTemplate(webTemplateId, name, description, filePath, navbarDTO);
        //        //CreateWebTemplateFiles(filePath, file);
        //        var result = _websiteRepository.GetTemplateById(webTemplateId);
        //        return new
        //        {
        //            id = result.Id,
        //            name = result.Name,
        //            description = result.Description,
        //            conferenceHasUsed = result.conferenceHasUsed,
        //            navbar = result.Navbar.navbar
        //        };
        //    } catch (Exception ex)
        //    {
        //        throw new Exception("Create Template error: " + ex.Message);
        //    }
        //}




        //// continue code here

        public async Task<FileDTO> downloadOneCameraReadyFile(Guid camId)
        {
            //var rootFilePath = _cameraReadyRepository.GetCameraReadyById(camId).GetAwaiter().GetResult().RootCameraReadyFilePath;
            var rootFilePath = "3fa85f64-5717-4562-b3fc-2c963f66afa6/haha.html";
            var fileName = rootFilePath.Split("/").Last();
            return new FileDTO()
            {
                file = GetCameraReadyFiles(rootFilePath).Result,
                fileName = fileName
            };
        }
    }
}
