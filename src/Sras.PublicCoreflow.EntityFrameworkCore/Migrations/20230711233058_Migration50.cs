using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration50 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getTopTimeSubmissionAggregationSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetTopTimeSubmissionAggregation]
			@InclusionText varchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@StatusId uniqueidentifier,
			@SkipCount int,
			@MaxResultCount int
			AS
			BEGIN

			declare @TotalCount int

			-- for count
			select @TotalCount = count(*)
			from 
			(
				--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
				select SelectedCTSSubmission.*,
				stuff(
				(select ';' + 
					(
						case
						when Participants.AccountId is not null and AbpUsers.MiddleName is not null
						then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
						when Participants.AccountId is not null and AbpUsers.MiddleName is null
						then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
						when Participants.AccountId is null and AbpUsers.MiddleName is not null
						then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
						when Participants.AccountId is null and AbpUsers.MiddleName is null
						then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
						end
					) as 'Author'
					from Authors
					join Participants on Authors.ParticipantId = Participants.Id
					left join AbpUsers on Participants.AccountId = AbpUsers.Id
					left join Outsiders on Participants.OutsiderId = Outsiders.Id
					where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
					and Participants.IsDeleted = 'false'
					and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
					and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
					order by Authors.IsPrimaryContact desc, Author asc
					for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
				) as 'SelectedAuthors',
				stuff(
				(select ';' + 
					(
						concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
					) as 'SubmissionSubjectArea'
					from SubmissionSubjectAreas
					join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
					where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
					and SubjectAreas.IsDeleted = 'false'
					order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
					for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
				) as 'SelectedSubmissionSubjectAreas'
				from 
				(
					--selected submission with @ConferenceId, @TrackId, and @StatusId
					select Submissions.Id, Submissions.Title, SelectedTracks.TrackName 
					from
					Submissions 
					join
					(
						--selected tracks
						select Tracks.Id, Tracks.Name as 'TrackName'
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
					on Submissions.TrackId = SelectedTracks.Id
					join PaperStatuses
					on Submissions.StatusId = PaperStatuses.Id
					where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
					and PaperStatuses.IsDeleted = 'false'
				) as SelectedCTSSubmission
				group by 
					SelectedCTSSubmission.Id, 
					SelectedCTSSubmission.Title,
					SelectedCTSSubmission.TrackName
			) as SelectedCTSASASubmission
			where @InclusionText is null or (
			lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
			lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
			lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
			lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%'
			)

			-- for select

			select
				@TotalCount as 'TotalCount',
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Title,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Abstract,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.SelectedAuthors,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.SelectedSubmissionSubjectAreas,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.TrackId,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.TrackName,
				SelectedSubmissionConflictsWithSubmissionIdSubmission.SubmissionConflicts,
				SelectedReviewerConflictsWithSubmissionIdSubmission.ReviewerConflicts,
				SelectedAssignedWithSubmissionIdSubmission.Assigned,
				SelectedReviewedAndAverageScoreWithSubmissionIdSubmission.Reviewed,
				SelectedReviewedAndAverageScoreWithSubmissionIdSubmission.AverageScore,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.StatusId,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Status,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.LatestSubmissionCloneId,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.CloneNo,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.IsRequestedForCameraReady,
				SelectedCameraReadies.CameraReadyId,
				SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.IsRequestedForPresentation
			from
			(
				select SelectedInfoPartCTSASAWithInclusionTextSubmission.*,
				SelectedLatestSubmissionClones.LatestSubmissionCloneId, SelectedLatestSubmissionClones.CloneNo
				from
				(
					--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText with Pagination
					select SelectedCTSASAWithInclusionTextSubmission.*
					from
					(
						--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText
						select SelectedCTSASASubmission.*
						from
						(

							--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
							select SelectedCTSSubmission.*,
							stuff(
							(select ';' + 
								(
									case
									when Participants.AccountId is not null and AbpUsers.MiddleName is not null
									then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
									when Participants.AccountId is not null and AbpUsers.MiddleName is null
									then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
									when Participants.AccountId is null and AbpUsers.MiddleName is not null
									then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
									when Participants.AccountId is null and AbpUsers.MiddleName is null
									then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
									end
								) as 'Author'
								from Authors
								join Participants on Authors.ParticipantId = Participants.Id
								left join AbpUsers on Participants.AccountId = AbpUsers.Id
								left join Outsiders on Participants.OutsiderId = Outsiders.Id
								where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
								and Participants.IsDeleted = 'false'
								and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
								and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
								order by Authors.IsPrimaryContact desc, Author asc
								for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
							) as 'SelectedAuthors',
							stuff(
							(select ';' + 
								(
									concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
								) as 'SubmissionSubjectArea'
								from SubmissionSubjectAreas
								join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
								where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
								and SubjectAreas.IsDeleted = 'false'
								order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
								for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
							) as 'SelectedSubmissionSubjectAreas'
							from
							(
								--selected submission with @ConferenceId, @TrackId, and @StatusId
								select Submissions.Id, Submissions.Title, Submissions.Abstract, 
								Submissions.TrackId, SelectedTracks.TrackName,
								Submissions.StatusId, PaperStatuses.Name as 'Status',
								Submissions.IsRequestedForCameraReady, Submissions.IsRequestedForPresentation, Submissions.CreationTime, Submissions.LastModificationTime
								from
								Submissions 
								join
								(
									--selected tracks
									select Tracks.Id, Tracks.Name as 'TrackName'
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
								on Submissions.TrackId = SelectedTracks.Id
								join PaperStatuses
								on Submissions.StatusId = PaperStatuses.Id
								where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
								and PaperStatuses.IsDeleted = 'false'
							) as SelectedCTSSubmission
							group by 
								SelectedCTSSubmission.Id, 
								SelectedCTSSubmission.Title,
								SelectedCTSSubmission.Abstract,
								SelectedCTSSubmission.TrackId,
								SelectedCTSSubmission.TrackName,
								SelectedCTSSubmission.StatusId,
								SelectedCTSSubmission.Status,
								SelectedCTSSubmission.IsRequestedForCameraReady,
								SelectedCTSSubmission.IsRequestedForPresentation,
								SelectedCTSSubmission.CreationTime,
								SelectedCTSSubmission.LastModificationTime
						) as SelectedCTSASASubmission
						where @InclusionText is null or (
						lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
						lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
						lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
						lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
					) as SelectedCTSASAWithInclusionTextSubmission
					order by
						case when SelectedCTSASAWithInclusionTextSubmission.LastModificationTime is not null then SelectedCTSASAWithInclusionTextSubmission.LastModificationTime end desc,
						SelectedCTSASAWithInclusionTextSubmission.CreationTime desc
					offset @SkipCount rows
					fetch next @MaxResultCount rows only
				) as SelectedInfoPartCTSASAWithInclusionTextSubmission
				join
				(
					select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId, SubmissionClones.CloneNo
					from SubmissionClones
					where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
				) as SelectedLatestSubmissionClones
				on SelectedInfoPartCTSASAWithInclusionTextSubmission.Id = SelectedLatestSubmissionClones.SubmissionId
			) as SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission
			--left join Reviewed and Average Score
			left join
			(
				--selected submission: Id with Reviewed and AverageScore
				select SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
				count(SelectedActiveAndNotNullTotalScoreReviewAssignments.ReviewedReviewAssignmentId) as 'Reviewed', 
				avg(SelectedActiveAndNotNullTotalScoreReviewAssignments.TotalScore) as 'AverageScore'
				from
				(
					select SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.*,
					SelectedLatestSubmissionClones.LatestSubmissionCloneId
					from
					(
						--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText with Pagination
						select SelectedCTSASAWithInclusionTextSubmission.*
						from
						(
							--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText
							select SelectedCTSASASubmission.*
							from
							(

								--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
								select SelectedCTSSubmission.*,
								stuff(
								(select ';' + 
									(
										case
										when Participants.AccountId is not null and AbpUsers.MiddleName is not null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is not null and AbpUsers.MiddleName is null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is not null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										end
									) as 'Author'
									from Authors
									join Participants on Authors.ParticipantId = Participants.Id
									left join AbpUsers on Participants.AccountId = AbpUsers.Id
									left join Outsiders on Participants.OutsiderId = Outsiders.Id
									where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
									and Participants.IsDeleted = 'false'
									and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
									and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
									order by Authors.IsPrimaryContact desc, Author asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedAuthors',
								stuff(
								(select ';' + 
									(
										concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
									) as 'SubmissionSubjectArea'
									from SubmissionSubjectAreas
									join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
									where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
									and SubjectAreas.IsDeleted = 'false'
									order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedSubmissionSubjectAreas'
								from
								(
									--selected submission with @ConferenceId, @TrackId, and @StatusId
									select Submissions.Id, Submissions.Title, SelectedTracks.TrackName,
									Submissions.CreationTime, Submissions.LastModificationTime
									from
									Submissions 
									join
									(
										--selected tracks
										select Tracks.Id, Tracks.Name as 'TrackName'
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
									on Submissions.TrackId = SelectedTracks.Id
									join PaperStatuses
									on Submissions.StatusId = PaperStatuses.Id
									where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
									and PaperStatuses.IsDeleted = 'false'
								) as SelectedCTSSubmission
								group by 
									SelectedCTSSubmission.Id, 
									SelectedCTSSubmission.Title,
									SelectedCTSSubmission.TrackName,
									SelectedCTSSubmission.CreationTime,
									SelectedCTSSubmission.LastModificationTime
							) as SelectedCTSASASubmission
							where @InclusionText is null or (
							lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
						) as SelectedCTSASAWithInclusionTextSubmission
						order by
							case when SelectedCTSASAWithInclusionTextSubmission.LastModificationTime is not null then SelectedCTSASAWithInclusionTextSubmission.LastModificationTime end desc,
							SelectedCTSASAWithInclusionTextSubmission.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission
					join
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.Id = SelectedLatestSubmissionClones.SubmissionId
				) as SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission
				join
				(
					select ReviewAssignments.Id as 'ReviewedReviewAssignmentId', ReviewAssignments.TotalScore, ReviewAssignments.SubmissionCloneId
					from ReviewAssignments
					where ReviewAssignments.IsActive = 'true' and ReviewAssignments.TotalScore is not null and ReviewAssignments.IsDeleted = 'false'
				) as SelectedActiveAndNotNullTotalScoreReviewAssignments
				on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId = SelectedActiveAndNotNullTotalScoreReviewAssignments.SubmissionCloneId
				group by
					SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id
			) as SelectedReviewedAndAverageScoreWithSubmissionIdSubmission
			on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id = SelectedReviewedAndAverageScoreWithSubmissionIdSubmission.Id
			--left join Assigned
			left join
			(
				--selected submission: Id with Reviewed and AverageScore
				select SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
				count(SelectedActiveAndNotNullTotalScoreReviewAssignments.ActiveOnlyReviewAssignmentId) as 'Assigned'
				from
				(
					select SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.*,
					SelectedLatestSubmissionClones.LatestSubmissionCloneId
					from
					(
						--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText with Pagination
						select SelectedCTSASAWithInclusionTextSubmission.*
						from
						(
							--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText
							select SelectedCTSASASubmission.*
							from
							(

								--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
								select SelectedCTSSubmission.*,
								stuff(
								(select ';' + 
									(
										case
										when Participants.AccountId is not null and AbpUsers.MiddleName is not null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is not null and AbpUsers.MiddleName is null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is not null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										end
									) as 'Author'
									from Authors
									join Participants on Authors.ParticipantId = Participants.Id
									left join AbpUsers on Participants.AccountId = AbpUsers.Id
									left join Outsiders on Participants.OutsiderId = Outsiders.Id
									where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
									and Participants.IsDeleted = 'false'
									and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
									and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
									order by Authors.IsPrimaryContact desc, Author asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedAuthors',
								stuff(
								(select ';' + 
									(
										concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
									) as 'SubmissionSubjectArea'
									from SubmissionSubjectAreas
									join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
									where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
									and SubjectAreas.IsDeleted = 'false'
									order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedSubmissionSubjectAreas'
								from
								(
									--selected submission with @ConferenceId, @TrackId, and @StatusId
									select Submissions.Id, Submissions.Title, SelectedTracks.TrackName,
									Submissions.CreationTime, Submissions.LastModificationTime
									from
									Submissions 
									join
									(
										--selected tracks
										select Tracks.Id, Tracks.Name as 'TrackName'
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
									on Submissions.TrackId = SelectedTracks.Id
									join PaperStatuses
									on Submissions.StatusId = PaperStatuses.Id
									where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
									and PaperStatuses.IsDeleted = 'false'
								) as SelectedCTSSubmission
								group by 
									SelectedCTSSubmission.Id, 
									SelectedCTSSubmission.Title,
									SelectedCTSSubmission.TrackName,
									SelectedCTSSubmission.CreationTime,
									SelectedCTSSubmission.LastModificationTime
							) as SelectedCTSASASubmission
							where @InclusionText is null or (
							lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
						) as SelectedCTSASAWithInclusionTextSubmission
						order by
							case when SelectedCTSASAWithInclusionTextSubmission.LastModificationTime is not null then SelectedCTSASAWithInclusionTextSubmission.LastModificationTime end desc,
							SelectedCTSASAWithInclusionTextSubmission.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission
					join
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.Id = SelectedLatestSubmissionClones.SubmissionId
				) as SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission
				join
				(
					select ReviewAssignments.Id as 'ActiveOnlyReviewAssignmentId', ReviewAssignments.SubmissionCloneId
					from ReviewAssignments
					where ReviewAssignments.IsActive = 'true' and ReviewAssignments.IsDeleted = 'false'
				) as SelectedActiveAndNotNullTotalScoreReviewAssignments
				on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.LatestSubmissionCloneId = SelectedActiveAndNotNullTotalScoreReviewAssignments.SubmissionCloneId
				group by
					SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id
			) as SelectedAssignedWithSubmissionIdSubmission
			on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id = SelectedAssignedWithSubmissionIdSubmission.Id
			--left join Submission Conflicts
			left join
			(
				--selected submission: Id with Reviewed and AverageScore
				select SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
				count(*) as 'SubmissionConflicts'
				from
				(
					select SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.*,
					SelectedLatestSubmissionClones.LatestSubmissionCloneId
					from
					(
						--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText with Pagination
						select SelectedCTSASAWithInclusionTextSubmission.*
						from
						(
							--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText
							select SelectedCTSASASubmission.*
							from
							(

								--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
								select SelectedCTSSubmission.*,
								stuff(
								(select ';' + 
									(
										case
										when Participants.AccountId is not null and AbpUsers.MiddleName is not null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is not null and AbpUsers.MiddleName is null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is not null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										end
									) as 'Author'
									from Authors
									join Participants on Authors.ParticipantId = Participants.Id
									left join AbpUsers on Participants.AccountId = AbpUsers.Id
									left join Outsiders on Participants.OutsiderId = Outsiders.Id
									where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
									and Participants.IsDeleted = 'false'
									and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
									and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
									order by Authors.IsPrimaryContact desc, Author asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedAuthors',
								stuff(
								(select ';' + 
									(
										concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
									) as 'SubmissionSubjectArea'
									from SubmissionSubjectAreas
									join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
									where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
									and SubjectAreas.IsDeleted = 'false'
									order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedSubmissionSubjectAreas'
								from
								(
									--selected submission with @ConferenceId, @TrackId, and @StatusId
									select Submissions.Id, Submissions.Title, SelectedTracks.TrackName,
									Submissions.CreationTime, Submissions.LastModificationTime
									from
									Submissions 
									join
									(
										--selected tracks
										select Tracks.Id, Tracks.Name as 'TrackName'
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
									on Submissions.TrackId = SelectedTracks.Id
									join PaperStatuses
									on Submissions.StatusId = PaperStatuses.Id
									where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
									and PaperStatuses.IsDeleted = 'false'
								) as SelectedCTSSubmission
								group by 
									SelectedCTSSubmission.Id, 
									SelectedCTSSubmission.Title,
									SelectedCTSSubmission.TrackName,
									SelectedCTSSubmission.CreationTime,
									SelectedCTSSubmission.LastModificationTime
							) as SelectedCTSASASubmission
							where @InclusionText is null or (
							lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
						) as SelectedCTSASAWithInclusionTextSubmission
						order by
							case when SelectedCTSASAWithInclusionTextSubmission.LastModificationTime is not null then SelectedCTSASAWithInclusionTextSubmission.LastModificationTime end desc,
							SelectedCTSASAWithInclusionTextSubmission.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission
					join
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.Id = SelectedLatestSubmissionClones.SubmissionId
				) as SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission
				join
				(
					select Conflicts.Id as 'ConflictId', Conflicts.SubmissionId, Conflicts.IncumbentId
					from Conflicts
					where Conflicts.IsDeleted = 'false' and Conflicts.IsDefinedByReviewer = 'false'
				) as SelectedSubmissionConflicts
				on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id = SelectedSubmissionConflicts.SubmissionId
				group by
					SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
					SelectedSubmissionConflicts.IncumbentId
			) as SelectedSubmissionConflictsWithSubmissionIdSubmission
			on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id = SelectedSubmissionConflictsWithSubmissionIdSubmission.Id
			--left join Reviewer Conflicts
			left join
			(
				--selected submission: Id with Reviewed and AverageScore
				select SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
				count(*) as 'ReviewerConflicts'
				from
				(
					select SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.*,
					SelectedLatestSubmissionClones.LatestSubmissionCloneId
					from
					(
						--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText with Pagination
						select SelectedCTSASAWithInclusionTextSubmission.*
						from
						(
							--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation with contains @InclusionText
							select SelectedCTSASASubmission.*
							from
							(

								--selected submission with @ConferenceId, @TrackId, and @StatusId and authors and subject areas aggregation
								select SelectedCTSSubmission.*,
								stuff(
								(select ';' + 
									(
										case
										when Participants.AccountId is not null and AbpUsers.MiddleName is not null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is not null and AbpUsers.MiddleName is null
										then concat(AbpUsers.Email,'|',AbpUsers.NamePrefix,'|',AbpUsers.Name+' '+AbpUsers.Surname,'|',AbpUsers.Organization,'|',1,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is not null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.MiddleName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										when Participants.AccountId is null and AbpUsers.MiddleName is null
										then concat(Outsiders.Email,'|',Outsiders.NamePrefix,'|',Outsiders.FirstName+' '+Outsiders.LastName,'|',Outsiders.Organization,'|',0,'|',Authors.IsPrimaryContact)
										end
									) as 'Author'
									from Authors
									join Participants on Authors.ParticipantId = Participants.Id
									left join AbpUsers on Participants.AccountId = AbpUsers.Id
									left join Outsiders on Participants.OutsiderId = Outsiders.Id
									where Authors.IsDeleted = 'false' and Authors.SubmissionId = SelectedCTSSubmission.Id
									and Participants.IsDeleted = 'false'
									and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
									and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
									order by Authors.IsPrimaryContact desc, Author asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedAuthors',
								stuff(
								(select ';' + 
									(
										concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
									) as 'SubmissionSubjectArea'
									from SubmissionSubjectAreas
									join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
									where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = SelectedCTSSubmission.Id
									and SubjectAreas.IsDeleted = 'false'
									order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
									for xml path(''), type).value('.', 'varchar(2048)'),1,1,''
								) as 'SelectedSubmissionSubjectAreas'
								from
								(
									--selected submission with @ConferenceId, @TrackId, and @StatusId
									select Submissions.Id, Submissions.Title, SelectedTracks.TrackName,
									Submissions.CreationTime, Submissions.LastModificationTime
									from
									Submissions 
									join
									(
										--selected tracks
										select Tracks.Id, Tracks.Name as 'TrackName'
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
									on Submissions.TrackId = SelectedTracks.Id
									join PaperStatuses
									on Submissions.StatusId = PaperStatuses.Id
									where Submissions.IsDeleted = 'false'and (@StatusId is null or Submissions.StatusId = @StatusId)
									and PaperStatuses.IsDeleted = 'false'
								) as SelectedCTSSubmission
								group by 
									SelectedCTSSubmission.Id, 
									SelectedCTSSubmission.Title,
									SelectedCTSSubmission.TrackName,
									SelectedCTSSubmission.CreationTime,
									SelectedCTSSubmission.LastModificationTime
							) as SelectedCTSASASubmission
							where @InclusionText is null or (
							lower(SelectedCTSASASubmission.Title) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.TrackName) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedAuthors) like '%'+@InclusionText+'%' or
							lower(SelectedCTSASASubmission.SelectedSubmissionSubjectAreas) like '%'+@InclusionText+'%')
						) as SelectedCTSASAWithInclusionTextSubmission
						order by
							case when SelectedCTSASAWithInclusionTextSubmission.LastModificationTime is not null then SelectedCTSASAWithInclusionTextSubmission.LastModificationTime end desc,
							SelectedCTSASAWithInclusionTextSubmission.CreationTime desc
						offset @SkipCount rows
						fetch next @MaxResultCount rows only
					) as SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission
					join
					(
						select SubmissionClones.Id as 'LatestSubmissionCloneId', SubmissionClones.SubmissionId
						from SubmissionClones
						where SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
					) as SelectedLatestSubmissionClones
					on SelectedInfoPartCTSASAWithInclusionTextWithSubmissionIdSubmission.Id = SelectedLatestSubmissionClones.SubmissionId
				) as SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission
				join
				(
					select Conflicts.Id as 'ConflictId', Conflicts.SubmissionId, Conflicts.IncumbentId
					from Conflicts
					where Conflicts.IsDeleted = 'false' and Conflicts.IsDefinedByReviewer = 'true'
				) as SelectedReviewerConflicts
				on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id = SelectedReviewerConflicts.SubmissionId
				group by
					SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneWithSubmissionIdSubmission.Id,
					SelectedReviewerConflicts.IncumbentId
			) as SelectedReviewerConflictsWithSubmissionIdSubmission
			on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id = SelectedReviewerConflictsWithSubmissionIdSubmission.Id
			left join
			(
				select CameraReadies.Id as 'CameraReadyId' 
				from CameraReadies
				where CameraReadies.IsDeleted = 'false' and CameraReadies.CopyRightFilePath is not null
			) as SelectedCameraReadies
			on SelectedInfoPartCTSASAWithInclusionTextWithLatestSubmissionCloneSubmission.Id = SelectedCameraReadies.CameraReadyId

			END
            ";

            migrationBuilder.Sql(getTopTimeSubmissionAggregationSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getTopTimeSubmissionAggregationSP = @"
			DROP PROCEDURE [dbo].[GetTopTimeSubmissionAggregation]
			"
            ;

            migrationBuilder.Sql(getTopTimeSubmissionAggregationSP);
        }
    }
}
