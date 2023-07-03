using System.Threading.Tasks;
using System;
using Volo.Abp.Application.Services;
using System.Collections.Generic;
using Sras.PublicCoreflow.Dto;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IRegistrationAppService : IApplicationService
    {
        Task<RegistrablePaperTable> GetRegistrablePaperTable(Guid conferenceId, Guid accountId);
        Task<RegistrationResponseDto> CreateRegistration(Guid accountId, Guid conferenceId, string mainPaperOption, List<RegistrationInput> registrations);
    }
}
