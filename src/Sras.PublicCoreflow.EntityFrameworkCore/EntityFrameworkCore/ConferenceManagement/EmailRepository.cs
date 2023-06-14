﻿using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class EmailRepository : EfCoreRepository<PublicCoreflowDbContext, Email, Guid>, IEmailRepository
    {
        private IPlaceHolderRepository _placeHolderRepository;
        private IOutsiderRepository _outsiderRepository;
        private IGuidGenerator _guidGenerator;
        public EmailRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IPlaceHolderRepository placeHolderRepository, IGuidGenerator guidGenerator, IOutsiderRepository outsiderRepository) : base(dbContextProvider)
        {
            _placeHolderRepository = placeHolderRepository;
            _guidGenerator = guidGenerator;
            _outsiderRepository = outsiderRepository;
        }

        public async Task<object> SendEmailForEachStatus(PaperStatusToEmail request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var sender = await dbContext.Users.FindAsync(request.userId);
                var emailSender = sender.Email;
                var submissions = await dbContext.Submissions
                    .Include(s => s.Authors)
                    .ThenInclude(a => a.Participant)
                    .ThenInclude(p => p.Outsider)
                    .Include(s => s.Authors).ThenInclude(a => a.Participant).ThenInclude(p => p.Account)
                    .Where(s => s.TrackId == request.trackId).ToListAsync();
                dbContext.Authors.Include(a => a.Participant);
                return request.statuses.Select(st =>
                {
                    var conference = dbContext.Tracks.Include(t => t.Conference).Where(t => t.Id == request.trackId).First().Conference;
                    var template = dbContext.EmailTemplates.Find(st.templateId);
                    var placeHoldersContainInSubject = dbContext.SupportedPlaceholders.Where(sp => template.Subject.Contains(sp.Encode)).ToList();
                    var placeHoldersContainInBody = dbContext.SupportedPlaceholders.Where(sp => template.Body.Contains(sp.Encode)).ToList();

                    if (request.allAuthors)
                    {
                        submissions.Where(ss => ss.StatusId == st.statusId).ToList().ForEach(su => su.IsNotified = true);
                        return new
                        {
                            statusId = st.statusId,
                            name = dbContext.PaperStatuses.Where(p => p.Id == st.statusId).First().Name,
                            sendEmails = submissions
                        .Where(ss => ss.StatusId == st.statusId)
                        .SelectMany(ss => ss.Authors)
                        .Select((au, index) =>
                        {
                            var pa = au.Participant;
                            RecipientInforForEmail recipient;
                            Guid recipientId = Guid.Empty;
                            string subjectString = template.Subject.ToString();
                            string bodyString = template.Body.ToString();

                            if (pa.Outsider != null)
                            {
                                recipient = new RecipientInforForEmail(pa.Outsider.FirstName, pa.Outsider.LastName, pa.Outsider.LastName + " " + pa.Outsider.MiddleName + " " + pa.Outsider.FirstName, pa.Outsider.Email, pa.Outsider.Organization);
                                recipientId = (Guid)pa.OutsiderId;
                            }
                            else
                            {
                                recipient = new RecipientInforForEmail(pa.Account.Name, pa.Account.Surname, pa.Account.Surname + " " + pa.Account.Name, pa.Account.Email, pa.Account.GetProperty<string?>("Organization"));
                                recipientId = (Guid)pa.AccountId;
                            }

                            var subject = placeHoldersContainInSubject.Select(pl => pl.Encode).ToList()
                                .Aggregate(subjectString, (subject, p) => subject.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var body = placeHoldersContainInBody.Select(pl => pl.Encode).ToList()
                                .Aggregate(bodyString, (body, p) => body.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var emailId = _guidGenerator.Create();
                            dbContext.Emails.Add(new Email(emailId, sender.Id, recipientId, subject, body, template.Id));
                            dbContext.SaveChanges();
                            return new
                            {
                                id = index,
                                fromName = sender.Name,
                                fromEmail = emailSender,
                                toFullName = recipient.FullName,
                                toEmail = recipient.Email,
                                subject = subject,
                                body = body
                            };
                        })
                        };
                    }
                    else
                    {
                        submissions.Where(ss => ss.StatusId == st.statusId).Where(su => su.Authors.Any(aus => aus.IsPrimaryContact)).ToList().ForEach(su => su.IsNotified = true);
                        return new
                        {
                            statusId = st.statusId,
                            name = dbContext.PaperStatuses.Where(p => p.Id == st.statusId).First().Name,
                            sendEmails = submissions
                        .Where(ss => ss.StatusId == st.statusId)
                        .SelectMany(ss => ss.Authors)
                        .Where(aus => aus.IsPrimaryContact)
                        .Select((au, index) =>
                        {
                            var pa = au.Participant;
                            RecipientInforForEmail recipient;
                            Guid recipientId = Guid.Empty;
                            string subjectString = template.Subject.ToString();
                            string bodyString = template.Body.ToString();

                            if (pa.Outsider != null)
                            {
                                recipient = new RecipientInforForEmail(pa.Outsider.FirstName, pa.Outsider.LastName, pa.Outsider.LastName + " " + pa.Outsider.MiddleName + " " + pa.Outsider.FirstName, pa.Outsider.Email, pa.Outsider.Organization);
                                recipientId = (Guid)pa.OutsiderId;
                            }
                            else
                            {
                                recipient = new RecipientInforForEmail(pa.Account.Name, pa.Account.Surname, pa.Account.Surname + " " + pa.Account.Name, pa.Account.Email, pa.Account.GetProperty<string?>("Organization"));
                                recipientId = (Guid)pa.AccountId;
                            }

                            var subject = placeHoldersContainInSubject.Select(pl => pl.Encode).ToList()
                                .Aggregate(subjectString, (subject, p) => subject.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var body = placeHoldersContainInBody.Select(pl => pl.Encode).ToList()
                                .Aggregate(bodyString, (body, p) => body.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var emailId = _guidGenerator.Create();
                            dbContext.Emails.Add(new Email(emailId, sender.Id, recipientId, subject, body, template.Id));
                            dbContext.SaveChanges();

                            return new
                            {
                                id = index,
                                fromName = sender.Name,
                                fromEmail = emailSender,
                                toFullName = recipient.FullName,
                                toEmail = recipient.Email,
                                subject = subject,
                                body = body
                            };
                        })
                        };
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
