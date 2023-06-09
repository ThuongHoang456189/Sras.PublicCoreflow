using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
using Sras.PublicCoreflow.Domain.ConferenceManagement;
using Sras.PublicCoreflow.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Guids;

namespace Sras.PublicCoreflow.EntityFrameworkCore.ConferenceManagement
{
    public class EmailTemplateRepository : EfCoreRepository<PublicCoreflowDbContext, EmailTemplate, Guid>, IEmailTemplateRepository
    {
        private readonly IPlaceHolderRepository _placeHolderRepository;
        public EmailTemplateRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator, IPlaceHolderRepository placeHolderRepository) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
            _placeHolderRepository = placeHolderRepository;
        }

        private readonly IGuidGenerator _guidGenerator;

        public async Task<object> GetEmailTemplateById(Guid id)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.EmailTemplates.Any(et => et.Id == id)) {
                    throw new Exception("EmailTemplateId not Found");
                } else
                {
                    var queryResult = await dbContext.EmailTemplates.FindAsync(id);
                    return new
                    {
                        subject = queryResult.Subject,
                        body = queryResult.Body
                    };
                }
            } catch (Exception ex)
            {
                throw new Exception("[Repository] GetEmailTemplateById : " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetEmailTemplateByConferenceId(Guid conferenceId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Conferences.Any(c => c.Id == conferenceId))
                {
                    throw new Exception("ConferenceId not exist in DB");
                } else if (!dbContext.EmailTemplates.Any(emt => emt.ConferenceId == conferenceId))
                {
                    return new List<string>();
                }

                var result = dbContext.EmailTemplates.Where(e => e.ConferenceId == conferenceId).Select(em => new
                {
                    templateId = em.Id,
                    templateName = "haha"
                });
                return result;
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][GetEmailTemplateByConferenceId] " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetEmailTemplateByConferenceIdAndTrackId(Guid conferenceId, Guid? trackId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var finalTemplateList = new List<object>();
                if (!dbContext.Tracks.Any(c => c.Id == trackId) || !dbContext.Conferences.Any(cf => cf.Id == conferenceId))
                {
                    throw new Exception("TrackId Or ConferenceId not exist in DB");
                }

                var templateOfTrack = dbContext.EmailTemplates
                    .Where(e => e.ConferenceId != null)
                    .Where(e => e.TrackId == trackId)
                    .ToList();
                var templateOfConference = dbContext.EmailTemplates
                    .Where(em => em.ConferenceId != null && em.TrackId == null)
                    .Where(et => et.ConferenceId == conferenceId)
                    .ToList();
                templateOfTrack.AddRange(templateOfConference);
                var totalTemplate = templateOfTrack.Select(r => new
                {
                    TemplateId = r.Id,
                    TemplateName = r.Name
                });
                return totalTemplate;
            }
            catch (Exception ex)
            {
                throw new Exception("[ERROR][GetEmailTemplateByConferenceId] " + ex.Message, ex);
            }
        }

        public async Task<object> GetEmailSendEachStatus(PaperStatusToEmail request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                var sender = (await dbContext.Users.FindAsync(request.userId));
                var emailSender = sender.Email;
                var submission = dbContext.Submissions.Where(s => s.TrackId == request.trackId);
                request.statuses.Select(st =>
                {
                    var conference = dbContext.Tracks.Where(t => t.Id == request.trackId).First().Conference;
                    var template = dbContext.EmailTemplates.FindAsync(st.templateId).Result;
                    var placeHoldersContainInSubject = dbContext.SupportedPlaceholders.Where(sp => template.Subject.Contains(sp.Encode));
                    var placeHoldersContainInBody = dbContext.SupportedPlaceholders.Where(sp => template.Body.Contains(sp.Encode));
                    var authors = submission.Where(ss => ss.StatusId == st.paperStatusId)
                    .SelectMany(ss => ss.Authors);
                    authors.Select(au => 
                    {
                        var pa = au.Participant;
                        if (pa.Outsider != null)
                        {
                            RecipientInforForEmail recipient = new RecipientInforForEmail(pa.Outsider.FirstName, pa.Outsider.LastName, pa.Outsider.FirstName + " " + pa.Outsider.MiddleName + " " + pa.Outsider.FirstName, pa.Outsider.Email, pa.Outsider.Organization);
                            var subjectString = template.Subject.ToString();
                            var bodyString = template.Body.ToString();
                            return new
                            {
                                from = emailSender,
                                to = pa.Outsider.Email,
                                subject = placeHoldersContainInSubject.Select(pl => pl.Encode).ToList()
                            .Aggregate(subjectString, (subject, p) => subject.Replace(p, _placeHolderRepository.GetDataFromPlaceholder(p, conference, recipient, au.Submission, sender))),
                                body = placeHoldersContainInBody
                            };
                        }
                    });
                    
                    return authors.
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        

    }
}
