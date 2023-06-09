using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ConferenceRoleAppService : PublicCoreflowAppService, IConferenceRoleAppService
    {
        private readonly IRepository<Conference, Guid> _conferenceRepository;
        private readonly IRepository<ConferenceRole, Guid> _conferenceRoleRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IIncumbentRepository _incumbentRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        private Dictionary<string, Guid> _conferenceRoles;

        private const string Chair = "chair";
        private const string Author = "author";
        private const string TrackChair = "track chair";
        private const string Reviewer = "reviewer";

        public ConferenceRoleAppService(
            IRepository<Conference, Guid> conferenceRepository,
            IRepository<ConferenceRole, Guid> conferenceRoleRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IIncumbentRepository incumbentRepository, 
            ICurrentUser currentUser, 
            IGuidGenerator guidGenerator)
        {
            _conferenceRepository = conferenceRepository;
            _conferenceRoleRepository = conferenceRoleRepository;
            _userRepository = userRepository;
            _incumbentRepository = incumbentRepository;
            _currentUser = currentUser;
            _guidGenerator = guidGenerator;

            _conferenceRoles = new Dictionary<string, Guid>();
        }

        private async Task GetMapConferenceRoles()
        {
            var roleList = await _conferenceRoleRepository.GetListAsync();
            roleList.ForEach(role =>
            {
                _conferenceRoles.Add(role.Name.ToLower(), role.Id);
            });
        }

        public async Task<ResponseDto> CreateOrUpdateAsync(UserConferenceRoleInput input)
        {
            // Check if conference exist
            var conference = await _conferenceRepository.FindAsync(input.ConferenceId);
            if (conference == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ConferenceNotFound);
            }

            // Check authority


            await GetMapConferenceRoles();

            if(_currentUser != null && _currentUser.Id != null)
            {
                var currentUserRoles = await _incumbentRepository.GetRoleTrackOperationTableAsync(_currentUser.Id.Value, input.ConferenceId);

                if (input.TrackId == null || conference.IsSingleTrack)
                {
                    // Current user need to be the Chair
                    if (!currentUserRoles.Any(x => x.RoleId == _conferenceRoles.GetValueOrDefault(Chair)))
                        throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToProceedToUpdateConferenceRoles);
                }
                else
                {
                    // Current user need to be Track chair of the track ID or Chair
                    if (!currentUserRoles.Any(x => 
                        x.RoleId == _conferenceRoles.GetValueOrDefault(Chair) ||
                        x.RoleId == _conferenceRoles.GetValueOrDefault(TrackChair) && x.TrackId == input.TrackId))
                        throw new BusinessException(PublicCoreflowDomainErrorCodes.UserNotAuthorizedToProceedToUpdateConferenceRoles);
                }
            }

            // Check valid account
            var userQueryable = await _userRepository.GetQueryableAsync();
            var user = userQueryable.Where(u => u.Id == input.AccountId);
            if (user == null)
            {
                throw new BusinessException(PublicCoreflowDomainErrorCodes.AccountNotFound);
            }

            ResponseDto response = new();

            try
            {
                // Clean the input
                if (conference.IsSingleTrack)
                {
                    input.TrackId = null;
                    var defaultTrack = conference.Tracks.SingleOrDefault(x => x.IsDefault);

                    for (int i = 0; i < input.Roles.Count; i++)
                    {
                        // Remove Duplicate
                        for (int j = i + 1; j < input.Roles.Count; j++)
                        {
                            if (input.Roles[i].RoleId == input.Roles[j].RoleId)
                            {
                                input.Roles.Remove(input.Roles[j]);
                                j--;
                            }
                        }

                        if (input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(Author) || input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(TrackChair))
                        {
                            input.Roles.Remove(input.Roles[i]);
                            i--;
                        }

                        if (input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(Reviewer))
                        {
                            input.Roles[i].TrackId = defaultTrack?.Id;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < input.Roles.Count; i++)
                    {
                        // Remove Duplicate
                        for (int j = i + 1; j < input.Roles.Count; j++)
                        {
                            if ((input.Roles[i].TrackId == null || input.Roles[i].TrackId == input.Roles[j].TrackId) && input.Roles[i].RoleId == input.Roles[j].RoleId)
                            {
                                input.Roles.Remove(input.Roles[j]);
                                j--;
                            }
                        }

                        if (input.TrackId != null)
                        {
                            if (input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(Author) || input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(Chair) || input.Roles[i].TrackId != input.TrackId)
                            {
                                input.Roles.Remove(input.Roles[i]);
                                i--;
                            }
                        }
                        else
                        {
                            if (input.Roles[i].RoleId == _conferenceRoles.GetValueOrDefault(Author))
                            {
                                input.Roles.Remove(input.Roles[i]);
                                i--;
                            }
                        }
                    }
                }

                // Allocate operations
                var roleTrackOperationTable = await _incumbentRepository.GetRoleTrackOperationTableAsync(input.AccountId, input.ConferenceId);

                if (roleTrackOperationTable == null)
                {
                    roleTrackOperationTable = new List<RoleTrackOperation>();
                }

                if (input.TrackId != null)
                {
                    roleTrackOperationTable.ForEach(x =>
                    {
                        if (x.TrackId == input.TrackId && x.RoleId != _conferenceRoles.GetValueOrDefault(Author))
                        {
                            if (!input.Roles.Any(y => y.TrackId == x.TrackId && y.RoleId == x.RoleId))
                            {
                                x.Operation = RoleTrackManipulationOperators.Del;
                            }
                        }

                    });
                }
                else
                {
                    roleTrackOperationTable.ForEach(x =>
                    {
                        if (x.RoleId != _conferenceRoles.GetValueOrDefault(Author))
                        {
                            if (!input.Roles.Any(y => ((y.TrackId == x.TrackId) || (x.TrackId == null && y.TrackId == null)) && y.RoleId == x.RoleId))
                            {
                                x.Operation = RoleTrackManipulationOperators.Del;
                            }
                        }
                    });
                }

                var conferenceAccount = conference.ConferenceAccounts.SingleOrDefault(x => x.ConferenceId == input.ConferenceId && x.AccountId == input.AccountId);

                if (conferenceAccount == null)
                {
                    conferenceAccount = new ConferenceAccount(_guidGenerator.Create(), input.ConferenceId, input.AccountId, false);
                }

                input.Roles.ForEach(x =>
                {
                    if (!roleTrackOperationTable.Any(y => ((y.TrackId == x.TrackId) || (x.TrackId == null && y.TrackId == null)) && y.RoleId == x.RoleId))
                    {
                        var newRow = new RoleTrackOperation(conferenceAccount.AccountId, _guidGenerator.Create(), x.RoleId, x.TrackId, RoleTrackManipulationOperators.Add);
                        roleTrackOperationTable.Add(newRow);
                    }
                });

                // Perform Operations
                if (roleTrackOperationTable.All(x => x.Operation == RoleTrackManipulationOperators.Del))
                {
                    conferenceAccount.Incumbents.Clear();
                    conference.DeleteConferenceAccount(conferenceAccount.Id);
                }
                else
                {
                    roleTrackOperationTable.ForEach(x =>
                    {
                        if (x.Operation == RoleTrackManipulationOperators.Del)
                        {
                            conferenceAccount.DeleteIncumbent(x.IncumbentId);
                        }
                        else if (x.Operation == RoleTrackManipulationOperators.Add)
                        {
                            var incumbent = new Incumbent(x.IncumbentId, conferenceAccount.Id, x.RoleId, x.TrackId, false);

                            if(x.RoleId == _conferenceRoles.GetValueOrDefault(Reviewer))
                            {
                                incumbent.AddReviewer(new Reviewer(incumbent.Id, null));
                            }

                            conferenceAccount.AddIncumbent(incumbent);
                        }
                    });

                    if (roleTrackOperationTable.All(x => x.Operation == RoleTrackManipulationOperators.Add))
                    {
                        conference.AddConferenceAccount(conferenceAccount);
                    }
                    else
                    {
                        conference.UpdateConferenceAccount(conferenceAccount);
                    }
                }

                await _conferenceRepository.UpdateAsync(conference);

                response.IsSuccess = true;
                response.Message = "Update user conference role(s) successfully";
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }
            
            return response;
        }

        public async Task<ConferenceParticipationInfo?> GetConferenceParticipationInfoAsync (ConferenceParticipationInput input)
        {
            return await _incumbentRepository.GetConferenceParticipationInfoAsync(input.AccountId, input.ConferenceId, input.TrackId);
        }

        public async Task<List<object>> GetAllConferenceRole()
        {
            var conferenceRoleList = await _conferenceRoleRepository.GetListAsync();
            List<object> resultList = new ();
            conferenceRoleList.ForEach(x =>
            {
                resultList.Add(new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Factor = x.Factor
                });
            });

            return resultList;
        }
    }
}
