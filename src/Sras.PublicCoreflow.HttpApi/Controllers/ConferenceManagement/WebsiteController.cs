using Microsoft.AspNetCore.Mvc;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.Controllers.ConferenceManagement
{
    [RemoteService(Name = "Sras")]
    [Area("sras")]
    [ControllerName("Website")]
    [Route("api/sras/website")]
    public class WebsiteController : AbpController
    {

        private readonly IWebsiteAppService _websiteAppService;

        public WebsiteController(IWebsiteAppService websiteAppService)
        {
            _websiteAppService = websiteAppService;
        }


        [HttpPost("{conferenceId}/{webTemplateId}")]
        public async Task<object> CreateWebsite(Guid webTemplateId, Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.CreateWebsite(webTemplateId, conferenceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-navbar/{conferenceId}")]
        public async Task<ActionResult<object>> GetNavbarOfWebsite(Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.getNavbarByConferenceId(conferenceId);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{conferenceId}/{webTemplateId}/update-navbar")]
        public async Task<ActionResult<object>> UpdateNavbarOfWebsite(Guid conferenceId, Guid webTemplateId, [FromBody] NavbarDTO navbarDTO)
        {
            try
            {
                var result = await _websiteAppService.UpdateNavbarByConferenceId(conferenceId, webTemplateId, navbarDTO);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add-content-of-website/{conferenceId}/{fileName}")]
        public async Task<ActionResult<object>> AddContentOfWebsite(Guid conferenceId, string fileName, [FromBody] ContentBodyRequest content)
        {
            try
            {
                _websiteAppService.UploadContentOfWebsite(conferenceId, fileName, content.temp, content.finalRuslt);
                return Ok(new ResponseDto()
                {
                    IsSuccess = true
                });
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-content-temp-file/{conferenceId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetContentTempOfWebsite(Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.GetContentTempOfWebsite(conferenceId);
                return Ok(result);
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-content-final-file/{conferenceId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetContentFinalOfWebsite(Guid conferenceId)
        {
            try
            {
                var result = await _websiteAppService.GetContentFinalOfWebsite(conferenceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IEnumerable<object>> GetAllWebsite()
        {
            return await _websiteAppService.GetAllWebsite();
        }

        [HttpGet("download-all-final-file/{conferenceId}")]
        public async Task<ActionResult> downloadAllFinalFiles(Guid conferenceId)
        {
            IEnumerable<FileNameAndByteDTO> listBytes = await _websiteAppService.DownloadAllFinalFile(conferenceId);
            using (var ms = new MemoryStream())
            {
                using (var archive =
                new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, true))
                {

                    var zipEntry = (ZipArchiveEntry)null;
                    foreach (var (item, index) in listBytes.Select((value, i) => (value, i)))
                    {
                        zipEntry = archive.CreateEntry(item.fileName , CompressionLevel.Fastest);
                        using (var zipStream = zipEntry.Open())
                        {
                            zipStream.Write(item.bytes, 0, item.bytes.Length);
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", "Final-Content-Website-" + conferenceId +".zip");
            }
        }

        [HttpGet("delete-final-file/{conferenceId}")]
        public async Task<ActionResult<bool>> deleteContentFile(Guid conferenceId, string idParent, string? idChild)
        {
            try
            {
                var result = await _websiteAppService.DeleteNavbarAndHrefFile(conferenceId, idParent, idChild);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update-pages-website")]
        public async Task<object> UpdatePageFile(Guid webId, string? newPages)
        {
            if (newPages == null)
            return await _websiteAppService.UpdatePageFile(webId, "");
            else
                return await _websiteAppService.UpdatePageFile(webId, newPages);

        }

        [HttpPost("save-final-website/{webId}")]
        public bool ExportFinalFileOfWebsiteCreating(Guid webId, [FromBody] FileNameContentRequest[] fileNameContentRequests)
        {
            return _websiteAppService.SaveFinalFileOfWebsiteCreating(webId, fileNameContentRequests);
        }

        [HttpGet("export-final-website/{webId}")]
        public object ExportFinalFileOfWebsiteCreating(Guid webId)
        {
            IEnumerable<FileNameAndByteDTO> listBytes = _websiteAppService.ExportFinalFileOfWebsiteCreating(webId);
            using (var ms = new MemoryStream())
            {
                using (var archive =
                new System.IO.Compression.ZipArchive(ms, ZipArchiveMode.Create, true))
                {

                    var zipEntry = (ZipArchiveEntry)null;
                    foreach (var (item, index) in listBytes.Select((value, i) => (value, i)))
                    {
                        zipEntry = archive.CreateEntry(item.fileName, CompressionLevel.Fastest);
                        using (var zipStream = zipEntry.Open())
                        {
                            zipStream.Write(item.bytes, 0, item.bytes.Length);
                        }
                    }
                }
                return File(ms.ToArray(), "application/zip", "Final-Content-Website-" + webId + ".zip");
            }
        }

    }
}
