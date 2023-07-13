using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class SrasBackgroundAppService : PublicCoreflowAppService, ISrasBackgroundAppService
    {
        private readonly IConferenceRepository _conferenceRepository;

        public SrasBackgroundAppService(IConferenceRepository conferenceRepository)
        {
            _conferenceRepository = conferenceRepository;
        }

        public async Task UpdateActivityTimelineAsync()
        {
            await _conferenceRepository.UpdateActivityTimelineAsync();
        }
    }
}
