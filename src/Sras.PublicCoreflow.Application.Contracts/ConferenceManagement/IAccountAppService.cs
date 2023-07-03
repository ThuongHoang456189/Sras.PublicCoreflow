﻿using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IAccountAppService : IApplicationService
    {
        bool ConfirmEmail(Guid id);
        Task<AccountWithBriefInfo?> FindAsync(string email);
        IEnumerable<RegisterAccountRequest> GetAllAccount();
        bool UpdateAccount(RegisterAccountRequest registerAccount);
    }
}
