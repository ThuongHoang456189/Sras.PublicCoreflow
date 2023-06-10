using Microsoft.EntityFrameworkCore;
using Sras.PublicCoreflow.ConferenceManagement;
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
    public class SubmissionRepository : EfCoreRepository<PublicCoreflowDbContext, Submission, Guid>, ISubmissionRepository
    {
        private readonly IGuidGenerator _guidGenerator;

        public SubmissionRepository(IDbContextProvider<PublicCoreflowDbContext> dbContextProvider, IGuidGenerator guidGenerator) : base(dbContextProvider)
        {
            _guidGenerator = guidGenerator;
        }

        public async Task<object> GetNumberOfSubmission(Guid trackId)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Submissions.Any(s => s.TrackId == trackId))
                {
                    throw new Exception("There Is no Submission for TrackID=" + trackId);
                }
                var totalSubmission = dbContext.Submissions.Where(s => s.TrackId == trackId).Count();
                return new
                {
                    numOfSubmission = totalSubmission
                };
            } catch (Exception ex)
            {
                throw new Exception("[ERROR][GetNumberOfSubmission] " + ex.Message, ex);
            }
        }

        public async Task<object> GetNumOfSubmissionAndEmailWithAllAuthor(SubmissionWithEmailRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Tracks.Any(t => t.Id == request.TrackId))
                    throw new Exception("TrackId not existing");
                if (!request.PaperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.PaperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.TrackId && s.StatusId == ps);
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        statusName = statusName,
                        numberSubmission = submissionOfOneStatus,
                        numberEmailSend = numOfAllAuthor
                    };
                });

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<object> GetNumOfSubmissionAndEmailWithPrimaryContactAuthor(SubmissionWithEmailRequest request)
        {
            try
            {
                var dbContext = await GetDbContextAsync();
                if (!dbContext.Tracks.Any(t => t.Id == request.TrackId))
                    throw new Exception("TrackId not existing");
                if (!request.PaperStatuses.All(ps => dbContext.PaperStatuses.Any(statusDB => statusDB.Id == ps)))
                {
                    throw new Exception("One Of Paper Status Id not existing");
                }
                var result = request.PaperStatuses.Select(ps =>
                {
                    var submissions = dbContext.Submissions.Where(s => s.TrackId == request.TrackId && s.StatusId == ps);
                    var numOfPrimaryAuthor = submissions.SelectMany(ss => ss.Authors).Where(au => au.IsPrimaryContact).Count();
                    var numOfAllAuthor = submissions.SelectMany(s => s.Authors).Count();
                    var submissionOfOneStatus = dbContext.Submissions.Where(s => s.StatusId == ps).ToList();
                    var statusName = dbContext.PaperStatuses.Where(p => p.Id == ps).First().Name;
                    return new
                    {
                        statusId = ps,
                        statusName = statusName,
                        numberSubmission = submissionOfOneStatus,
                        numberEmailSend = numOfPrimaryAuthor,
                    };
                });

                return result;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<object>> GetSubmissionAsync()
        {
            var dbContext = await GetDbContextAsync();
            return dbContext.Submissions.Select(s => new
            {
                Id = s.Id,
                Title = s.Title,
                TrackId = s.TrackId,
                StatusId = s.StatusId,
                isNotified = s.IsNotified,
                NotifiedStatusId = s.NotifiedStatusId,
                NotifiedStatusName = s.NotifiedStatus.Name
            }).ToList();
        }
    }
}
