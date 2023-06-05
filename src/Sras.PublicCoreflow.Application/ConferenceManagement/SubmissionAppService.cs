using Sras.PublicCoreflow.BlobContainer;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SubmissionAppService : PublicCoreflowAppService, ISubmissionAppService
    {
        private readonly IBlobContainer<SubmissionContainer> _submissionBlobContainer;

        public SubmissionAppService(IBlobContainer<SubmissionContainer> submissionBlobContainer)
        {
            _submissionBlobContainer = submissionBlobContainer;
        }

        public async Task SaveBytesAsync(byte[] bytes)
        {
            await _submissionBlobContainer.SaveAsync("my-blob-1", bytes);
        }

        public async Task<byte[]> GetBytesAsync()
        {
            return await _submissionBlobContainer.GetAllBytesOrNullAsync("my-blob-1");
        }
    }
}
