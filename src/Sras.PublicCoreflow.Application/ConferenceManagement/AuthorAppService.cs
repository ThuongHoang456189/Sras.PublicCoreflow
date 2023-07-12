using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class AuthorAppService : PublicCoreflowAppService, IAuthorAppService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ISubmissionRepository _submissionRepository;

        private const string BlobRoot = "host";
        private const string SubmissionBlobRoot = "sras-submissions";
        private const string SupplementaryMaterialBlobRoot = "sras-supplementary-materials";
        private const string RevisionBlobRoot = "sras-revisions";
        private const string CameraReadyBlobRoot = "sras-camera-readies";
        private const string CopyRightBlobRoot = "sras-copyrights";
        private const string PresentationBlobRoot = "sras-presentations";

        public AuthorAppService(IAuthorRepository authorRepository, ISubmissionRepository submissionRepository)
        {
            _authorRepository = authorRepository;
            _submissionRepository = submissionRepository;
        }

        public async Task<PagedResultDto<AuthorSubmission>> GetListAuthorAggregation(Guid accountId, Guid conferenceId, string sorting = AuthorConsts.DefaultSorting, int skipCount = 0, int maxResultCount = AuthorConsts.DefaultMaxResultCount)
        {
            var count = await _authorRepository.GetCountAuthorAggregation(accountId, conferenceId);

            var items = await _authorRepository.GetListAuthorAggregation(accountId, conferenceId, sorting, skipCount, maxResultCount);

            return new PagedResultDto<AuthorSubmission>(count, items);
        }

        private List<string>? GetSubmissionFiles(string? submissionRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(submissionRootFilePath))
                return null;

            var submissionPath = string.Join("/", BlobRoot, SubmissionBlobRoot, submissionRootFilePath);

            try
            {
                return Directory.GetFiles(submissionPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetSupplementaryMaterialFiles(string? supplementaryMaterialRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(supplementaryMaterialRootFilePath))
                return null;

            var supplementaryMaterialPath = string.Join("/", BlobRoot, SupplementaryMaterialBlobRoot, supplementaryMaterialRootFilePath);

            try
            {
                return Directory.GetFiles(supplementaryMaterialPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetRevisionFiles(string? revisionRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(revisionRootFilePath))
                return null;

            var revisionPath = string.Join("/", BlobRoot, RevisionBlobRoot, revisionRootFilePath);

            try
            {
                return Directory.GetFiles(revisionPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetCameraReadyFiles(string? cameraReadyRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(cameraReadyRootFilePath))
                return null;

            var cameraReadyPath = string.Join("/", BlobRoot, CameraReadyBlobRoot, cameraReadyRootFilePath);

            try
            {
                return Directory.GetFiles(cameraReadyPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetCopyRightFiles(string? copyRightFilePath)
        {
            if (string.IsNullOrWhiteSpace(copyRightFilePath))
                return null;

            var copyRightPath = string.Join("/", BlobRoot, CopyRightBlobRoot, copyRightFilePath);

            try
            {
                return Directory.GetFiles(copyRightPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetPresentationFiles(string? presentationRootFilePath)
        {
            if (string.IsNullOrWhiteSpace(presentationRootFilePath))
                return null;

            var presentationPath = string.Join("/", BlobRoot, PresentationBlobRoot, presentationRootFilePath);

            try
            {
                return Directory.GetFiles(presentationPath).Select(x => Path.GetFileName(x)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string>? GetListAction(string? actionStr)
        {
            if (string.IsNullOrWhiteSpace(actionStr))
                return null;

            return actionStr.Split('|').ToList();
        }

        public async Task<PagedResultDto<AuthorSubmissionAggregationDto>?> GetAuthorSubmissionAggregationAsync(AuthorSubmissionAggregationInput input)
        {
            var foundItems = await _submissionRepository.GetAuthorSubmissionAggregationAsync
                (
                    input.InclusionText,
                    input.ConferenceId,
                    input.TrackId,
                    input.AccountId,
                    input.StatusId,
                    input.Sorting,
                    input.SortedAsc,
                    input.SkipCount == null ? 0 : input.SkipCount.Value,
                    input.MaxResultCount == null ? PublicCoreflowConsts.DefaultMaxResultCount : input.MaxResultCount.Value
                );

            // process output
            if(foundItems == null || foundItems.Count == 0)
                return null;

            var count = (long) foundItems.First().TotalCount.Value;

            List<AuthorSubmissionAggregationDto> items = new List<AuthorSubmissionAggregationDto>();

            foundItems.ForEach(x =>
            {
                items.Add(new AuthorSubmissionAggregationDto()
                {
                    SubmissionId = x.Id,
                    Title = x.Title,
                    TrackId = x.TrackId,
                    TrackName = x.TrackName,
                    Files = new SubmissionRelatedFilesDto()
                    {
                        SubmissionFiles = GetSubmissionFiles(x.SubmissionRootFilePath),
                        SupplementaryMaterialFiles = GetSupplementaryMaterialFiles(x.SupplementaryMaterialRootFilePath),
                        RevisionNo = x.CloneNo,
                        RevisionFiles = GetRevisionFiles(x.RevisionRootFilePath),
                        CameraReadyFiles = GetCameraReadyFiles(x.CameraReadyRootFilePath),
                        CopyRightFiles = GetCopyRightFiles(x.CopyRightFilePath),
                        PresentationFiles = GetPresentationFiles(x.PresentationRootFilePath)
                    },
                    StatusId = x.StatusId,
                    Status = x.Status,
                    Actions = GetListAction(x.Actions)
                });
            });

            return new PagedResultDto<AuthorSubmissionAggregationDto>(count, items);
        }
    }
}
