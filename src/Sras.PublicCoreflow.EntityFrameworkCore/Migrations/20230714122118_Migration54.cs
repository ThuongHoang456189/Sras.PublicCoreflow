using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Sras.PublicCoreflow.ConferenceManagement;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration54 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getReviewerReviewingInformationAggregationSP = @"
			CREATE OR ALTER PROCEDURE [dbo].[GetReviewerReviewingInformationAggregation]
            @UTCNowStr nvarchar(20),
			@InclusionText nvarchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@AccountId uniqueidentifier,
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
				select
				*
				from
				(
					select 
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Quota,
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
					join Incumbents on Reviewers.Id = Incumbents.Id
					join
					(
						--selected tracks with current deadline
						select SelectedConferences.Id as 'ConferenceId', SelectedConferences.FullName as 'ConferenceFullName', SelectedConferences.ShortName as 'ConferenceShortName', Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', SelectedActivityDeadlines.Name as 'DeadlineName', SelectedActivityDeadlines.RevisionNo
						from
						Tracks 
						join 
						(
							--selected conference
							select Conferences.Id, Conferences.FullName, Conferences.ShortName
							from Conferences 
							where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
						) as SelectedConferences
						on Tracks.ConferenceId = SelectedConferences.Id
						left join 
						(
							select ActivityDeadlines.*
							from
							(
								select Tracks.Id as 'TrackId', min(ActivityDeadlines.Factor) as 'MinFactor'
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
								group by
									Tracks.Id
							) as SelectedFactors
							join ActivityDeadlines on SelectedFactors.TrackId = ActivityDeadlines.TrackId and ActivityDeadlines.Factor = SelectedFactors.MinFactor
						) as SelectedActivityDeadlines
						on Tracks.Id = SelectedActivityDeadlines.TrackId
						where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
						and SelectedActivityDeadlines.Status = 1 and SelectedActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(SelectedActivityDeadlines.Deadline as date) as datetime2(7))
					) as SelectedWithTrackIdTracks
					on Incumbents.TrackId = SelectedWithTrackIdTracks.TrackId
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					join Tracks on SelectedWithTrackIdTracks.TrackId = Tracks.Id
					where Reviewers.IsDeleted = 'false'
					and Incumbents.IsDeleted = 'false'
					and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
					and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
					group by
						AbpUsers.Id,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id,
						Reviewers.Quota
				) as SelectedReviewers
				where (@InclusionText is null or (
				lower(SelectedReviewers.ConferenceFullName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.ConferenceShortName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.TrackName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.SelectedReviewerSubjectAreas) like '%'+lower(@InclusionText)+'%'))
			) as list

			-- for select

			if @SortedAsc = 1
			begin
				select
					@TotalCount as 'TotalCount',
					SelectedReviewers.AccountId,
					SelectedReviewers.NamePrefix,
					SelectedReviewers.FirstName,
					SelectedReviewers.MiddleName,
					SelectedReviewers.LastName,
					SelectedReviewers.Email,
					SelectedReviewers.Organization,
					SelectedReviewers.Country,
					SelectedReviewers.DomainConflicts,
					SelectedReviewers.ConferenceId,
					SelectedReviewers.ConferenceFullName,
					SelectedReviewers.ConferenceShortName,
					SelectedReviewers.TrackId,
					SelectedReviewers.TrackName,
					SelectedReviewers.ReviewerId,
					SelectedReviewers.Quota,
					SelectedReviewers.SelectedReviewerSubjectAreas
				from
				(
					select 
						AbpUsers.Id as 'AccountId',
						AbpUsers.NamePrefix,
						AbpUsers.Name as 'FirstName',
						AbpUsers.MiddleName,
						AbpUsers.Surname as 'LastName',
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id as 'ReviewerId',
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime,
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
					join Incumbents on Reviewers.Id = Incumbents.Id
					join
					(
						--selected tracks with current deadline
						select SelectedConferences.Id as 'ConferenceId', SelectedConferences.FullName as 'ConferenceFullName', SelectedConferences.ShortName as 'ConferenceShortName', Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', SelectedActivityDeadlines.Name as 'DeadlineName', SelectedActivityDeadlines.RevisionNo
						from Tracks 
						join 
						(
							--selected conference
							select Conferences.Id, Conferences.FullName, Conferences.ShortName
							from Conferences 
							where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
						) as SelectedConferences
						on Tracks.ConferenceId = SelectedConferences.Id
						left join 
						(
							select ActivityDeadlines.*
							from
							(
								select Tracks.Id as 'TrackId', min(ActivityDeadlines.Factor) as 'MinFactor'
								from Tracks 
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
								group by
									Tracks.Id
							) as SelectedFactors
							join ActivityDeadlines on SelectedFactors.TrackId = ActivityDeadlines.TrackId and ActivityDeadlines.Factor = SelectedFactors.MinFactor
						) as SelectedActivityDeadlines
						on Tracks.Id = SelectedActivityDeadlines.TrackId
						where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
						and SelectedActivityDeadlines.Status = 1 and SelectedActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(SelectedActivityDeadlines.Deadline as date) as datetime2(7))
					) as SelectedWithTrackIdTracks
					on Incumbents.TrackId = SelectedWithTrackIdTracks.TrackId
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					join Tracks on SelectedWithTrackIdTracks.TrackId = Tracks.Id
					where Reviewers.IsDeleted = 'false'
					and Incumbents.IsDeleted = 'false'
					and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
					and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
					group by
						AbpUsers.Id,
						AbpUsers.NamePrefix,
						AbpUsers.Name,
						AbpUsers.MiddleName,
						AbpUsers.Surname,
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id,
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime
				) as SelectedReviewers
				where (@InclusionText is null or (
				lower(SelectedReviewers.ConferenceFullName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.ConferenceShortName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.TrackName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.SelectedReviewerSubjectAreas) like '%'+lower(@InclusionText)+'%'))
				order by
					case 
						when @Sorting is not null and @Sorting like '%conferencefullname' then SelectedReviewers.ConferenceFullName
						when @Sorting is not null and @Sorting like '%conferenceshortname' then SelectedReviewers.ConferenceShortName
						when @Sorting is not null and @Sorting like '%trackname' then SelectedReviewers.TrackName
					end asc,
					case when SelectedReviewers.LastModificationTime is not null then SelectedReviewers.LastModificationTime end desc,
					SelectedReviewers.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			if @SortedAsc = 0
			begin
				select
					@TotalCount as 'TotalCount',
					SelectedReviewers.AccountId,
					SelectedReviewers.NamePrefix,
					SelectedReviewers.FirstName,
					SelectedReviewers.MiddleName,
					SelectedReviewers.LastName,
					SelectedReviewers.Email,
					SelectedReviewers.Organization,
					SelectedReviewers.Country,
					SelectedReviewers.DomainConflicts,
					SelectedReviewers.ConferenceId,
					SelectedReviewers.ConferenceFullName,
					SelectedReviewers.ConferenceShortName,
					SelectedReviewers.TrackId,
					SelectedReviewers.TrackName,
					SelectedReviewers.ReviewerId,
					SelectedReviewers.Quota,
					SelectedReviewers.SelectedReviewerSubjectAreas
				from
				(
					select 
						AbpUsers.Id as 'AccountId',
						AbpUsers.NamePrefix,
						AbpUsers.Name as 'FirstName',
						AbpUsers.MiddleName,
						AbpUsers.Surname as 'LastName',
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id as 'ReviewerId',
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime,
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
					join Incumbents on Reviewers.Id = Incumbents.Id
					join
					(
						--selected tracks with current deadline
						select SelectedConferences.Id as 'ConferenceId', SelectedConferences.FullName as 'ConferenceFullName', SelectedConferences.ShortName as 'ConferenceShortName', Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', SelectedActivityDeadlines.Name as 'DeadlineName', SelectedActivityDeadlines.RevisionNo
						from Tracks 
						join 
						(
							--selected conference
							select Conferences.Id, Conferences.FullName, Conferences.ShortName
							from Conferences 
							where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
						) as SelectedConferences
						on Tracks.ConferenceId = SelectedConferences.Id
						left join 
						(
							select ActivityDeadlines.*
							from
							(
								select Tracks.Id as 'TrackId', min(ActivityDeadlines.Factor) as 'MinFactor'
								from Tracks 
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
								group by
									Tracks.Id
							) as SelectedFactors
							join ActivityDeadlines on SelectedFactors.TrackId = ActivityDeadlines.TrackId and ActivityDeadlines.Factor = SelectedFactors.MinFactor
						) as SelectedActivityDeadlines
						on Tracks.Id = SelectedActivityDeadlines.TrackId
						where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
						and SelectedActivityDeadlines.Status = 1 and SelectedActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(SelectedActivityDeadlines.Deadline as date) as datetime2(7))
					) as SelectedWithTrackIdTracks
					on Incumbents.TrackId = SelectedWithTrackIdTracks.TrackId
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					join Tracks on SelectedWithTrackIdTracks.TrackId = Tracks.Id
					where Reviewers.IsDeleted = 'false'
					and Incumbents.IsDeleted = 'false'
					and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
					and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
					group by
						AbpUsers.Id,
						AbpUsers.NamePrefix,
						AbpUsers.Name,
						AbpUsers.MiddleName,
						AbpUsers.Surname,
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id,
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime
				) as SelectedReviewers
				where (@InclusionText is null or (
				lower(SelectedReviewers.ConferenceFullName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.ConferenceShortName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.TrackName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.SelectedReviewerSubjectAreas) like '%'+lower(@InclusionText)+'%'))
				order by
					case 
						when @Sorting is not null and @Sorting like '%conferencefullname' then SelectedReviewers.ConferenceFullName
						when @Sorting is not null and @Sorting like '%conferenceshortname' then SelectedReviewers.ConferenceShortName
						when @Sorting is not null and @Sorting like '%trackname' then SelectedReviewers.TrackName
					end desc,
					case when SelectedReviewers.LastModificationTime is not null then SelectedReviewers.LastModificationTime end desc,
					SelectedReviewers.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			if @Sorting is null and @SortedAsc is null
			begin
				select
					@TotalCount as 'TotalCount',
					SelectedReviewers.AccountId,
					SelectedReviewers.NamePrefix,
					SelectedReviewers.FirstName,
					SelectedReviewers.MiddleName,
					SelectedReviewers.LastName,
					SelectedReviewers.Email,
					SelectedReviewers.Organization,
					SelectedReviewers.Country,
					SelectedReviewers.DomainConflicts,
					SelectedReviewers.ConferenceId,
					SelectedReviewers.ConferenceFullName,
					SelectedReviewers.ConferenceShortName,
					SelectedReviewers.TrackId,
					SelectedReviewers.TrackName,
					SelectedReviewers.ReviewerId,
					SelectedReviewers.Quota,
					SelectedReviewers.SelectedReviewerSubjectAreas
				from
				(
					select 
						AbpUsers.Id as 'AccountId',
						AbpUsers.NamePrefix,
						AbpUsers.Name as 'FirstName',
						AbpUsers.MiddleName,
						AbpUsers.Surname as 'LastName',
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id as 'ReviewerId',
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime,
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
					join Incumbents on Reviewers.Id = Incumbents.Id
					join
					(
						--selected tracks with current deadline
						select SelectedConferences.Id as 'ConferenceId', SelectedConferences.FullName as 'ConferenceFullName', SelectedConferences.ShortName as 'ConferenceShortName', Tracks.Id as 'TrackId', Tracks.Name as 'TrackName', SelectedActivityDeadlines.Name as 'DeadlineName', SelectedActivityDeadlines.RevisionNo
						from Tracks 
						join 
						(
							--selected conference
							select Conferences.Id, Conferences.FullName, Conferences.ShortName
							from Conferences 
							where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
						) as SelectedConferences
						on Tracks.ConferenceId = SelectedConferences.Id
						left join 
						(
							select ActivityDeadlines.*
							from
							(
								select Tracks.Id as 'TrackId', min(ActivityDeadlines.Factor) as 'MinFactor'
								from Tracks 
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
								group by
									Tracks.Id
							) as SelectedFactors
							join ActivityDeadlines on SelectedFactors.TrackId = ActivityDeadlines.TrackId and ActivityDeadlines.Factor = SelectedFactors.MinFactor
						) as SelectedActivityDeadlines
						on Tracks.Id = SelectedActivityDeadlines.TrackId
						where Tracks.IsDeleted = 'false' and (@TrackId is null or (Tracks.Id = @TrackId))
						and SelectedActivityDeadlines.Status = 1 and SelectedActivityDeadlines.IsDeleted = 'false' and @Today <= cast(cast(SelectedActivityDeadlines.Deadline as date) as datetime2(7))
					) as SelectedWithTrackIdTracks
					on Incumbents.TrackId = SelectedWithTrackIdTracks.TrackId
					join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
					join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
					join Tracks on SelectedWithTrackIdTracks.TrackId = Tracks.Id
					where Reviewers.IsDeleted = 'false'
					and Incumbents.IsDeleted = 'false'
					and ConferenceAccounts.IsDeleted = 'false' and ConferenceAccounts.ConferenceId = @ConferenceId
					and AbpUsers.IsDeleted='false' and AbpUsers.Id = @AccountId
					group by
						AbpUsers.Id,
						AbpUsers.NamePrefix,
						AbpUsers.Name,
						AbpUsers.MiddleName,
						AbpUsers.Surname,
						AbpUsers.Email,
						AbpUsers.Organization,
						AbpUsers.Country,
						AbpUsers.DomainConflicts,
						SelectedWithTrackIdTracks.ConferenceId,
						SelectedWithTrackIdTracks.ConferenceFullName,
						SelectedWithTrackIdTracks.ConferenceShortName,
						SelectedWithTrackIdTracks.TrackId,
						SelectedWithTrackIdTracks.TrackName,
						Reviewers.Id,
						Reviewers.Quota,
						Reviewers.CreationTime,
						Reviewers.LastModificationTime
				) as SelectedReviewers
				where (@InclusionText is null or (
				lower(SelectedReviewers.ConferenceFullName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.ConferenceShortName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.TrackName) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedReviewers.SelectedReviewerSubjectAreas) like '%'+lower(@InclusionText)+'%'))
				order by
					case when SelectedReviewers.LastModificationTime is not null then SelectedReviewers.LastModificationTime end desc,
					SelectedReviewers.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
			end

			END
            "
            ;

            migrationBuilder.Sql(getReviewerReviewingInformationAggregationSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getReviewerReviewingInformationAggregationSP = @"
			DROP PROCEDURE [dbo].[GetReviewerReviewingInformationAggregation]
			"
            ;

            migrationBuilder.Sql(getReviewerReviewingInformationAggregationSP);
        }
    }
}
