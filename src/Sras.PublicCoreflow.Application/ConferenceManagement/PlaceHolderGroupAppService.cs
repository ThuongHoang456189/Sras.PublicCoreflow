using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class PlaceHolderGroupAppService : PublicCoreflowAppService, IPlaceHolderGroupAppService
    {
        private readonly IPlaceHolderRepository _placeHolderRepository;
        public PlaceHolderGroupAppService(IPlaceHolderRepository placeHolderRepository)
        {
            _placeHolderRepository = placeHolderRepository;
        }

        public async Task<IEnumerable<object>> GetAllSupportedPlaceHolderAsync()
        {
            return await _placeHolderRepository.GetAllSupportedPlaceHolder();
        }
    }
}
