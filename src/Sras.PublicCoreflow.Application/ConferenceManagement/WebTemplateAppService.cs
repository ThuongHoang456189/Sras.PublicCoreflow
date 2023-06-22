using AutoMapper.Internal.Mappers;
using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class WebTemplateAppService : PublicCoreflowAppService, IWebTemplateAppService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebTemplateRepository _websiteRepository;
        private readonly IBlobContainer<WebTemplateContainer> _webTemplateBlobContainer;

        public WebTemplateAppService(IGuidGenerator guidGenerator, IWebTemplateRepository websiteRepository, IBlobContainer<WebTemplateContainer> webTemplateBlobContainer)
        {
            _guidGenerator = guidGenerator;
            _websiteRepository = websiteRepository;
            _webTemplateBlobContainer = webTemplateBlobContainer;
        }

        private async Task CreateTemplateFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _webTemplateBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        public async Task<ResponseDto> CreateWebTemplateFiles(string filePath, RemoteStreamContent file)
        {
            ResponseDto response = new();

            // Check valid submission

            try
            {
                // Assume that the file extension is exactly matched its file name extension
                if (file != null && file.ContentLength > 0)
                    {
                        await CreateTemplateFilesAsync(filePath, file, true);
                    }

                response.IsSuccess = true;
                response.Message = "Create webTemplate files successfully";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
                throw new Exception("Error to save file");
            }

            return response;
        }
        public async Task<byte[]> GetSubmissionFiles(string rootFilePath)
        {
            return await _webTemplateBlobContainer.GetAllBytesOrNullAsync(rootFilePath);
        }

        public async Task<ResponseDto> CreateTemplate(RemoteStreamContent file)
        {
            var webTemplateId = _guidGenerator.Create();
            var filePath = webTemplateId + "/" + file.FileName;
            var response = new ResponseDto();
            try
            {
                response = await CreateWebTemplateFiles(filePath, file);
                _websiteRepository.CreateTemplate(webTemplateId, filePath);
            } catch (Exception ex)
            {
                return new ResponseDto() { IsSuccess = false, Message = "Error in Upload and save file"};
            }

            return response;
        }

        public async Task<IEnumerable<object>> GetListWebTemplateName()
        {
            return await _websiteRepository.GetListWebTemplateName();
        }

    }
}
