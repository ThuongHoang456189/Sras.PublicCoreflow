using Sras.PublicCoreflow.ConferenceManagement;
using System;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Identity;
using System.Linq.Dynamic.Core;
using Sras.PublicCoreflow.Dto;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class OutsiderRepository : EfCoreRepository<PublicCoreflowDbContext, Outsider, Guid>, IOutsiderRepository
    {
        public OutsiderRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        private readonly IGuidGenerator _guidGenerator;

        public async Task<OutsiderCreateResponse> CreateOutsider(OutsiderCreateRequest request)
        {
            if (request == null)
            {
                throw new Exception("Invalid Input");
            }
            else
            {
                try
                {
                    var outsiderId = _guidGenerator.Create();
                    var participantId = _guidGenerator.Create();
                    var outsider = new Outsider(outsiderId, request.Email, request.Firstname, "", request.Lastname, request.Organization);
                    var participant = new Participant(participantId);
                    participant.Outsiders.Add(outsider);

                    var dbContext = await GetDbContextAsync();
                    await dbContext.Participants.AddAsync(participant);
                    await dbContext.SaveChangesAsync();

                    return new OutsiderCreateResponse()
                    {
                        OutsiderId = outsiderId.ToString(),
                        Email = request.Email,
                        Firstname = request.Firstname,
                        Lastname = request.Lastname,
                        Organization = request.Organization,
                        hasAccount = false,
                        ParticipantId = participantId.ToString(),
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("[ERROR] Create new outsider or participant FAIL", ex);
                }

            }
        }

        public async Task<IEnumerable<object>> GetAllOutsiders()
        {
            var dbContext = await GetDbContextAsync();
            var result = await dbContext.Outsiders.Select(x => new 
            {
                OutsiderId = x.Id.ToString(),
                Email = x.Email,
                Firstname = x.FirstName,
                Lastname = x.LastName,
                Organization = x.Organization,
                ParticipantId = x.ParticipantId.ToString(),
            }).ToListAsync();

            return result;
        }

    }
}