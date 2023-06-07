using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class OutsiderAppService : PublicCoreflowAppService, IOutsiderAppService
    {
        private readonly IOutsiderRepository _outsiderRepository;
        private readonly IGuidGenerator _guidGenerator;

        public OutsiderAppService(IOutsiderRepository outsiderRepository,
            ICurrentUser currentUser, IGuidGenerator guidGenerator)
        {
            _guidGenerator = guidGenerator;
            _outsiderRepository = outsiderRepository;
        }

        public async Task<OutsiderCreateResponse> CreateOutsider(OutsiderCreateRequest request)
        {
            return await _outsiderRepository.CreateOutsider(request);
        }

        public async Task<IEnumerable<object>> GetAllOutsider()
        {
            return await _outsiderRepository.GetAllOutsiders();
        }

        public async Task<object> UpdateOutsider(OutsiderUpdateRequest request)
        {
            return await _outsiderRepository.UpdateOutsider(request);
        }

        public async Task<object> SearchOutsiderByEmail(string email)
        {
            return await _outsiderRepository.SearchOutsiderByEmail(email);
        }
    }
}