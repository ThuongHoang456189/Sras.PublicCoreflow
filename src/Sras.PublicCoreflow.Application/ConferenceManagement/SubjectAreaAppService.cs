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
    public class SubjectAreaAppService : PublicCoreflowAppService, ISubjectAreaAppService
    {
        private readonly IRepository<Track, Guid> _trackRepository;
        private readonly IRepository<SubjectArea, Guid> _subjectAreaRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public SubjectAreaAppService(IRepository<Track, Guid> trackRepository, IRepository<SubjectArea, Guid> subjectAreaRepository,
            ICurrentUser currentUser, IGuidGenerator guidGenerator)
        {
            _trackRepository = trackRepository;
            _subjectAreaRepository = subjectAreaRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }

        public async Task CreateAsync(SubjectAreaInput input)
        {
            // Check authority

            var track = await _trackRepository.FindAsync(input.TrackId);
            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            track.AddSubjectArea(_guidGenerator.Create(), input.SubjectAreaName);

            await _trackRepository.UpdateAsync(track, true);
        }

        public async Task UpdateAsync(Guid subjectAreaId, SubjectAreaInput input)
        {
            // Check authority

            var track = await _trackRepository.FindAsync(input.TrackId);
            if (track == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.TrackNotFound);
            }

            track.UpdateSubjectArea(subjectAreaId, input.SubjectAreaName);

            await _trackRepository.UpdateAsync(track, true);
        }

        public async Task DeleteAsync(Guid subjectAreaId)
        {
            // Check authority

            await _subjectAreaRepository.DeleteAsync(subjectAreaId, true);
        }

        public async Task<List<SubjectAreaBriefInfo>> GetListAsync (Guid trackId)
        {
            return ObjectMapper.Map<List<SubjectArea>, List<SubjectAreaBriefInfo>>(await _subjectAreaRepository.GetListAsync(x => x.TrackId == trackId));
        }
    }
}
