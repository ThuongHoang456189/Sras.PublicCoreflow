using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConflictCaseAppService : PublicCoreflowAppService, IConflictCaseAppService
    {
        private readonly IConflictCaseRepository _conflictCaseRepository;
        public ConflictCaseAppService(IConflictCaseRepository conflictCaseRepository)
        {
            _conflictCaseRepository = conflictCaseRepository;
        }

        public async Task<IEnumerable<object>> GetAllConflictCasesAsync([Optional] Guid trackId)
        {
            return await _conflictCaseRepository.GetAllConflictCases(trackId); 
        }
    }
}
