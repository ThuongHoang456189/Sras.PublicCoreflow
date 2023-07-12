using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration48 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getSubmissionReviewerAssignmentSuggestionSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetSubmissionReviewerAssignmentSuggestion]
			@UTCNowStr nvarchar(20),
			@InclusionText nvarchar(1024),
			@SubmissionId uniqueidentifier,
			@IsAssigned bit
			AS
			BEGIN

			declare 
				@ConferenceId uniqueidentifier,
				@TrackId uniqueidentifier,
				@Today datetime2(7),
				@UTCNow datetime2(7),
				@LocalTimeZoneId nvarchar(512),
				@LocalNow datetime2(7)

			select 
				@ConferenceId = Tracks.ConferenceId 
			from Submissions
			join Tracks on Submissions.TrackId = Tracks.Id
			where Submissions.Id = @SubmissionId and Submissions.IsDeleted = 'false'
			and Tracks.IsDeleted = 'false'

			select @UTCNow = convert(datetime2(7), @UTCNowStr, 126)

			select @LocalTimeZoneId = Conferences.TimeZoneId
			from Conferences
			where Conferences.Id = @ConferenceId
			and Conferences.IsDeleted = 'false'

			select @LocalNow = cast((@UTCNow at time zone 'UTC') at time zone @LocalTimeZoneId as datetime2(7))

			select @Today = cast(cast(@LocalNow as date) as datetime2(7))

			select 
				@TrackId = Submissions.TrackId 
			from Submissions
			join Tracks on Submissions.TrackId = Tracks.Id
			where Submissions.Id = @SubmissionId and Submissions.IsDeleted = 'false'
			and Tracks.IsDeleted = 'false'

			-- for select
			select 
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.ReviewerId,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.FullName,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.FirstName,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.MiddleName,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.LastName,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.Email,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.Organization,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.SelectedSubmissionConflicts,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.SelectedReviewerConflicts,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.SelectedReviewerSubjectAreas,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.Quota,
				SelectedInfoPartWithInclusionTextWithConflictsReviewer.IsAssigned,
				SelectedNumberOfAssignments.NumberOfAssignments
			from
			(
				-- InfoPart with Inclusion Text and Conflicts
				select 
					SelectedInfoPartWithInclusionTextReviewer.ReviewerId,
					SelectedInfoPartWithInclusionTextReviewer.FullName,
					SelectedInfoPartWithInclusionTextReviewer.FirstName,
					SelectedInfoPartWithInclusionTextReviewer.MiddleName,
					SelectedInfoPartWithInclusionTextReviewer.LastName,
					SelectedInfoPartWithInclusionTextReviewer.Email,
					SelectedInfoPartWithInclusionTextReviewer.Organization,
					stuff(
					(select ';' + 
						(
							ConflictCases.Name
						) as 'SubmissionConflict'
						from Conflicts
						join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
						where SelectedInfoPartWithInclusionTextReviewer.ReviewerId = Conflicts.IncumbentId
						and Conflicts.IsDeleted = 'false'
						and Conflicts.IsDefinedByReviewer = 'false'
						and ConflictCases.IsDeleted = 'false'
						order by SubmissionConflict asc
						for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
					) as 'SelectedSubmissionConflicts',
					stuff(
					(select ';' + 
						(
							ConflictCases.Name
						) as 'ReviewerConflict'
						from Conflicts
						join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
						where SelectedInfoPartWithInclusionTextReviewer.ReviewerId = Conflicts.IncumbentId
						and Conflicts.IsDeleted = 'false'
						and Conflicts.IsDefinedByReviewer = 'true'
						and ConflictCases.IsDeleted = 'false'
						order by ReviewerConflict asc
						for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
					) as 'SelectedReviewerConflicts',
					SelectedInfoPartWithInclusionTextReviewer.SelectedReviewerSubjectAreas,
					SelectedInfoPartWithInclusionTextReviewer.Quota,
					SelectedInfoPartWithInclusionTextReviewer.IsAssigned
				from
				(
					-- InfoPart with Inclusion Text
					select SelectedInfoPartReviewer.*
					from
					(
						-- InfoPart
						select 
							SelectedReviewingWithReviewAssignmentSubmissions.ReviewerId,
							SelectedReviewers.FullName,
							SelectedReviewers.Name as 'FirstName',
							SelectedReviewers.MiddleName,
							SelectedReviewers.Surname as 'LastName',
							SelectedReviewers.Email,
							SelectedReviewers.Organization,
							SelectedReviewers.SelectedReviewerSubjectAreas,
							SelectedReviewers.Quota,
							SelectedReviewingWithReviewAssignmentSubmissions.IsAssigned
						from
						(
							select 
								SelectedReviewingSubmissions.*,
								ReviewAssignments.Id as 'ReviewAssignmentId',
								ReviewAssignments.ReviewerId,
								ReviewAssignments.IsActive as 'IsAssigned'
							from
							(
								select SelectedWithLatestSubmissionCloneSameTrackSubmissions.*
								from
								(
									select 
										SelectedSameTrackSubmissions.*,
										SelectedLatestSubmissionClones.*
									from
									(
										-- Found submission in the same track as 
										select Submissions.Id, ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
										from Submissions
										join Tracks on Submissions.TrackId = Tracks.Id
										left join ActivityDeadlines
										on Tracks.Id = ActivityDeadlines.TrackId
										where Submissions.Id = @SubmissionId and Submissions.TrackId = @TrackId and Submissions.IsDeleted = 'false'
										and Tracks.Id = @TrackId and Tracks.IsDeleted = 'false'
										and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
										order by 
											ActivityDeadlines.Factor asc
										offset 0 rows
										fetch next 1 rows only
									) as SelectedSameTrackSubmissions
									join
									(
										select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
										from SubmissionClones
										where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
									) as SelectedLatestSubmissionClones
									on SelectedSameTrackSubmissions.Id = SelectedLatestSubmissionClones.SubmissionId
									where SelectedSameTrackSubmissions.DeadlineName like '%Review Submission Deadline'
								) as SelectedWithLatestSubmissionCloneSameTrackSubmissions
								where 
									SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo = 0
									or (SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo > 0 
									and SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo = SelectedWithLatestSubmissionCloneSameTrackSubmissions.RevisionNo)
							) as SelectedReviewingSubmissions
							join ReviewAssignments on SelectedReviewingSubmissions.LatestSubmissionCloneId = ReviewAssignments.SubmissionCloneId
							where ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
						) as SelectedReviewingWithReviewAssignmentSubmissions
						join
						(
							-- Reviewer List
							select 
								SelectedIncumbents.*,
								AbpUsers.Email,
								AbpUsers.Name,
								AbpUsers.MiddleName,
								AbpUsers.Surname,
								(
									case 
									when AbpUsers.MiddleName is null
									then concat(AbpUsers.Name,' ',AbpUsers.Surname)
									else
									concat(AbpUsers.Name,' ',AbpUsers.MiddleName,' ',AbpUsers.Surname)
									end
								) as 'FullName',
								AbpUsers.Organization
							from
							(
								-- Reviewer List
								select 
									Reviewers.Id as 'ReviewerId',
									Reviewers.Quota,
									ConferenceAccounts.AccountId,
									stuff(
									(select ';' + 
										(
											concat(SubjectAreas.Name,'|',ReviewerSubjectAreas.IsPrimary)
										) as 'ReviewerSubjectArea'
										from ReviewerSubjectAreas
										join SubjectAreas on ReviewerSubjectAreas.SubjectAreaId = SubjectAreas.Id
										where (ReviewerSubjectAreas.IsDeleted is null or ReviewerSubjectAreas.IsDeleted = 'false') and ReviewerSubjectAreas.ReviewerId = Reviewers.Id
										and SubjectAreas.IsDeleted = 'false'
										order by ReviewerSubjectAreas.IsPrimary desc, ReviewerSubjectArea asc
										for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
									) as 'SelectedReviewerSubjectAreas'
								from Reviewers
								join Incumbents
								on Reviewers.Id = Incumbents.Id
								join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
								where Incumbents.TrackId = @TrackId
								and Reviewers.IsDeleted = 'false' and Incumbents.IsDeleted = 'false'
								group by
									Reviewers.Id,
									Reviewers.Quota,
									ConferenceAccounts.AccountId
							) as SelectedIncumbents
							join AbpUsers on SelectedIncumbents.AccountId = AbpUsers.Id
							where AbpUsers.IsDeleted = 'false'
						) as SelectedReviewers
						on SelectedReviewingWithReviewAssignmentSubmissions.ReviewerId = SelectedReviewers.ReviewerId
					) as SelectedInfoPartReviewer
					where @InclusionText is null or (
					lower(SelectedInfoPartReviewer.FullName) like '%'+@InclusionText+'%' or
					lower(SelectedInfoPartReviewer.Organization) like '%'+@InclusionText+'%' or
					lower(SelectedInfoPartReviewer.SelectedReviewerSubjectAreas) like '%'+@InclusionText+'%')
				) as SelectedInfoPartWithInclusionTextReviewer
				group by
					SelectedInfoPartWithInclusionTextReviewer.ReviewerId,
					SelectedInfoPartWithInclusionTextReviewer.FullName,
					SelectedInfoPartWithInclusionTextReviewer.FirstName,
					SelectedInfoPartWithInclusionTextReviewer.MiddleName,
					SelectedInfoPartWithInclusionTextReviewer.LastName,
					SelectedInfoPartWithInclusionTextReviewer.Email,
					SelectedInfoPartWithInclusionTextReviewer.Organization,
					SelectedInfoPartWithInclusionTextReviewer.SelectedReviewerSubjectAreas,
					SelectedInfoPartWithInclusionTextReviewer.Quota,
					SelectedInfoPartWithInclusionTextReviewer.IsAssigned
			) as SelectedInfoPartWithInclusionTextWithConflictsReviewer
			left join
			(
				select count(*) as 'NumberOfAssignments', SelectedReviewingWithReviewAssignmentSubmissions.ReviewerId
				from
				(
					select 
						SelectedReviewingSubmissions.*,
						ReviewAssignments.Id as 'ReviewAssignmentId',
						ReviewAssignments.ReviewerId
					from
					(
						select SelectedWithLatestSubmissionCloneSameTrackSubmissions.*
						from
						(
							select 
								SelectedSameTrackSubmissions.*,
								SelectedLatestSubmissionClones.*
							from
							(
								-- All submission in the same track as 
								select Submissions.Id, ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
								from Submissions
								join Tracks on Submissions.TrackId = Tracks.Id
								left join ActivityDeadlines
								on Tracks.Id = ActivityDeadlines.TrackId
								where Submissions.TrackId = @TrackId and Submissions.IsDeleted = 'false'
								and Tracks.Id = @TrackId and Tracks.IsDeleted = 'false'
								and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
								order by 
									ActivityDeadlines.Factor asc
								offset 0 rows
								fetch next 1 rows only
							) as SelectedSameTrackSubmissions
							join
							(
								select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
								from SubmissionClones
								where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
							) as SelectedLatestSubmissionClones
							on SelectedSameTrackSubmissions.Id = SelectedLatestSubmissionClones.SubmissionId
							where SelectedSameTrackSubmissions.DeadlineName like '%Review Submission Deadline'
						) as SelectedWithLatestSubmissionCloneSameTrackSubmissions
						where 
							SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo = 0
							or (SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo > 0 
							and SelectedWithLatestSubmissionCloneSameTrackSubmissions.CloneNo = SelectedWithLatestSubmissionCloneSameTrackSubmissions.RevisionNo)
					) as SelectedReviewingSubmissions
					join ReviewAssignments on SelectedReviewingSubmissions.LatestSubmissionCloneId = ReviewAssignments.SubmissionCloneId
					where ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
				) as SelectedReviewingWithReviewAssignmentSubmissions
				join
				(
					-- Reviewer List
					select 
						Reviewers.Id as 'ReviewerId'
					from Reviewers
					join Incumbents
					on Reviewers.Id = Incumbents.Id
					where Incumbents.TrackId = @TrackId
					and Reviewers.IsDeleted = 'false' and Incumbents.IsDeleted = 'false'
				) as SelectedReviewers
				on SelectedReviewingWithReviewAssignmentSubmissions.ReviewerId = SelectedReviewers.ReviewerId
				group by 
					SelectedReviewingWithReviewAssignmentSubmissions.ReviewerId
			) as SelectedNumberOfAssignments
			on SelectedInfoPartWithInclusionTextWithConflictsReviewer.ReviewerId = SelectedNumberOfAssignments.ReviewerId

			END
            ";

            migrationBuilder.Sql(getSubmissionReviewerAssignmentSuggestionSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getSubmissionReviewerAssignmentSuggestionSP = @"
			DROP PROCEDURE [dbo].[GetSubmissionReviewerAssignmentSuggestion]
			"
            ;

            migrationBuilder.Sql(getSubmissionReviewerAssignmentSuggestionSP);
        }
    }
}
