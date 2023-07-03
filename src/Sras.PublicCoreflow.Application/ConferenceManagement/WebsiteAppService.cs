using Sras.PublicCoreflow.BlobContainer;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class WebsiteAppService : PublicCoreflowAppService, IWebsiteAppService
    {
        private readonly IGuidGenerator _guidGenerator;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IBlobContainer<WebsiteContainer> _webBlobContainer;
        private readonly string TEMP_FOLDER_NAME = "temp";
        private readonly string FINAL_FOLDER_NAME = "final";

        public WebsiteAppService(IGuidGenerator guidGenerator, IWebsiteRepository websiteRepository, IBlobContainer<WebsiteContainer> webBlobContainer)
        {
            _guidGenerator = guidGenerator;
            _websiteRepository = websiteRepository;
            _webBlobContainer = webBlobContainer;
        }

        //private void SaveFileAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        //{
        //    _webBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        //}

        public async Task<object> getNavbarByConferenceId(Guid conferenceId)
        {
            return await _websiteRepository.getNavbarByConferenceId(conferenceId);
        }

        public async Task<object> CreateWebsite(Guid webtemplateId, Guid conferenceId)
        {
            return await _websiteRepository.CreateWebsite(webtemplateId, conferenceId);
        }

        //public RemoteStreamContent GetRemoteStreamFileFromContent(string fileContent)
        //{

        //    byte[] contentBytes = System.Text.Encoding.Unicode.GetBytes(fileContent);
        //    var remoteStreamContent = (RemoteStreamContent)null;
        //    using (Stream stream = new MemoryStream(contentBytes))
        //    {
        //        stream.WriteAsync(contentBytes, 0, contentBytes.Length);
        //        stream.FlushAsync();
        //        stream.Position = 0;
        //        remoteStreamContent = new RemoteStreamContent(stream);
        //    }

        //    return remoteStreamContent;
        //}

        //public ResponseDto SaveContentWebsiteFiles(string filePath, RemoteStreamContent file)
        //{
        //    ResponseDto response = new();

        //    // Check valid submission

        //    try
        //    {
        //        // Assume that the file extension is exactly matched its file name extension
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            _webBlobContainer.SaveAsync(filePath, file.GetStream(), true);
        //        }

        //        response.IsSuccess = true;
        //        response.Message = "Create content files successfully";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = "Exception";
        //        throw new Exception("Error to save file");
        //    }

        //    return response;
        //}

        public async Task<object> UpdateNavbarByConferenceId(Guid conferenceId, Guid webTemplateId, NavbarDTO navbarDTO)
        {
            return await _websiteRepository.UpdateNavbarByConferenceId(conferenceId, webTemplateId, navbarDTO);
        }

        public void UploadContentOfWebsite(Guid conferenceId, string fileName, string contentTemp, string contentFinal)
        {
            // craete html file with content inside
            //RemoteStreamContent tempFile = GetRemoteStreamFileFromContent(contentTemp);
            //RemoteStreamContent finalFile = GetRemoteStreamFileFromContent(contentFinal);
            //// create file in temp folder {conferenceId}/temp/{fileName}.html
            //SaveContentWebsiteFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + fileName, tempFile);
            //// create file in final folder {conferenceId}/final/{fileName}.html
            //SaveContentWebsiteFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + fileName, finalFile);
            //_websiteRepository.AddContentToWebsite(conferenceId, fileName);

            using (var stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteAsync(contentTemp);
                    writer.Flush();

                    stream.Position = 0;
                    var remoteStreamContent = new RemoteStreamContent(stream);

                    var result = _webBlobContainer.SaveAsync(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + fileName, remoteStreamContent.GetStream(), true);
                }
            }
            using (var stream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteAsync(contentFinal);
                    writer.Flush();

                    stream.Position = 0;
                    var remoteStreamContent = new RemoteStreamContent(stream);

                    var result = _webBlobContainer.SaveAsync(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + fileName, remoteStreamContent.GetStream(), true);
                }
            }

        }

        public byte[] GetWebsiteFiles(string rootFilePath)
        {
            return _webBlobContainer.GetAllBytesOrNullAsync(rootFilePath).Result;
        }

        public bool DeleteContentFiles(string rootFilePath)
        {
            return _webBlobContainer.DeleteAsync(rootFilePath).Result;
        }
        public async Task<IEnumerable<object>> GetContentTempOfWebsite(Guid conferenceId)
        {
            var websiteNames = await _websiteRepository.GetAllPageNameOfWebsite(conferenceId);
            if (websiteNames.Count() == 0) { 
                return Enumerable.Empty<object>();
            }
            return websiteNames.Select(w =>
            {
                byte[] file = GetWebsiteFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + w);
                var content = Encoding.Default.GetString(file);
                return new
                {
                    fileName = w,
                    content
                };
            });
        }

        public async Task<IEnumerable<object>> GetContentFinalOfWebsite(Guid conferenceId)
        {
            var websiteNames = await _websiteRepository.GetAllPageNameOfWebsite(conferenceId);

            return websiteNames.Select(w =>
            {
                byte[] file = GetWebsiteFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + w);
                var content = Encoding.Default.GetString(file);
                return new
                {
                    fileName = w,
                    content
                };
            });
        }

        public async Task<IEnumerable<object>> GetAllWebsite()
        {
            return await _websiteRepository.GetAllWebsite();
        }

        public async Task<IEnumerable<FileNameAndByteDTO>> DownloadAllFinalFile(Guid conferenceId)
        {
            var listWebsiteFileNames = await _websiteRepository.GetAllPageNameOfWebsite(conferenceId);
            return listWebsiteFileNames.ToList().Select(name => 
            new FileNameAndByteDTO() { 
                bytes = GetWebsiteFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + name),
                fileName = name
            });
        }

        public async Task<bool> DeleteNavbarAndHrefFile(Guid conferenceId, string idParent, string idChild)
        {
            if (idChild != null)
            {
                var deleteInDb = _websiteRepository.DeleteFileNameInPages(conferenceId, new List<string>() { idParent + "@" + idChild + ".html" });
                return DeleteContentFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + idParent + "@" + idChild + ".html") && 
                    DeleteContentFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + idParent + "@" + idChild + ".html") && 
                    deleteInDb;
            } else
            {
                var listWebsiteFileNames = await _websiteRepository.GetAllPageNameOfWebsite(conferenceId);
                var needToRemove = listWebsiteFileNames.ToList()
                    .Where(name => name.StartsWith(idParent)).ToList();
                var deleteInDb = _websiteRepository.DeleteFileNameInPages(conferenceId, needToRemove);
                //return listWebsiteFileNames.ToList()
                //    .Where(name => name.Contains(idParent)).ToList()
                //    .Select(na => DeleteContentFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + na) && DeleteContentFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + na))
                //    .Any(statusDelete => statusDelete == false) && deleteInDb;

                
                //var removeFile = needToRemove.Select(na => DeleteContentFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + na) && DeleteContentFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + na));
                var isSuccess = true;
                foreach ( var item in needToRemove )
                {
                    isSuccess = isSuccess && DeleteContentFiles(conferenceId + "/" + FINAL_FOLDER_NAME + "/" + item);
                    isSuccess = isSuccess && DeleteContentFiles(conferenceId + "/" + TEMP_FOLDER_NAME + "/" + item);
                }
                return isSuccess && deleteInDb;
            }
        }

        public async Task<object> UpdatePageFile(Guid webId, string newPages)
        {
            return await _websiteRepository.UpdatePageFile(webId, newPages);
        }

        public bool SaveFinalFileOfWebsiteCreating(Guid webId, FileNameContentRequest[] fileNameContentRequests)
        {
            var listWebsiteFileNames = fileNameContentRequests.ToList();
            var listFilenames = listWebsiteFileNames.Select(file => file.fileName);
            var oldPage = _websiteRepository.GetWebsitePage(webId);
            _websiteRepository.UpdatePageFile(webId, string.Join(";", listFilenames));
            var listLabelHref = _websiteRepository.GetAllLabelHrefNavbar(webId);
            var isDeleteSuccess = true;
            foreach (var file in listWebsiteFileNames )
            {
                foreach ( var label in listLabelHref.Keys)
                {
                    string pattern = @"\{\{" + label + @":(.*?)\}\}";

                    // Create a Regex object with the pattern
                    Regex regex = new Regex(pattern);
                    if (regex.IsMatch(file.finalContent))
                    {
                        Match match = regex.Match(file.finalContent);
                        string anyText = null;
                        // Check if a match is found
                        if (match.Success)
                        {
                            // Get the captured value (anytext)
                            anyText = match.Groups[1].Value;
                            if (string.IsNullOrEmpty(anyText)) {
                                anyText = label;
                            }
                        }
                        else
                        {
                            anyText = label;
                        }

                        // Replace the matched pattern with the desired text
                        file.finalContent = regex.Replace(file.finalContent, "<a href=\"" + listLabelHref[label] + "\">" + anyText + "</a>");
                    }
                }

                //foreach (var needDel in oldPage.Split(";").ToList())
                //{
                //    isDeleteSuccess = isDeleteSuccess && _webBlobContainer.DeleteAsync(webId.ToString() + "/" + TEMP_FOLDER_NAME + needDel).Result;
                //    isDeleteSuccess = isDeleteSuccess && _webBlobContainer.DeleteAsync(webId.ToString() + "/" + FINAL_FOLDER_NAME + needDel).Result;
                //}
                try
                {
                        // Check if the folder exists
                        if (Directory.Exists("host/sras-websites/" + webId.ToString()))
                        {
                            // Delete the folder and its contents recursively
                            Directory.Delete("host/sras-websites/" + webId.ToString(), true);
                            Console.WriteLine("Folder deleted successfully.");
                        }
                        else
                        {
                            return false;
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
                UploadContentOfWebsite(webId, file.fileName, file.tempContent, file.finalContent);
            }
            return true & isDeleteSuccess;
        }

        public IEnumerable<FileNameAndByteDTO> ExportFinalFileOfWebsiteCreating(Guid webId)
        {
            IEnumerable<string> listNames = _websiteRepository.GetAllPageNameOfWebsite(webId).Result;
            return listNames.ToList().Select(name =>
            new FileNameAndByteDTO()
            {
                bytes = GetWebsiteFiles(webId + "/" + FINAL_FOLDER_NAME + "/" + name),
                fileName = name
            });
        }
    }
}
