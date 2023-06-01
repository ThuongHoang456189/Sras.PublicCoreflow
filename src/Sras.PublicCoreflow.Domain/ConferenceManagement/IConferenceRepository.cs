using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Volo.Abp.Domain.Repositories;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public interface IConferenceRepository : IRepository<Conference, Guid>
    {
        Task<int> GetCountAsync(
            string? inclusionText = null,
            string? fullName = null,
            string? shortName = null,
            string? city = null,
            string? country = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isAccepted = null,
            Guid? accountId = null,
            CancellationToken cancellationToken = default);

        Task<List<ConferenceWithBriefInfo>> GetListAsync(
            string? sorting = null,
            int skipCount = 0,
            int maxResultCount = int.MaxValue,
            string? inclusionText = null,
            string? fullName = null,
            string? shortName = null,
            string? city = null,
            string? country = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isAccepted = true,
            Guid? accountId = null,
            CancellationToken cancellationToken = default);

        Task<bool> IsExistAsync(
            string fullName,
            string shortName,
            string city,
            string country,
            DateTime startDate,
            DateTime endDate);

        Task<List<IncumbentOperation>> GetIncumbentOperationTableAsync(Guid conferenceId);
    }
}
