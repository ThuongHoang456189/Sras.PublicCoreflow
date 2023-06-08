using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Users;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerAppService : PublicCoreflowAppService, IReviewerAppService
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IReviewerSubjectAreaRepository _reviewerSubjectAreaRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public ReviewerAppService(
            IReviewerRepository reviewerRepository,
            IReviewerSubjectAreaRepository reviewerSubjectAreaRepository,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator)
        {
            _reviewerRepository = reviewerRepository;
            _reviewerSubjectAreaRepository = reviewerSubjectAreaRepository;

            _currentUser = currentUser;
            _guidGenerator = guidGenerator;
        }

        public async Task<ResponseDto> UpdateReviewerQuota(ReviewerQuotaInput input)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                var reviewer = await _reviewerRepository.UpdateReviewerQuota(input.AccountId, input.ConferenceId, input.TrackId, input.Quota);

                if(reviewer == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Update failed";
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "Update successfully";
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }

        public async Task<ResponseDto> UpdateReviewerSubjectArea(ReviewerSubjectAreaInput input)
        {
            ResponseDto response = new ResponseDto();

            // Get reviewer
            var reviewer = await _reviewerRepository.FindAsync(input.AccountId, input.ConferenceId, input.TrackId);
            if (reviewer == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);

            var reviewerSubjectAreas = await _reviewerSubjectAreaRepository.GetListAsync(x => x.ReviewerId == reviewer.Id);

            if (!reviewer.SubjectAreas.Any())
            {
                reviewerSubjectAreas.ForEach(x =>
                {
                    reviewer.SubjectAreas.Add(x);
                });
            }

            if (reviewer == null)
                throw new BusinessException(PublicCoreflowDomainErrorCodes.ReviewerNotFound);
            // Clean input

            try
            {
                // Allocate operation
                var reviewerSubjectAreaOperationTable = await _reviewerSubjectAreaRepository.GetReviewerSubjectAreaOperationTableAsync(reviewer.Id);
                reviewerSubjectAreaOperationTable.ForEach(x =>
                {
                    if (!input.SubjectAreas.Any(y => y.SubjectAreaId == x.SubjectAreaId))
                    {
                        x.Operation = ReviewerSubjectAreaManipulationOperators.Del;
                    }
                    else
                    {
                        var update = input.SubjectAreas.Find(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (update != null)
                        {
                            if (update.IsPrimary != x.IsPrimary)
                            {
                                x.IsPrimary = update.IsPrimary;
                                x.Operation = ReviewerSubjectAreaManipulationOperators.Up;
                            }
                        }
                    }
                });

                input.SubjectAreas.ForEach(x =>
                {
                    if (!reviewerSubjectAreaOperationTable.Any(y => y.SubjectAreaId == x.SubjectAreaId))
                    {
                        ReviewerSubjectAreaOperation newOperation = new ReviewerSubjectAreaOperation
                        {
                            ReviewerId = reviewer.Id,
                            SubjectAreaId = x.SubjectAreaId,
                            IsPrimary = x.IsPrimary,
                            Operation = ReviewerSubjectAreaManipulationOperators.Add
                        };

                        reviewerSubjectAreaOperationTable.Add(newOperation);
                    }
                });

                // Perform operation
                reviewerSubjectAreaOperationTable.ForEach(x =>
                {
                    if (x.Operation == ReviewerSubjectAreaManipulationOperators.Add)
                    {
                        ReviewerSubjectArea newSubjectArea = new ReviewerSubjectArea(_guidGenerator.Create(), x.ReviewerId, x.SubjectAreaId, x.IsPrimary);
                        reviewer.SubjectAreas.Add(newSubjectArea);
                    }
                    else if (x.Operation == ReviewerSubjectAreaManipulationOperators.Del)
                    {
                        var foundSubjectArea = reviewer.SubjectAreas.FirstOrDefault(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (foundSubjectArea != null)
                        {
                            reviewer.SubjectAreas.Remove(foundSubjectArea);
                        }
                    }
                    else if (x.Operation == ReviewerSubjectAreaManipulationOperators.Up)
                    {
                        var foundSubjectArea = reviewer.SubjectAreas.FirstOrDefault(y => y.SubjectAreaId == x.SubjectAreaId);
                        if (foundSubjectArea != null)
                        {
                            foundSubjectArea.IsPrimary = x.IsPrimary;
                        }
                    }
                });

                await _reviewerRepository.UpdateAsync(reviewer);

                response.IsSuccess = true;
                response.Message = "Update successfully";
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                response.IsSuccess = false;
                response.Message = "Exception";
            }

            return await Task.FromResult(response);
        }
    }
}