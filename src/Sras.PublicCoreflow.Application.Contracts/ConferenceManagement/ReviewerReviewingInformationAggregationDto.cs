using Volo.Abp.Application.Dtos;

namespace Sras.PublicCoreflow.ConferenceManagement
{
    public class ReviewerReviewingInformationAggregationDto
    {
        public ReviewerBriefInformationDto? Reviewer { get; set; }
        public PagedResultDto<ReviewingInformationAggregationDto>? ReviewingFacts { get; set; }
    }
}
