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
        private readonly IIncumbentRepository _incumbentRepository;

        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public ReviewerAppService(
            IReviewerRepository reviewerRepository,
            IIncumbentRepository incumbentRepository,
            ICurrentUser currentUser,
            IGuidGenerator guidGenerator)
        {
            _reviewerRepository = reviewerRepository;
            _incumbentRepository = incumbentRepository;
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
    }
}
