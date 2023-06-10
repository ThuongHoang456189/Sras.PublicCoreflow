using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class PaperStatusAppService : PublicCoreflowAppService, IPaperStatusAppService
    {

        private readonly IPaperStatusRepository _paperStatusRepository;
        public PaperStatusAppService(IPaperStatusRepository paperStatusRepository) {
            _paperStatusRepository = paperStatusRepository;
        }

        public async Task<IEnumerable<object>> GetAllPaperStatusAsync()
        {
            return await _paperStatusRepository.GetAllPaperStatus();
        }

    }
}
