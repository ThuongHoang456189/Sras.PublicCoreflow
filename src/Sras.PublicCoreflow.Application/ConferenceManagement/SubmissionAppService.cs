using Sras.PublicCoreflow.BlobContainer;
using System;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAppService : PublicCoreflowAppService, ISubmissionAppService
    {
        private readonly IBlobContainer<SubmissionContainer> _submissionBlobContainer;

        public SubmissionAppService(IBlobContainer<SubmissionContainer> submissionBlobContainer)
        {
            _submissionBlobContainer = submissionBlobContainer;
        }

        //public async Task SaveBytesAsync(byte[] bytes)
        //{
        //    await _submissionBlobContainer.SaveAsync("my-blob-1", bytes);
        //}

        //public async Task<byte[]> GetBytesAsync()
        //{
        //    return await _submissionBlobContainer.GetAllBytesOrNullAsync("my-blob-1");
        //}

        private async Task CreateSubmissionFilesAsync(string blobName, IRemoteStreamContent streamContent, bool overrideExisting = true)
        {
            await _submissionBlobContainer.SaveAsync(blobName, streamContent.GetStream(), overrideExisting);
        }

        private async Task DeleteSubmissionFilesAsync(string blobName)
        {
            await _submissionBlobContainer.DeleteAsync(blobName);
        }

        public void Create(SubmissionInput input)
        {


            input.Files.ForEach(async file =>
            {
                if (file != null && file.ContentLength > 0)
                {
                    await CreateSubmissionFilesAsync("test/hello", file, true);
                }
            });
        }
    }
}
