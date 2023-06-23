using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class UserAppService : PublicCoreflowAppService, IUserAppService
    {
        private readonly IIdentityUserRepository _userRepository;

        public UserAppService(IIdentityUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<IdentityRoleDto>> GetRolesAsync(Guid id)
        {
            var roles = await _userRepository.GetRolesAsync(id);

            return new List<IdentityRoleDto>(
                ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(roles)
            );
        }
    }
}
