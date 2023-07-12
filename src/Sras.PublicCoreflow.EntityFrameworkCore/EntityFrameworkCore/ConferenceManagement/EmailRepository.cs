using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

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

        public async Task<string> SendEmailAsync(string toEmails, string body, string subject)
        {
            var email = new MimeMessage();
            //email.From.Add(MailboxAddress.Parse("fptsciencemanagement@gmail.com"));
            email.From.Add(MailboxAddress.Parse("abcder"));
            email.To.Add(MailboxAddress.Parse(toEmails));
            email.Subject = subject;
            //body = await File.ReadAllTextAsync("B:\\WorkingSet CODE\\VisualStudio\\ScienceManagement\\Service\\Programming.html");
            email.Body = new TextPart(TextFormat.Html) { Text = body };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("fptsciencemanagement@gmail.com", "xvddaoqsyumnascw");
            var result = smtp.Send(email);
            await smtp.DisconnectAsync(false);

            return result;
        }

        public async Task<object> SendEmailForEachStatus(PaperStatusToSendEmail request)
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
                    
                    var conferenceId = dbContext.Tracks.Where(t => t.Id == request.trackId).First().Conference.Id;
                    var conferenceAccId = dbContext.ConferenceAccounts.Where(t => t.ConferenceId == conferenceId).Where(cc => cc.AccountId == sender.Id).First().Id;
                    var incumbentSenderId = _guidGenerator.Create();
                    if (dbContext.ConferenceRoles.Where(c => c.Id == request.conferenceRoleId).First().Name == "Chair")
                    {
                        incumbentSenderId = dbContext.Incumbents
                            .Where(i => i.ConferenceAccountId == conferenceAccId)
                            .Where(ii => ii.ConferenceRoleId == request.conferenceRoleId)
                            .First().Id;
                    } else
                    {
                        incumbentSenderId = dbContext.Incumbents
                            .Where(i => i.ConferenceAccountId == conferenceAccId)
                            .Where(ii => ii.ConferenceRoleId == request.conferenceRoleId)
                            .Where(iii => iii.TrackId == request.trackId)
                            .First().Id;
                    }
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
                                recipientId = (Guid)pa.Id;
                            }
                            else
                            {
                                recipient = new RecipientInforForEmail(pa.Account.Name, pa.Account.Surname, pa.Account.Surname + " " + pa.Account.Name, pa.Account.Email, pa.Account.GetProperty<string?>("organization"));
                                recipientId = (Guid)pa.Id;
                            }

                            var subject = placeHoldersContainInSubject.Select(pl => pl.Encode).ToList()
                                .Aggregate(subjectString, (subject, p) => subject.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var body = placeHoldersContainInBody.Select(pl => pl.Encode).ToList()
                                .Aggregate(bodyString, (body, p) => body.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var emailId = _guidGenerator.Create();

                            this.SendEmailAsync(recipient.Email, body, subject);
                            dbContext.Emails.Add(new Email(emailId, incumbentSenderId, recipientId, subject, body, template.Id));
                            dbContext.SaveChanges();

                            return new
                            {
                                id = index,
                                fromName = sender.Name,
                                fromEmail = emailSender,
                                toFullName = recipient.FullName,
                                toEmail = recipient.Email,
                                subject = subject,
                                body = body,
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
                                recipientId = (Guid)pa.Id;
                            }
                            else
                            {
                                recipient = new RecipientInforForEmail(pa.Account.Name, pa.Account.Surname, pa.Account.Surname + " " + pa.Account.Name, pa.Account.Email, pa.Account.GetProperty<string?>("organization"));
                                recipientId = (Guid)pa.Id;
                            }

                            var subject = placeHoldersContainInSubject.Select(pl => pl.Encode).ToList()
                                .Aggregate(subjectString, (subject, p) => subject.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var body = placeHoldersContainInBody.Select(pl => pl.Encode).ToList()
                                .Aggregate(bodyString, (body, p) => body.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender)));

                            var emailId = _guidGenerator.Create();
                            this.SendEmailAsync(recipient.Email, body, subject);
                            dbContext.Emails.Add(new Email(emailId, incumbentSenderId, recipientId, subject, body, template.Id));
                            dbContext.SaveChanges();

                            return new
                            {
                                id = index,
                                fromName = sender.Name,
                                fromEmail = emailSender,
                                toFullName = recipient.FullName,
                                toEmail = recipient.Email,
                                subject = subject,
                                body = body,
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
