using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("WebTemplate")]
    [Route("api/sras/web-templates")]
    public class WebTemplateController : AbpController
    {

        private readonly IWebTemplateAppService _webTemplateAppService;

        public WebTemplateController(IWebTemplateAppService webTemplateAppService)
        {
            _webTemplateAppService = webTemplateAppService;
        }

        //[HttpPost("web-template")]
        //public async Task<object> CreateWebTemplate([FromBody]string rootFilePath)
        //{
        //    try
        //    {
        //        var result = await _websiteAppService.CreateWebtemplate(rootFilePath);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        [HttpGet("{hasContent}")]
        public async Task<ActionResult<object>> GetNavbarOfWebsite(bool hasContent)
        {
            try
            {
                var result = await _webTemplateAppService.GetListWebTemplateName(hasContent);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetTemplates(string? websiteId)
        {
            try
            {
                var result = _webTemplateAppService.GetListTemplate(websiteId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-navbar-template/{templateId}")]
        public async Task<ActionResult<object>> UpdateTemplate(Guid templateId, [FromBody] TemplateCreateRequestDTO dto)
        {
            try
            {
                var result = await _webTemplateAppService.UpdateTemplate(templateId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //public async Task<ActionResult<ResponseDto>> CreateWebTemplate(string name, string description, List<RemoteStreamContent> file)
        //{
        //    var result = await _webTemplateAppService.CreateTemplate(file.First(), name, description);
        //    return Ok(result);
        //}
        //[HttpPost("web-template-files")]
        //public async Task<ActionResult<object>> CreateWebTemplate(string name, string description, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file is selected.");
        //    }
        //    var fileName = file.FileName;

        //    try
        //    {
        //        using (var stream = new MemoryStream())
        //        {
        //            await file.CopyToAsync(stream);
        //            stream.Position = 0;
        //            var remoteStreamContent = new RemoteStreamContent(stream);

        //            //var result = _webTemplateAppService.CreateTemplate(remoteStreamContent, name.Trim(), description.Trim(), fileName);
        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Error in uploading and creating the web template: " + ex.Message);
        //    }
        //}

        [HttpGet("download-all-templates")]
        public async Task<ActionResult> downloadAllTemplates()
        {
            IEnumerable<byte[]> listBytes = await _webTemplateAppService.downloadAllTemplates();
            using (var ms = new MemoryStream())
            {
                using (var archive =
                new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, true))
                {

                    var zipEntry = (ZipArchiveEntry)null;
                    foreach (var (item, index) in listBytes.Select((value, i) => (value, i)))
                    {
                        zipEntry = archive.CreateEntry("template" + index + ".html", CompressionLevel.Fastest);
                        using (var zipStream = zipEntry.Open())
                        {
                            zipStream.Write(item, 0, item.Length);
                        }
                    }

                    //var zipEntry = archive.CreateEntry("image1.png", CompressionLevel.Fastest);
                    //using (var zipStream = zipEntry.Open())
                    //{
                    //    zipStream.Write(bytes1, 0, bytes1.Length);
                    //}

                    //var zipEntry2 = archive.CreateEntry("image2.png", CompressionLevel.Fastest);
                    //using (var zipStream = zipEntry2.Open())
                    //{
                    //    zipStream.Write(bytes2, 0, bytes2.Length);
                    //}
                }
                return File(ms.ToArray(), "application/zip", "Template.zip");
            }
        }

        [HttpGet("{templateId}/download-one-template")]
        public async Task<ActionResult<byte[]>> downloadOneTemplate(Guid templateId)
        {
            try
            {
                var fileDTO = await _webTemplateAppService.downloadOneTemplate(templateId);
                var stream = new MemoryStream(fileDTO.file);
                stream.Position = 0;

                return new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = fileDTO.fileName
                };
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("templateFileInfos")]
        public async Task<ActionResult<object>> GetListWebTemplateFileInfo()
        {
            try
            {
                var result = await _webTemplateAppService.GetListWebTemplateFileInfo();
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create-template")]
        public async Task<ActionResult<object>> CreateTempalte(string name, string description, [FromBody] NavbarDTO navbar)
        {
            try
            {
                var result = _webTemplateAppService.CreateTemplate(name, description, navbar);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{templateId}")]
        public async Task<ActionResult<bool>> DeleteWebTemplateById(Guid templateId)
        {
            try
            {
                var result = await _webTemplateAppService.DeleteWebTemplateById(templateId);
                return Ok(result);
            } catch(Exception ex)
            {
                if (ex.Message == "Web Template is using") return Forbid();
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("create-origin-web-template")]
        public async void createOriginTemplate(string name, string description)
        {
            _webTemplateAppService.createOriginTemplate(name, description);
        }

    }
}
