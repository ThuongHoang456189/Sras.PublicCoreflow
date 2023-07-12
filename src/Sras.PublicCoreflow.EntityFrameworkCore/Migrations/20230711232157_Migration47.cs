using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration47 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getReviewerSubmissionAggregationSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetReviewerSubmissionAggregation]
			@UTCNowStr nvarchar(20),
			@InclusionText nvarchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@AccountId uniqueidentifier,
			@IsReviewed bit,
			@Sorting varchar(128),
			@SortedAsc bit,
			@SkipCount int,
			@MaxResultCount int
			AS
			BEGIN

			declare 
				@TotalCount int,
				@Today datetime2(7),
				@UTCNow datetime2(7),
				@LocalTimeZoneId nvarchar(512),
				@LocalNow datetime2(7)

			select @UTCNow = convert(datetime2(7), @UTCNowStr, 126)

			select @LocalTimeZoneId = Conferences.TimeZoneId
			from Conferences
			where Conferences.Id = @ConferenceId
			and Conferences.IsDeleted = 'false'

			select @LocalNow = cast((@UTCNow at time zone 'UTC') at time zone @LocalTimeZoneId as datetime2(7))

			select @Today = cast(cast(@LocalNow as date) as datetime2(7))

			-- for count
			select 
				@TotalCount = count(*)
			from
			(
				select SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.*
				from
				(
					-- submission with subject area include
					select 
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*,
						stuff(
						(select ';' + 
							(
								concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
							) as 'SubmissionSubjectArea'
							from SubmissionSubjectAreas
							join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
							where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id
							and SubjectAreas.IsDeleted = 'false'
							order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
						) as 'SelectedSubmissionSubjectAreas'
					from
					(
						-- submission with latest submission
						select 
							Submissions.Id,
							Submissions.Title,
							SelectedTracks.TrackId,
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId,
							SelectedLatestSubmissionClones.CloneNo,
							Submissions.RootFilePath as 'SubmissionRootFilePath',
							Revisions.RootFilePath as 'RevisionRootFilePath'
						from
						Submissions
						join
						(
							select SelectedWithTrackIdTracks.*
							from
							(
								--selected tracks with current deadline
								select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
								from
								Tracks 
								join 
								(
									--selected conference
									select Conferences.Id 
									from Conferences 
									where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
								) as SelectedConferences
								on Tracks.ConferenceId = SelectedConferences.Id
								left join ActivityDeadlines
								on Tracks.Id = ActivityDeadlines.TrackId
								where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
								and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
								order by 
									ActivityDeadlines.Factor asc
								offset 0 rows
								fetch next 1 rows only
							) as SelectedWithTrackIdTracks
							where 
								SelectedWithTrackIdTracks.DeadlineName like '%Review Submission Deadline'
						) as SelectedTracks
						on Submissions.TrackId = SelectedTracks.TrackId
						join 
						(
							select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
							from SubmissionClones
							where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
						) as SelectedLatestSubmissionClones
						on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
						left join Revisions on SelectedLatestSubmissionClones.LatestSubmissionCloneId = Revisions.Id
						where
							Submissions.IsDeleted = 'false'
							and (SelectedLatestSubmissionClones.CloneNo = 0 
							or (SelectedLatestSubmissionClones.CloneNo > 0 and SelectedLatestSubmissionClones.CloneNo = SelectedTracks.RevisionNo))
					) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
					group by
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Title,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackId,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackName,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.CloneNo,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SubmissionRootFilePath,
						SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionRootFilePath
				) as SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission
				where (@InclusionText is null or (
				lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.Title) like '%'+@InclusionText+'%' or
				lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.TrackName) like '%'+@InclusionText+'%' or
				lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%'))
				and (
				(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo = 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SubmissionRootFilePath is not null)
				or
				(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo > 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.RevisionRootFilePath is not null)
				)
			) as SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission
			-- select review assignment assign to 
			join
			(
				select 
					ReviewAssignments.Id as 'ReviewAssignmentId', 
					ReviewAssignments.TotalScore, 
					ReviewAssignments.SubmissionCloneId, 
					ReviewAssignments.Review,
					ReviewAssignments.CreationTime,
					ReviewAssignments.LastModificationTime
				from ReviewAssignments
				join Reviewers on ReviewAssignments.ReviewerId = Reviewers.Id
				join Incumbents on Reviewers.Id = Incumbents.Id
				join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
				join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
				where 
					ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
					and Reviewers.IsDeleted = 'false'
					and Incumbents.IsDeleted = 'false'
					and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
					and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
			) as SelectedReviewAssignments
			on SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.LatestSubmissionCloneId = SelectedReviewAssignments.SubmissionCloneId

			-- for select
 
			if @SortedAsc = 1
			begin
				select 
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SelectedSubmissionSubjectAreas,
					SelectedReviewAssignments.ReviewAssignmentId,
					(
						case
							when 
								SelectedReviewAssignments.Review is null
							then 'EnterReview'
							else
								concat('EditReview','|','DeleteReview','|','ViewReview')
						end
					) as 'Actions',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SubmissionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SupplementaryMaterialRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.RevisionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.CloneNo
				from
				(
					select SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.*
					from
					(
						-- submission with subject area include
						select 
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*,
							stuff(
							(select ';' + 
								(
									concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
								) as 'SubmissionSubjectArea'
								from SubmissionSubjectAreas
								join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
								where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id
								and SubjectAreas.IsDeleted = 'false'
								order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
								for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
							) as 'SelectedSubmissionSubjectAreas'
						from
						(
							-- submission with latest submission
							select 
								Submissions.Id,
								Submissions.Title,
								SelectedTracks.TrackId,
								SelectedTracks.TrackName,
								SelectedTracks.DeadlineName,
								SelectedTracks.RevisionNo,
								SelectedLatestSubmissionClones.LatestSubmissionCloneId,
								SelectedLatestSubmissionClones.CloneNo,
								Submissions.RootFilePath as 'SubmissionRootFilePath',
								SelectedAttachment.SupplementaryMaterialRootFilePath,
								Revisions.RootFilePath as 'RevisionRootFilePath'
							from
							Submissions
							join
							(
								select SelectedWithTrackIdTracks.*
								from
								(
									--selected tracks with current deadline
									select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
									from
									Tracks 
									join 
									(
										--selected conference
										select Conferences.Id 
										from Conferences 
										where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
									) as SelectedConferences
									on Tracks.ConferenceId = SelectedConferences.Id
									left join ActivityDeadlines
									on Tracks.Id = ActivityDeadlines.TrackId
									where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
									and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
									order by 
										ActivityDeadlines.Factor asc
									offset 0 rows
									fetch next 1 rows only
								) as SelectedWithTrackIdTracks
								where 
									SelectedWithTrackIdTracks.DeadlineName like '%Review Submission Deadline'
							) as SelectedTracks
							on Submissions.TrackId = SelectedTracks.TrackId
							join 
							(
								select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
								from SubmissionClones
								where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
							) as SelectedLatestSubmissionClones
							on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
							left join
							(
								select
									SubmissionAttachments.Id as 'SubmissionAttachmentId',
									SubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath'
								from SubmissionAttachments
							) as SelectedAttachment
							on Submissions.Id = SelectedAttachment.SubmissionAttachmentId
							left join Revisions on SelectedLatestSubmissionClones.LatestSubmissionCloneId = Revisions.Id
							where
								Submissions.IsDeleted = 'false'
								and (SelectedLatestSubmissionClones.CloneNo = 0 
								or (SelectedLatestSubmissionClones.CloneNo > 0 and SelectedLatestSubmissionClones.CloneNo = SelectedTracks.RevisionNo))
						) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
						group by
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Title,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.DeadlineName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.CloneNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SubmissionRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SupplementaryMaterialRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionRootFilePath
					) as SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission
					where @InclusionText is null or (
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.Title) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.TrackName) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
					and (
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo = 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SubmissionRootFilePath is not null)
					or
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo > 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.RevisionRootFilePath is not null)
					)
				) as SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission
				-- select review assignment assign to 
				join
				(
					select 
						ReviewAssignments.Id as 'ReviewAssignmentId', 
						ReviewAssignments.TotalScore, 
						ReviewAssignments.SubmissionCloneId, 
						ReviewAssignments.Review,
						ReviewAssignments.CreationTime,
						ReviewAssignments.LastModificationTime
					from ReviewAssignments
					join Reviewers on ReviewAssignments.ReviewerId = Reviewers.Id
					join Incumbents on Reviewers.Id = Incumbents.Id
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					where 
						ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
						and Reviewers.IsDeleted = 'false'
						and Incumbents.IsDeleted = 'false'
						and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
						and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
				) as SelectedReviewAssignments
				on SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.LatestSubmissionCloneId = SelectedReviewAssignments.SubmissionCloneId
				order by
					case 
						when @Sorting is not null and @Sorting like '%title' then SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Title
						when @Sorting is not null and @Sorting like '%track' then SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackName
					end asc,
					case when SelectedReviewAssignments.LastModificationTime is not null then SelectedReviewAssignments.LastModificationTime end desc,
					SelectedReviewAssignments.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			if @SortedAsc = 0
			begin
				select 
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SelectedSubmissionSubjectAreas,
					SelectedReviewAssignments.ReviewAssignmentId,
					(
						case
							when 
								SelectedReviewAssignments.Review is null
							then 'EnterReview'
							else
								concat('EditReview','|','DeleteReview','|','ViewReview')
						end
					) as 'Actions',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SubmissionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SupplementaryMaterialRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.RevisionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.CloneNo
				from
				(
					select SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.*
					from
					(
						-- submission with subject area include
						select 
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*,
							stuff(
							(select ';' + 
								(
									concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
								) as 'SubmissionSubjectArea'
								from SubmissionSubjectAreas
								join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
								where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id
								and SubjectAreas.IsDeleted = 'false'
								order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
								for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
							) as 'SelectedSubmissionSubjectAreas'
						from
						(
							-- submission with latest submission
							select 
								Submissions.Id,
								Submissions.Title,
								SelectedTracks.TrackId,
								SelectedTracks.TrackName,
								SelectedTracks.DeadlineName,
								SelectedTracks.RevisionNo,
								SelectedLatestSubmissionClones.LatestSubmissionCloneId,
								SelectedLatestSubmissionClones.CloneNo,
								Submissions.RootFilePath as 'SubmissionRootFilePath',
								SelectedAttachment.SupplementaryMaterialRootFilePath,
								Revisions.RootFilePath as 'RevisionRootFilePath'
							from
							Submissions
							join
							(
								select SelectedWithTrackIdTracks.*
								from
								(
									--selected tracks with current deadline
									select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
									from
									Tracks 
									join 
									(
										--selected conference
										select Conferences.Id 
										from Conferences 
										where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
									) as SelectedConferences
									on Tracks.ConferenceId = SelectedConferences.Id
									left join ActivityDeadlines
									on Tracks.Id = ActivityDeadlines.TrackId
									where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
									and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
									order by 
										ActivityDeadlines.Factor asc
									offset 0 rows
									fetch next 1 rows only
								) as SelectedWithTrackIdTracks
								where 
									SelectedWithTrackIdTracks.DeadlineName like '%Review Submission Deadline'
							) as SelectedTracks
							on Submissions.TrackId = SelectedTracks.TrackId
							join 
							(
								select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
								from SubmissionClones
								where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
							) as SelectedLatestSubmissionClones
							on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
							left join
							(
								select
									SubmissionAttachments.Id as 'SubmissionAttachmentId',
									SubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath'
								from SubmissionAttachments
							) as SelectedAttachment
							on Submissions.Id = SelectedAttachment.SubmissionAttachmentId
							left join Revisions on SelectedLatestSubmissionClones.LatestSubmissionCloneId = Revisions.Id
							where
								Submissions.IsDeleted = 'false'
								and (SelectedLatestSubmissionClones.CloneNo = 0 
								or (SelectedLatestSubmissionClones.CloneNo > 0 and SelectedLatestSubmissionClones.CloneNo = SelectedTracks.RevisionNo))
						) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
						group by
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Title,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.DeadlineName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.CloneNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SubmissionRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SupplementaryMaterialRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionRootFilePath
					) as SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission
					where @InclusionText is null or (
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.Title) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.TrackName) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
					and (
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo = 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SubmissionRootFilePath is not null)
					or
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo > 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.RevisionRootFilePath is not null)
					)
				) as SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission
				-- select review assignment assign to 
				join
				(
					select 
						ReviewAssignments.Id as 'ReviewAssignmentId', 
						ReviewAssignments.TotalScore, 
						ReviewAssignments.SubmissionCloneId, 
						ReviewAssignments.Review,
						ReviewAssignments.CreationTime,
						ReviewAssignments.LastModificationTime
					from ReviewAssignments
					join Reviewers on ReviewAssignments.ReviewerId = Reviewers.Id
					join Incumbents on Reviewers.Id = Incumbents.Id
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					where 
						ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
						and Reviewers.IsDeleted = 'false'
						and Incumbents.IsDeleted = 'false'
						and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
						and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
				) as SelectedReviewAssignments
				on SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.LatestSubmissionCloneId = SelectedReviewAssignments.SubmissionCloneId
				order by
					case 
						when @Sorting is not null and @Sorting like '%title' then SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Title
						when @Sorting is not null and @Sorting like '%track' then SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackName
					end desc,
					case when SelectedReviewAssignments.LastModificationTime is not null then SelectedReviewAssignments.LastModificationTime end desc,
					SelectedReviewAssignments.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			if @Sorting is null and @SortedAsc is null
			begin
				select
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SelectedSubmissionSubjectAreas,
					SelectedReviewAssignments.ReviewAssignmentId,
					(
						case
							when 
								SelectedReviewAssignments.Review is null
							then 'EnterReview'
							else
								concat('EditReview','|','DeleteReview','|','ViewReview')
						end
					) as 'Actions',
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SubmissionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.SupplementaryMaterialRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.RevisionRootFilePath,
					SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.CloneNo
				from
				(
					select SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.*
					from
					(
						-- submission with subject area include
						select 
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*,
							stuff(
							(select ';' + 
								(
									concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
								) as 'SubmissionSubjectArea'
								from SubmissionSubjectAreas
								join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
								where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id
								and SubjectAreas.IsDeleted = 'false'
								order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
								for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
							) as 'SelectedSubmissionSubjectAreas'
						from
						(
							-- submission with latest submission
							select 
								Submissions.Id,
								Submissions.Title,
								SelectedTracks.TrackId,
								SelectedTracks.TrackName,
								SelectedTracks.DeadlineName,
								SelectedTracks.RevisionNo,
								SelectedLatestSubmissionClones.LatestSubmissionCloneId,
								SelectedLatestSubmissionClones.CloneNo,
								Submissions.RootFilePath as 'SubmissionRootFilePath',
								SelectedAttachment.SupplementaryMaterialRootFilePath,
								Revisions.RootFilePath as 'RevisionRootFilePath'
							from
							Submissions
							join
							(
								select SelectedWithTrackIdTracks.*
								from
								(
									--selected tracks with current deadline
									select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', ActivityDeadlines.Name as 'DeadlineName', ActivityDeadlines.RevisionNo
									from
									Tracks 
									join 
									(
										--selected conference
										select Conferences.Id 
										from Conferences 
										where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
									) as SelectedConferences
									on Tracks.ConferenceId = SelectedConferences.Id
									left join ActivityDeadlines
									on Tracks.Id = ActivityDeadlines.TrackId
									where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
									and ActivityDeadlines.Status = 1 and ActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7))
									order by 
										ActivityDeadlines.Factor asc
									offset 0 rows
									fetch next 1 rows only
								) as SelectedWithTrackIdTracks
								where 
									SelectedWithTrackIdTracks.DeadlineName like '%Review Submission Deadline'
							) as SelectedTracks
							on Submissions.TrackId = SelectedTracks.TrackId
							join 
							(
								select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
								from SubmissionClones
								where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
							) as SelectedLatestSubmissionClones
							on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
							left join
							(
								select
									SubmissionAttachments.Id as 'SubmissionAttachmentId',
									SubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath'
								from SubmissionAttachments
							) as SelectedAttachment
							on Submissions.Id = SelectedAttachment.SubmissionAttachmentId
							left join Revisions on SelectedLatestSubmissionClones.LatestSubmissionCloneId = Revisions.Id
							where
								Submissions.IsDeleted = 'false'
								and (SelectedLatestSubmissionClones.CloneNo = 0 
								or (SelectedLatestSubmissionClones.CloneNo > 0 and SelectedLatestSubmissionClones.CloneNo = SelectedTracks.RevisionNo))
						) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
						group by
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Id,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.Title,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.TrackName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.DeadlineName,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.CloneNo,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SubmissionRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.SupplementaryMaterialRootFilePath,
							SelectedLatestSubmissionCloneWithSubmissionIdSubmission.RevisionRootFilePath
					) as SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission
					where @InclusionText is null or (
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.Title) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.TrackName) like '%'+@InclusionText+'%' or
					lower(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
					and (
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo = 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.SubmissionRootFilePath is not null)
					or
					(SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.CloneNo > 0 and SelectedLatestSubmissionCloneWithSubjectAreaWithSubmissionIdSubmission.RevisionRootFilePath is not null)
					)
				) as SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission
				-- select review assignment assign to 
				join
				(
					select 
						ReviewAssignments.Id as 'ReviewAssignmentId', 
						ReviewAssignments.TotalScore, 
						ReviewAssignments.SubmissionCloneId, 
						ReviewAssignments.Review,
						ReviewAssignments.CreationTime,
						ReviewAssignments.LastModificationTime
					from ReviewAssignments
					join Reviewers on ReviewAssignments.ReviewerId = Reviewers.Id
					join Incumbents on Reviewers.Id = Incumbents.Id
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					where 
						ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
						and Reviewers.IsDeleted = 'false'
						and Incumbents.IsDeleted = 'false'
						and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
						and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
				) as SelectedReviewAssignments
				on SelectedInfoPartLatestSubmissionCloneWithSubjectAreaSubmission.LatestSubmissionCloneId = SelectedReviewAssignments.SubmissionCloneId
				order by
					case when SelectedReviewAssignments.LastModificationTime is not null then SelectedReviewAssignments.LastModificationTime end desc,
					SelectedReviewAssignments.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			END
            ";

            migrationBuilder.Sql(getReviewerSubmissionAggregationSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getReviewerSubmissionAggregationSP = @"
			DROP PROCEDURE [dbo].[GetReviewerSubmissionAggregation]
			"
            ;

            migrationBuilder.Sql(getReviewerSubmissionAggregationSP);
        }
    }
}
