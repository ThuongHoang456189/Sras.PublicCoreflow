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

        private void CreateTemplateFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
             _webTemplateBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        public ResponseDto CreateWebTemplateFiles(string filePath, RemoteStreamContent file)
        {
            ResponseDto response = new();

            // Check valid submission

            try
            {
                // Assume that the file extension is exactly matched its file name extension
                if (file != null && file.ContentLength > 0)
                {
                    CreateTemplateFilesAsync(filePath, file, true);
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
        public async Task<byte[]> GetTemplateFiles(string rootFilePath)
        {
            return await _webTemplateBlobContainer.GetAllBytesOrNullAsync(rootFilePath);
        }

        public async Task<object> UpdateTemplate(Guid webTemplateId, NavbarDTO navbarDTO)
        {
            return _websiteRepository.UpdateTemplate(webTemplateId, navbarDTO);
        }

        public object CreateTemplate(RemoteStreamContent file, string name, string description, string fileName)
        {
            var webTemplateId = _guidGenerator.Create();
            var filePath = webTemplateId + "/" + fileName;
            try
            {
                _websiteRepository.CreateTemplate(webTemplateId, name, description, filePath);
                CreateWebTemplateFiles(filePath, file);
                var result = _websiteRepository.GetTemplateById(webTemplateId);
                return new
                {
                    id = result.Id,
                    name = result.Name,
                    fileName = result.FilePath.Split("/").Last(),
                    content = "",
                    description = result.Description,
                    size = (float)Math.Round(GetTemplateFiles(result.FilePath).Result.Length / 1024.0, 2)
                };
            } catch (Exception ex)
            {
                throw new Exception("Create Template error: " + ex.Message);
            }
        }

        public async Task<IEnumerable<object>> GetListWebTemplateName(bool hasContent)
        {
            var listTemplate = await _websiteRepository.GetListWebTemplateName();
            if (hasContent)
            {
                return listTemplate.ToList().Select( item =>
                {
                    var bylesFile =  GetTemplateFiles(item.FilePath).Result;
                    var stringFile = Encoding.Default.GetString(bylesFile);
                    return new
                    {
                        id = item.Id,
                        name = item.Name,
                        fileName = item.FileName,
                        description = item.Description,
                        conferenceUsed = _websiteRepository.GetConferenceUsedByTemplateId(item.Id).Result,
                        content = stringFile
                    };
                });
            } else
            {
                return listTemplate.ToList().Select(item => new
                {
                    id = item.Id,
                    name = item.Name,
                    fileName = item.FileName,
                    description = item.Description,
                    conferenceUsed = new List<string>() { },
                    content = ""
                });
            }
        }

        public async Task<IEnumerable<byte[]>> downloadAllTemplates()
        {
            var listTemplate = await _websiteRepository.GetListWebTemplateName();
            return listTemplate.ToList().Select(item => GetTemplateFiles(item.FilePath).Result);
        }

        public async Task<FileDTO> downloadOneTemplate(Guid templateId)
        {
            var listTemplate = await _websiteRepository.GetListWebTemplateName();
            var file = listTemplate.ToList().Where(t => t.Id == templateId).First();
            var filePath = file.FilePath;
            var fileName = file.FilePath.Split("/").Last();
            return new FileDTO()
            {
                file = GetTemplateFiles(filePath).Result,
                fileName = fileName
            };
        }

        public async Task<IEnumerable<object>> GetListWebTemplateFileInfo()
        {
            var result = await _websiteRepository.GetListWebTemplateName();
            return result.ToList().Select(wt => new
            {
                id = wt.Id,
                name = wt.Name,
                fileName = wt.FileName,
                description = wt.Description,
                size = (float)Math.Round(GetTemplateFiles(wt.FilePath).Result.Length / 1024.0, 2)
            });
        }

        public async Task<object> GetListTemplate(string? websiteId)
        {
            var bylesFile = GetTemplateFiles("00676048-fe73-e4b9-7d38-3a0c152f40a6" + "/" + "original-template.html").Result;
            var stringFile = Encoding.Default.GetString(bylesFile);
            if (websiteId != null)
            {
                return new
                {
                    content = stringFile,
                    selectedTemplate = await _websiteRepository.getTemplateIdByWebId(websiteId),
                    templates = await _websiteRepository.GetListWebTemplate()
                };
            }
            else
            {
                return new
                {
                    content = stringFile,
                    selectedTemplate = (string)null,
                    templates = await _websiteRepository.GetListWebTemplate()
                };

            }
        }
    }
}
