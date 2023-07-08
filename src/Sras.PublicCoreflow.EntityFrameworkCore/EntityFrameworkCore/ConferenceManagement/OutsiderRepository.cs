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
                    var dbContext = await GetDbContextAsync();
                    if (dbContext.Users.Any(u => u.Email == request.Email) || dbContext.Outsiders.Any(o => o.Email == request.Email))
                    {
                        throw new Exception("Duplicate Email");
                    }
                    var outsiderId = _guidGenerator.Create();
                    var participantId = _guidGenerator.Create();

                    // Modified this
                    var outsider = new Outsider(outsiderId, request.Email, null, request.Firstname, request.Middlename, request.Lastname, request.Organization, request.Country);
                    var participant = new Participant(participantId, null, outsiderId);
                    outsider.Participants.Add(participant);
                    //participant.Outsiders.Add(outsider);

                    var outsiderResult = await dbContext.Outsiders.AddAsync(outsider);
                    await dbContext.SaveChangesAsync();
                    var result = dbContext.Outsiders.Include(o => o.Participants).FirstOrDefault(o => o.Email == request.Email);
                    return new OutsiderCreateResponse()
                    {
                        OutsiderId = result.Id.ToString(),
                        Email = result.Email,
                        Firstname = result.FirstName,
                        Middlename = result.MiddleName,
                        Lastname = result.LastName,
                        Organization = result.Organization,
                        hasAccount = false,
                        ParticipantId = result.Participants.First().Id,
                        Country = result.Country
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("[ERROR] Create new outsider or participant FAIL: " + ex.Message, ex);
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
                MiddleName = x.MiddleName,
                Lastname = x.LastName,
                Organization = x.Organization,
                Country = x.Country,
                //ParticipantId = x.ParticipantId.ToString(),
            }).ToListAsync();

            return result;
        }

        public async Task<object> UpdateOutsider(OutsiderUpdateRequest request)
        {
            var dbContext = await GetDbContextAsync();
            var isExisting = await dbContext.Outsiders.AnyAsync(outs => outs.Id == request.Id);
            if (isExisting)
            {
                Outsider needToUpdate = await dbContext.Outsiders.FindAsync(request.Id);
                if (request.Firstname != null) needToUpdate.SetFirstName(request.Firstname);
                if (request.Middlename != null) needToUpdate.SetMiddleName(request.Middlename);
                if (request.Lastname != null) needToUpdate.SetLastName(request.Lastname);
                if (request.Email != null)
                {
                    if (request.Email != needToUpdate.Email)
                    {
                        if (!dbContext.Outsiders.Any(o => o.Email == request.Email) &&
                        !dbContext.Users.Any(u => u.Email == request.Email))
                            needToUpdate.SetEmail(request.Email);
                        else throw new Exception("Email is existing");
                    }
                }
                if (request.Organization != null) needToUpdate.SetOrganization(request.Organization);
                if (request.Country != null) needToUpdate.SetCountry(request.Country);
                dbContext.SaveChanges();
                return new
                {
                    message = "Update Success"
                };
            }
            else
            {
                throw new Exception("Update Outsider Repo: The OutsiderId is not Existing");
            }
        }

        public async Task<object> SearchOutsiderByEmail(string email)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (dbContext.Users.Any(u => u.Email == email))
                    return dbContext.Users.Where(us => us.Email == email).Select(r => new
                    {
                        userId = r.Id,
                        outsiderId = (string)null,
                        firstName = r.Name,
                        middleName = (string)null, // cant get middle
                        lastName = r.Surname, 
                        email = r.Email, 
                        organization = (string)null,  // cant get 
                        country = (string)null,
                        hasAccount = true
                    }).First();
                else if (dbContext.Outsiders.Any(o => o.Email == email))
                    return dbContext.Outsiders.Where(us => us.Email == email).Select(r => new
                    {
                        outsiderId = r.Id,
                        userId = (string)null,
                        firstname = r.FirstName,
                        middlename = r.MiddleName,
                        lastname = r.LastName,
                        email = r.Email,
                        organization = r.Organization,
                        country = r.Country,
                        hasAccount = false
                    }).First();

                return (string)null;
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][SearchOutsiderByEmail] " + ex.Message, ex);
            }
        }

    }
}