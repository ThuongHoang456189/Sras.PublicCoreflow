using AutoMapper.Internal.Mappers;
using Sras.PublicCoreflow.ConferenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using Sras.PublicCoreflow.Extension;
using Sras.PublicCoreflow.Dto;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class PlaceHolderRepository : EfCoreRepository<PublicCoreflowDbContext, SupportedPlaceholder, Guid>, IPlaceHolderRepository
    {

        public PlaceHolderRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public string GetDataFromPlaceholder(string placeHolderName, Conference? conference, RecipientInforForEmail recipient, Submission? submission, IdentityUser sender)
        {
            switch (placeHolderName)
            {
                case "{Conference.Name}":
                    return conference?.FullName;
                case "{Submission.StatusName}":
                    return submission?.Status.Name;
                case "{Submission.TrackName}":
                    return submission?.Track.Name;
                case "{Recipient.LastName}":
                    return recipient.LastName;
                    //return recipient.Surname;
                case "{Submission.Id}":
                    return submission?.Id.ToString();
                case "{Conference.City}":
                    return conference?.City;
                case "{Recipient.Email}":
                    return recipient.Email;
                    //return recipient?.Email;
                case "{Conference.Country}":
                    return conference?.Country;
                case "{Recipient.FirstName}":
                    return recipient.FirstName;
                    //return recipient?.Name;
                case "{Recipient.Name}":
                    return recipient.FullName;
                    //return recipient?.Surname + " " + recipient?.GetProperty<string?>("MiddleName") + " " + recipient?.Name;
                case "{Conference.StartDate}":
                    return conference?.StartDate.ToString();
                case "{Sender.LastName}":
                    return sender?.Surname;
                case "{Conference.EndDate}":
                    return conference?.EndDate.ToString();
                case "{Submission.PrimarySubjectArea.Name}":
                    return submission?.SubjectAreas.Where(sa => sa.IsPrimary).First().SubjectArea.Name;
                case "{Submission.UpdateDate}":
                    return submission?.LastModificationTime.ToString();
                case "{Sender.Email}":
                    return sender?.Email;
                case "{Submission.Title}":
                    return submission?.Title;
                case "{Sender.Organization}":
                    return sender?.GetProperty<string?>("Organization");
                case "{Recipient.Organization}":
                    return recipient.Organization;
                    //return recipient?.GetProperty<string?>("Organization");
                case "{Sender.FirstName}":
                    return sender?.Name;
                case "{Sender.Name}":
                    return sender?.Surname + " " + sender?.GetProperty<string?>("MiddleName") + " " + sender?.Name;
                case "{Submission.Abstract}":
                    return submission?.Abstract;
                case "{Submission.CreateDate}":
                    return submission?.CreationTime.ToString();
                default:
                    return string.Empty;
            }
        }


    }
}
