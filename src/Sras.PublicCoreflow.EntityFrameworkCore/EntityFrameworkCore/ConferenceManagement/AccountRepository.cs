using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Volo.Abp.Identity;
using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.Dto;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Data;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class AccountRepository : EfCoreRepository<PublicCoreflowDbContext, IdentityUser, Guid>, IAccountRepository
    {
        private readonly IGuidGenerator _guidGenerator;
        public AccountRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public void AddAccount(RegisterAccountRequest registerAccount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RegisterAccountRequest> GetAllAccount()
        {
            var dbContext = GetDbContextAsync().Result;
            return dbContext.Users.Select(u => new RegisterAccountRequest()
            {
                Id = u.Id,
                Email = u.Email,
                Firstname = u.Name,
                Lastname = u.Surname,
                Organization = "",                
                Country = ""
            });
        }

        public bool IsEmailDuplicate(string email)
        {
            var dbContext = GetDbContextAsync().Result;
            //var acc = dbContext.Users.Where(a => a.)
            return true;
        }

        public bool UpdateAccount(RegisterAccountRequest registerAccount)
        {
            var dbContext = GetDbContextAsync().Result;
            if (!dbContext.Users.Any(u => u.Id == registerAccount.Id)) { return false; }
            else
            {
                var user = dbContext.Users.FindAsync(registerAccount.Id).Result;
                user.Name = registerAccount.Firstname;
                user.Surname = registerAccount.Lastname;
                user.SetProperty(AccountConsts.MiddleNamePropertyName, registerAccount.MiddleName);
                user.SetProperty(AccountConsts.OrganizationPropertyName, registerAccount.Organization);
                user.SetProperty(AccountConsts.CountryPropertyName, registerAccount.Country);
                if (!dbContext.Participants.Any(p => p.AccountId == user.Id))
                {
                    dbContext.Participants.Add(new Participant(_guidGenerator.Create(), user.Id, null));
                }
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool ConfirmEmail(Guid id)
        {
            var dbContext = GetDbContextAsync().Result;
            if (!dbContext.Users.Any(u => u.Id == id)) { return false; }
            else
            {
                var user = dbContext.Users.FindAsync(id).Result;
                user.SetEmailConfirmed(true);
                dbContext.SaveChanges();
                return true;
            }
        }

        public bool isConfirmAccount(Guid id)
        {
            var dbContext = GetDbContextAsync().Result;
            if (!dbContext.Users.Any(u => u.Id != id)) { throw new Exception("Account Not Found"); };
            return dbContext.Users.Where(u => u.Id == id).First().EmailConfirmed;
        }
    }
}
