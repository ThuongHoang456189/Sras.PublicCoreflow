using Microsoft.AspNetCore.Identity;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace Sras.PublicCoreflow.Data
{
    public class ConferenceManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IGuidGenerator _guidGenerator;

        private readonly IRepository<Participant, Guid> _participantRepository;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IRepository<IdentityUser, Guid> _userRepository;

        private Guid _participantSandraId;
        private Guid _participantSergeyId;
        private Guid _participantWellyId;
        private Guid _participantAlessandroId;
        private Guid _participantMarkId;
        private Guid _participantTonyId;
        private Guid _participantDavidGrausId;
        private Guid _participantDavidRossId;
        private Guid _participantTomId;
        private Guid _participantShreeId;

        private IdentityUser _sandra;
        private IdentityUser _sergey;
        private IdentityUser _welly;
        private IdentityUser _alessandro;
        private IdentityUser _mark;
        private IdentityUser _tony;
        private IdentityUser _davidGraus;
        private IdentityUser _davidRoss;
        private IdentityUser _tom;
        private IdentityUser _shree;

        public ConferenceManagementDataSeedContributor(
            IGuidGenerator guidGenerator,
            IRepository<Participant, Guid> participantRepository,
            IdentityUserManager identityUserManager,
            IRepository<IdentityUser, Guid> userRepository)
        {
            _guidGenerator = guidGenerator;
            _participantRepository = participantRepository;
            _identityUserManager = identityUserManager;
            _userRepository = userRepository;
        }

        private async Task CreateParticipantsAsync()
        {
            if(await _participantRepository.GetCountAsync() > 0)
            {
                return;
            }

            var participants = new List<Participant>
            {
                new Participant(_participantSandraId = _guidGenerator.Create()),
                new Participant(_participantSergeyId = _guidGenerator.Create()),
                new Participant(_participantWellyId = _guidGenerator.Create()),
                new Participant(_participantAlessandroId = _guidGenerator.Create()),
                new Participant(_participantMarkId = _guidGenerator.Create()),
                new Participant(_participantTonyId = _guidGenerator.Create()),
                new Participant(_participantDavidGrausId = _guidGenerator.Create()),
                new Participant(_participantDavidRossId = _guidGenerator.Create()),
                new Participant(_participantTomId = _guidGenerator.Create()),
                new Participant(_participantShreeId = _guidGenerator.Create()),
            };

            await _participantRepository.InsertManyAsync(participants);
        }

        private async Task CreateSampleUsersAsync()
        {
            _sandra = new IdentityUser(_guidGenerator.Create(), "SandraWolf", "sandra_wolf@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantSandraId);
            _sandra.Name = "Sandra";
            _sandra.Surname = "Wolf";

            _sergey = new IdentityUser(_guidGenerator.Create(), "SergeyPolgul", "sergey_polgul@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantSergeyId);
            _sergey.Name = "Sergey";
            _sergey.Surname = "Polgul";

            _welly = new IdentityUser(_guidGenerator.Create(), "WellyTambunan", "welly_tambunan@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantWellyId);
            _welly.Name = "Welly";
            _welly.Surname = "Tambunan";

            _alessandro = new IdentityUser(_guidGenerator.Create(), "AlessandroMuci", "alessandro_muci@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantAlessandroId);
            _alessandro.Name = "Alessandro";
            _alessandro.Surname = "Muci";

            _mark = new IdentityUser(_guidGenerator.Create(), "MarkGodfrey", "mark_godfrey@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantMarkId);
            _mark.Name = "Mark";
            _mark.Surname = "Godfrey";

            _tony = new IdentityUser(_guidGenerator.Create(), "TonyBurton", "tony_burton@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantTonyId);
            _tony.Name = "Tony";
            _tony.Surname = "Burton";

            _davidGraus = new IdentityUser(_guidGenerator.Create(), "DavidGraus", "david_graus@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantDavidGrausId);
            _davidGraus.Name = "David";
            _davidGraus.Surname = "Graus";

            _davidRoss = new IdentityUser(_guidGenerator.Create(), "DavidRoss", "david_ross@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantDavidRossId);
            _davidRoss.Name = "David";
            _davidRoss.Surname = "Ross";

            _tom = new IdentityUser(_guidGenerator.Create(), "TomLidy", "tom_lidy@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantTomId);
            _tom.Name = "Tom";
            _tom.Surname = "Lidy";

            _shree = new IdentityUser(_guidGenerator.Create(), "ShreePatel", "shree_patel@gmail.com")
                .SetProperty(AccountConsts.ParticipantPropertyName, _participantShreeId);
            _shree.Name = "Shree";
            _shree.Surname = "Patel";

            var users = new List<IdentityUser>
            {
                _sandra,
                _sergey,
                _welly,
                _alessandro,
                _mark,
                _tony,
                _davidGraus,
                _davidRoss,
                _tom,
                _shree,
            };

            foreach (var user in users)
            {
                (await _identityUserManager.CreateAsync(user, "Abc#123")).CheckErrors();
            }
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _userRepository.GetCountAsync() <= 1)
            {
                await CreateParticipantsAsync();
                await CreateSampleUsersAsync();
            }
        }
    }
}
