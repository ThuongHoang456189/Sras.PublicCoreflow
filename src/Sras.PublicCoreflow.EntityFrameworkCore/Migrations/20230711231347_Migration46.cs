using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Sras.PublicCoreflow.ConferenceManagement;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration46 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getAuthorSubmissionAggregationSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetAuthorSubmissionAggregation]
			@UTCNowStr nvarchar(20),
			@InclusionText nvarchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@AccountId uniqueidentifier,
			@StatusId uniqueidentifier,
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

			select @TotalCount = count(*)
			from
			Submissions
			join
			(
				--selected tracks
				select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName'
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
				where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
			) as SelectedTracks
			on Submissions.TrackId = SelectedTracks.TrackId
			join PaperStatuses
			on Submissions.NotifiedStatusId = PaperStatuses.Id
			join Authors on Authors.SubmissionId = Submissions.Id
			join Participants on Authors.ParticipantId = Participants.Id
			join AbpUsers on Participants.AccountId = AbpUsers.Id
			where 
			Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
			and PaperStatuses.IsDeleted = 'false'
			and Authors.IsDeleted = 'false'
			and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
			and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
			and (
			@InclusionText is null or
			lower(Submissions.Title) like '%'+@InclusionText+'%' or
			lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
			group by 
				Submissions.Id


			-- for select
 
			if @SortedAsc = 1
			begin

				select 
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneSubmission.SubmissionRootFilePath,
					SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo,
					SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath,
					SelectedCameraReadies.CameraReadyRootFilePath,
					SelectedCameraReadies.CopyRightFilePath,
					SelectedSubmissionAttachments.RootPresentationFilePath as 'PresentationRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName,
					SelectedInfoPartLatestSubmissionCloneSubmission.StatusId,
					SelectedInfoPartLatestSubmissionCloneSubmission.Status,
					(
						case
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Deadline'
							then concat('DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Edits Deadline'
							then concat('EditSubmission','|','DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is null
							then 'UploadSupplementary'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is not null
							then concat('EditSupplementary','|','DeleteSupplementary')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and (SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo < SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								or SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is null
								)
							then 'UploadRevision'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo = SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								and SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is not null
							then concat('EditRevision','|','DeleteRevision')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is null
							then concat('CreateCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is not null
							then concat('EditCameraReadySubmission','|','DeleteCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is null
							then 'UploadPresentation'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is not null
							then concat('EditPresentation','|','DeletePresentation')
							else
								null
						end
					) as 'Actions'
				from
				(
					-- submission info part
					select 
						Submissions.Id, 
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath as 'SubmissionRootFilePath',
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id as 'StatusId',
						PaperStatuses.Name as 'Status',
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation
					from
					Submissions
					join
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
					) as SelectedTracks
					on Submissions.TrackId = SelectedTracks.TrackId
					join PaperStatuses
					on Submissions.NotifiedStatusId = PaperStatuses.Id
					join Authors on Authors.SubmissionId = Submissions.Id
					join Participants on Authors.ParticipantId = Participants.Id
					join AbpUsers on Participants.AccountId = AbpUsers.Id
					join 
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId,  SubmissionClones.CloneNo
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
					where 
					Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
					and PaperStatuses.IsDeleted = 'false'
					and Authors.IsDeleted = 'false'
					and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
					and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
					and (
					@InclusionText is null or
					lower(Submissions.Title) like '%'+@InclusionText+'%' or
					lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
					group by 
						Submissions.Id,
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath,
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id,
						PaperStatuses.Name,
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation,
						Submissions.CreationTime,
						Submissions.LastModificationTime
					--order by
					order by
						case 
							when @Sorting is not null and @Sorting like '%title' then Submissions.Title
							when @Sorting is not null and @Sorting like '%track' then SelectedTracks.TrackName
						end asc,
						case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
						Submissions.CreationTime desc
					offset @SkipCount rows
					fetch next @MaxResultCount rows only
				) as SelectedInfoPartLatestSubmissionCloneSubmission
				-- left join supplementary material and presentation
				left join
				(
					select 
						SubmissionAttachments.Id as 'SubmissionAttachmentId',
						SubmissionAttachments.RootSupplementaryMaterialFilePath,
						SubmissionAttachments.RootPresentationFilePath
					from SubmissionAttachments
					where SubmissionAttachments.IsDeleted = 'false'
				) as SelectedSubmissionAttachments
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedSubmissionAttachments.SubmissionAttachmentId
				-- left join revision
				left join
				(
					-- submission with latest submission clone and revision 
					select SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*, Revisions.RootFilePath as 'RevisionRootFilePath'
					from
					(
						-- submission with latest submission clone
						select 
							Submissions.Id, 
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId
						from
						Submissions
						join
						(
							--selected tracks with current deadline
							select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName'
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
						) as SelectedTracks
						on Submissions.TrackId = SelectedTracks.TrackId
						join PaperStatuses
						on Submissions.NotifiedStatusId = PaperStatuses.Id
						join Authors on Authors.SubmissionId = Submissions.Id
						join Participants on Authors.ParticipantId = Participants.Id
						join AbpUsers on Participants.AccountId = AbpUsers.Id
						join 
						(
							select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
							from SubmissionClones
							where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
						) as SelectedLatestSubmissionClones
						on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
						where 
						Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
						and PaperStatuses.IsDeleted = 'false'
						and Authors.IsDeleted = 'false'
						and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
						and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
						and (
						@InclusionText is null or
						lower(Submissions.Title) like '%'+@InclusionText+'%' or
						lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
						group by 
							Submissions.Id,
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId,
							Submissions.CreationTime,
							Submissions.LastModificationTime
						--order by
						order by
							case 
								when @Sorting is not null and @Sorting like '%title' then Submissions.Title
								when @Sorting is not null and @Sorting like '%track' then SelectedTracks.TrackName
							end asc,
							case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
							Submissions.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
					join Revisions
					on Revisions.Id = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId
				) as SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.Id
				-- left join camera ready
				left join
				(
					select 
						CameraReadies.Id as 'CameraReadyId',
						CameraReadies.RootCameraReadyFilePath as 'CameraReadyRootFilePath',
						CameraReadies.CopyRightFilePath as 'CopyRightFilePath'
					from CameraReadies
					where CameraReadies.IsDeleted = 'false' and CameraReadies.CopyRightFilePath is not null
				) as SelectedCameraReadies
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedCameraReadies.CameraReadyId

			end

			if @SortedAsc = 0
			begin 

				select
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneSubmission.SubmissionRootFilePath,
					SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo,
					SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath,
					SelectedCameraReadies.CameraReadyRootFilePath,
					SelectedCameraReadies.CopyRightFilePath,
					SelectedSubmissionAttachments.RootPresentationFilePath as 'PresentationRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName,
					SelectedInfoPartLatestSubmissionCloneSubmission.StatusId,
					SelectedInfoPartLatestSubmissionCloneSubmission.Status,
					(
						case
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Deadline'
							then concat('DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Edits Deadline'
							then concat('EditSubmission','|','DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is null
							then 'UploadSupplementary'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is not null
							then concat('EditSupplementary','|','DeleteSupplementary')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and (SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo < SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								or SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is null
								)
							then 'UploadRevision'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo = SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								and SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is not null
							then concat('EditRevision','|','DeleteRevision')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is null
							then concat('CreateCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is not null
							then concat('EditCameraReadySubmission','|','DeleteCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is null
							then 'UploadPresentation'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is not null
							then concat('EditPresentation','|','DeletePresentation')
							else
								null
						end
					) as 'Actions'
				from
				(
					select 
						Submissions.Id, 
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath as 'SubmissionRootFilePath',
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id as 'StatusId',
						PaperStatuses.Name as 'Status',
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation
					from
					Submissions
					join
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
					) as SelectedTracks
					on Submissions.TrackId = SelectedTracks.TrackId
					join PaperStatuses
					on Submissions.NotifiedStatusId = PaperStatuses.Id
					join Authors on Authors.SubmissionId = Submissions.Id
					join Participants on Authors.ParticipantId = Participants.Id
					join AbpUsers on Participants.AccountId = AbpUsers.Id
					join 
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId,  SubmissionClones.CloneNo
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
					where 
					Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
					and PaperStatuses.IsDeleted = 'false'
					and Authors.IsDeleted = 'false'
					and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
					and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
					and (
					@InclusionText is null or
					lower(Submissions.Title) like '%'+@InclusionText+'%' or
					lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
					group by 
						Submissions.Id,
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath,
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id,
						PaperStatuses.Name,
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation,
						Submissions.CreationTime,
						Submissions.LastModificationTime
					--order by
					order by
						case 
							when @Sorting is not null and @Sorting like '%title' then Submissions.Title
							when @Sorting is not null and @Sorting like '%track' then SelectedTracks.TrackName
						end desc,
						case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
						Submissions.CreationTime desc
					offset @SkipCount rows
					fetch next @MaxResultCount rows only
				) as SelectedInfoPartLatestSubmissionCloneSubmission
				-- left join supplementary material and presentation
				left join
				(
					select 
						SubmissionAttachments.Id as 'SubmissionAttachmentId',
						SubmissionAttachments.RootSupplementaryMaterialFilePath,
						SubmissionAttachments.RootPresentationFilePath
					from SubmissionAttachments
					where SubmissionAttachments.IsDeleted = 'false'
				) as SelectedSubmissionAttachments
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedSubmissionAttachments.SubmissionAttachmentId
				-- left join revision
				left join
				(
					-- submission with latest submission clone and revision
					select SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*, Revisions.RootFilePath as 'RevisionRootFilePath'
					from
					(
						-- submission with latest submission clone
						select 
							Submissions.Id, 
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId
						from
						Submissions
						join
						(
							--selected tracks with current deadline
							select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName'
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
						) as SelectedTracks
						on Submissions.TrackId = SelectedTracks.TrackId
						join PaperStatuses
						on Submissions.NotifiedStatusId = PaperStatuses.Id
						join Authors on Authors.SubmissionId = Submissions.Id
						join Participants on Authors.ParticipantId = Participants.Id
						join AbpUsers on Participants.AccountId = AbpUsers.Id
						join 
						(
							select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
							from SubmissionClones
							where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
						) as SelectedLatestSubmissionClones
						on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
						where 
						Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
						and PaperStatuses.IsDeleted = 'false'
						and Authors.IsDeleted = 'false'
						and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
						and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
						and (
						@InclusionText is null or
						lower(Submissions.Title) like '%'+@InclusionText+'%' or
						lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
						group by 
							Submissions.Id,
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId,
							Submissions.CreationTime,
							Submissions.LastModificationTime
						--order by
						order by
							case 
								when @Sorting is not null and @Sorting like '%title' then Submissions.Title
								when @Sorting is not null and @Sorting like '%track' then SelectedTracks.TrackName
							end desc,
							case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
							Submissions.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
					join Revisions
					on Revisions.Id = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId
					where Revisions.IsDeleted = 'false'
				) as SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.Id
				-- left join camera ready
				left join
				(
					select 
						CameraReadies.Id as 'CameraReadyId',
						CameraReadies.RootCameraReadyFilePath as 'CameraReadyRootFilePath',
						CameraReadies.CopyRightFilePath as 'CopyRightFilePath'
					from CameraReadies
					where CameraReadies.IsDeleted = 'false' and CameraReadies.CopyRightFilePath is not null
				) as SelectedCameraReadies
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedCameraReadies.CameraReadyId

			end

			if @Sorting is null and @SortedAsc is null
			begin

				select 
					@TotalCount as 'TotalCount',
					SelectedInfoPartLatestSubmissionCloneSubmission.Id,
					SelectedInfoPartLatestSubmissionCloneSubmission.Title,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackId,
					SelectedInfoPartLatestSubmissionCloneSubmission.TrackName,
					SelectedInfoPartLatestSubmissionCloneSubmission.SubmissionRootFilePath,
					SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath as 'SupplementaryMaterialRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo,
					SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath,
					SelectedCameraReadies.CameraReadyRootFilePath,
					SelectedCameraReadies.CopyRightFilePath,
					SelectedSubmissionAttachments.RootPresentationFilePath as 'PresentationRootFilePath',
					SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName,
					SelectedInfoPartLatestSubmissionCloneSubmission.StatusId,
					SelectedInfoPartLatestSubmissionCloneSubmission.Status,
					(
						case
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Deadline'
							then concat('DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Submission Edits Deadline'
							then concat('EditSubmission','|','DeleteSubmission','|','Edit Conflicts')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is null
							then 'UploadSupplementary'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Supplementary Material Deadline'
								and SelectedSubmissionAttachments.RootSupplementaryMaterialFilePath is not null
							then concat('EditSupplementary','|','DeleteSupplementary')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and (SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo < SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								or SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is null
								)
							then 'UploadRevision'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.Status like 'Revision'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Revision%'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like '%Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName not like '%Review Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.CloneNo = SelectedInfoPartLatestSubmissionCloneSubmission.RevisionNo
								and SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.RevisionRootFilePath is not null
							then concat('EditRevision','|','DeleteRevision')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is null
							then concat('CreateCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Camera Ready Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForCameraReady = 1
								and SelectedCameraReadies.CameraReadyRootFilePath is not null
							then concat('EditCameraReadySubmission','|','DeleteCameraReadySubmission','|','SubmitCopyright')
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is null
							then 'UploadPresentation'
							when 
								SelectedInfoPartLatestSubmissionCloneSubmission.DeadlineName like 'Presentation Submission Deadline'
								and SelectedInfoPartLatestSubmissionCloneSubmission.IsRequestedForPresentation = 1
								and SelectedSubmissionAttachments.RootPresentationFilePath is not null
							then concat('EditPresentation','|','DeletePresentation')
							else
								null
						end
					) as 'Actions'
				from
				(
					select 
						Submissions.Id, 
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath as 'SubmissionRootFilePath',
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id as 'StatusId',
						PaperStatuses.Name as 'Status',
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation
					from
					Submissions
					join
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
					) as SelectedTracks
					on Submissions.TrackId = SelectedTracks.TrackId
					join PaperStatuses
					on Submissions.NotifiedStatusId = PaperStatuses.Id
					join Authors on Authors.SubmissionId = Submissions.Id
					join Participants on Authors.ParticipantId = Participants.Id
					join AbpUsers on Participants.AccountId = AbpUsers.Id
					join 
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId,  SubmissionClones.CloneNo
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
					where 
					Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
					and PaperStatuses.IsDeleted = 'false'
					and Authors.IsDeleted = 'false'
					and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
					and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
					and (
					@InclusionText is null or
					lower(Submissions.Title) like '%'+@InclusionText+'%' or
					lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
					group by 
						Submissions.Id,
						Submissions.Title, 
						SelectedTracks.TrackId, 
						SelectedTracks.TrackName,
						Submissions.RootFilePath,
						SelectedLatestSubmissionClones.CloneNo,
						PaperStatuses.Id,
						PaperStatuses.Name,
						SelectedTracks.DeadlineName,
						SelectedTracks.RevisionNo,
						Submissions.IsRequestedForCameraReady,
						Submissions.IsRequestedForPresentation,
						Submissions.CreationTime,
						Submissions.LastModificationTime
					--order by
					order by
						case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
						Submissions.CreationTime desc
					offset @SkipCount rows
					fetch next @MaxResultCount rows only
				) as SelectedInfoPartLatestSubmissionCloneSubmission
				-- left join supplementary material and presentation
				left join
				(
					select 
						SubmissionAttachments.Id as 'SubmissionAttachmentId',
						SubmissionAttachments.RootSupplementaryMaterialFilePath,
						SubmissionAttachments.RootPresentationFilePath
					from SubmissionAttachments
					where SubmissionAttachments.IsDeleted = 'false'
				) as SelectedSubmissionAttachments
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedSubmissionAttachments.SubmissionAttachmentId
				-- left join revision
				left join
				(
					-- submission with latest submission clone and revision
					select SelectedLatestSubmissionCloneWithSubmissionIdSubmission.*, Revisions.RootFilePath as 'RevisionRootFilePath'
					from
					(
						-- submission with latest submission clone
						select 
							Submissions.Id, 
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId
						from
						Submissions
						join
						(
							--selected tracks with current deadline
							select Tracks.Id as 'TrackId', Tracks.Name as 'TrackName'
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
						) as SelectedTracks
						on Submissions.TrackId = SelectedTracks.TrackId
						join PaperStatuses
						on Submissions.NotifiedStatusId = PaperStatuses.Id
						join Authors on Authors.SubmissionId = Submissions.Id
						join Participants on Authors.ParticipantId = Participants.Id
						join AbpUsers on Participants.AccountId = AbpUsers.Id
						join 
						(
							select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
							from SubmissionClones
							where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
						) as SelectedLatestSubmissionClones
						on Submissions.Id = SelectedLatestSubmissionClones.SubmissionId
						where 
						Submissions.IsDeleted = 'false' and (@StatusId is null or Submissions.NotifiedStatusId = @StatusId)
						and PaperStatuses.IsDeleted = 'false'
						and Authors.IsDeleted = 'false'
						and Participants.IsDeleted = 'false' and Participants.AccountId is not null 
						and AbpUsers.IsDeleted = 'false' and AbpUsers.Id = @AccountId
						and (
						@InclusionText is null or
						lower(Submissions.Title) like '%'+@InclusionText+'%' or
						lower(SelectedTracks.TrackName) like '%'+@InclusionText+'%')
						group by 
							Submissions.Id,
							Submissions.Title, 
							SelectedTracks.TrackId, 
							SelectedTracks.TrackName,
							SelectedLatestSubmissionClones.LatestSubmissionCloneId,
							Submissions.CreationTime,
							Submissions.LastModificationTime
						--order by
						order by
							case when Submissions.LastModificationTime is not null then Submissions.LastModificationTime end desc,
							Submissions.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedLatestSubmissionCloneWithSubmissionIdSubmission
					join Revisions
					on Revisions.Id = SelectedLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId
					where Revisions.IsDeleted = 'false'
				) as SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedLatestSubmissionCloneWithRevisionWithSubmissionIdSubmission.Id
				-- left join camera ready
				left join
				(
					select 
						CameraReadies.Id as 'CameraReadyId',
						CameraReadies.RootCameraReadyFilePath as 'CameraReadyRootFilePath',
						CameraReadies.CopyRightFilePath as 'CopyRightFilePath'
					from CameraReadies
					where CameraReadies.IsDeleted = 'false' and CameraReadies.CopyRightFilePath is not null
				) as SelectedCameraReadies
				on SelectedInfoPartLatestSubmissionCloneSubmission.Id = SelectedCameraReadies.CameraReadyId

			end

			END          
            ";

            migrationBuilder.Sql(getAuthorSubmissionAggregationSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getAuthorSubmissionAggregationSP = @"
			DROP PROCEDURE [dbo].[GetAuthorSubmissionAggregation]
			"
            ;

            migrationBuilder.Sql(getAuthorSubmissionAggregationSP);
        }
    }
}
