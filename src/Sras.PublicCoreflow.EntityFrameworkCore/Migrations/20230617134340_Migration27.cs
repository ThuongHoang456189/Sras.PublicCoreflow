using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration27 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var submissionAggregationSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetSubmissionAggregation]
			@InclusionText varchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@StatusId uniqueidentifier,
			@SkipCount int,
			@MaxResultCount int
			AS
			BEGIN
				declare @TotalCount int

				select @TotalCount = count(ssstt.Id)
				from (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null and lower(id.Country) like 'vietnam'
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null and lower(id.Country) like 'vietnam'
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null and lower(id.Country) like 'vietnam'
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null and lower(id.Country) like 'vietnam'
				then concat(ot.LastName, ' ' + ot.FirstName)
				when p.AccountId is not null and id.MiddleName is not null and lower(id.Country) not like 'vietnam'
				then concat(id.Name, ' ' + id.MiddleName, ' ' + id.Surname)
				when p.AccountId is not null and id.MiddleName is null and lower(id.Country) not like 'vietnam'
				then concat(id.Name, ' ' + id.Surname)
				when p.AccountId is null and ot.MiddleName is not null and lower(id.Country) not like 'vietnam'
				then concat(ot.FirstName, ' ' + ot.MiddleName, ' ' + ot.LastName)
				when p.AccountId is null and ot.MiddleName is null and lower(id.Country) not like 'vietnam'
				then concat(ot.FirstName, ' ' + ot.LastName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				) as ssstt

				select @TotalCount as 'TotalCount', ssstt.Id, ssstt.Title, ssstt.Authors, ssstt.SubjectAreas, ssstt.TrackId, ssstt.TrackName, 
				subcfcount.SubmissionConflicts, revcfcount.ReviewerConflicts,
				asgcount.Assigned, revcount.Reviewed, revcount.AverageScore, 
				ssstt.StatusId, ssstt.Status,
				ssubclone.CloneNo,
				revision.RevisionSubId, 
				ssstt.IsRequestedForCameraReady, cameraready.CameraReadySubId,
				ssstt.IsRequestedForPresentation
				from (
				select sstt.Id, sstt.Title, author.Authors, sbjarea.SubjectAreas, sstt.TrackId, sstt.TrackName, 
				sstt.StatusId, sstt.Status,
				sstt.IsRequestedForCameraReady,
				sstt.IsRequestedForPresentation
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as ssstt
				left join (
				select count(*) as 'SubmissionConflicts', subcf.SubmissionId from (
				select cf.SubmissionId, cf.IncumbentId from Conflicts as cf join (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as stt1 on cf.SubmissionId = stt1.Id
				where cf.IsDefinedByReviewer = 'false'
				group by cf.SubmissionId, cf.IncumbentId) as subcf
				group by subcf.SubmissionId) as subcfcount on ssstt.Id = subcfcount.SubmissionId
				left join (
				select count(*) as 'ReviewerConflicts', subcf.SubmissionId from (
				select cf.SubmissionId, cf.IncumbentId from Conflicts as cf join (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as stt1 on cf.SubmissionId = stt1.Id
				where cf.IsDefinedByReviewer = 'true'
				group by cf.SubmissionId, cf.IncumbentId) as subcf
				group by subcf.SubmissionId
				) as revcfcount on ssstt.Id = revcfcount.SubmissionId
				left join (
				select sc.SubmissionId as 'SubId', sc.CloneNo from SubmissionClones as sc
				where sc.IsLast = 'true' and sc.IsDeleted = 'false'
				) as ssubclone on ssstt.Id = ssubclone.SubId
				left join (
				select count(*) as 'Assigned', asgra.Id from
				(select subclone.Id, ra.SubmissionCloneId, ra.ReviewerId from ReviewAssignments as ra
				join (
				select stt1.Id, sc.Id as 'CloneId' from SubmissionClones as sc join (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as stt1 on sc.SubmissionId = stt1.Id
				where sc.IsLast = 'true' and sc.IsDeleted = 'false'
				) as subclone on ra.SubmissionCloneId = subclone.CloneId
				where ra.IsDeleted = 'false' and ra.IsActive = 'true'
				group by subclone.Id, ra.SubmissionCloneId, ra.ReviewerId) as asgra
				group by asgra.SubmissionCloneId, asgra.Id
				) as asgcount on ssstt.Id = asgcount.Id
				left join (
				select count(*) as 'Reviewed', avg(revra.TotalScore) as 'AverageScore', revra.Id from
				(select subclone.Id, ra.SubmissionCloneId, ra.ReviewerId, ra.TotalScore from ReviewAssignments as ra
				join (
				select stt1.Id, sc.Id as 'CloneId' from SubmissionClones as sc join (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as stt1 on sc.SubmissionId = stt1.Id
				where sc.IsLast = 'true' and sc.IsDeleted = 'false'
				) as subclone on ra.SubmissionCloneId = subclone.CloneId
				where ra.IsDeleted = 'false' and ra.IsActive = 'true' and ra.TotalScore is not null
				group by subclone.Id, ra.SubmissionCloneId, ra.ReviewerId, ra.TotalScore) as revra
				group by revra.SubmissionCloneId, revra.Id
				) as revcount on ssstt.Id = revcount.Id
				left join (
				select lastsubclone.Id as 'RevisionSubId' from Revisions as revi
				join (
				select stt1.Id, sc.Id as 'CloneId' from SubmissionClones as sc join (
				select sstt.Id
				from (
				select s.Id, s.Title, s.TrackId, tt.Name as 'TrackName', s.StatusId, stat.Name as 'Status', s.IsRequestedForCameraReady, s.IsRequestedForPresentation,
				s.CreationTime, s.LastModificationTime
				from Submissions as s join (
				select t.Id, t.Name, t.ConferenceId from Tracks as t join ( 
				select Id from Conferences as c where Id = @ConferenceId and IsDeleted = 'false') as c on t.ConferenceId = c.Id
				where t.IsDeleted = 'false' and (@TrackId is null or (t.Id = @TrackId))) as tt on s.TrackId = tt.Id
				join PaperStatuses as stat on s.StatusId = stat.Id
				where s.IsDeleted = 'false' and (@StatusId is null or stat.Id = @StatusId) and stat.IsDeleted = 'false') as sstt
				left join (
				select s.Id,
				stuff (
				(select ', '+(
				case 
				when p.AccountId is not null and id.MiddleName is not null
				then concat(id.Surname, ' ' + id.MiddleName, ' ' + id.Name)
				when p.AccountId is not null and id.MiddleName is null
				then concat(id.Surname, ' ' + id.Name)
				when p.AccountId is null and ot.MiddleName is not null
				then concat(ot.LastName, ' ' + ot.MiddleName, ' ' + ot.FirstName)
				when p.AccountId is null and ot.MiddleName is null
				then concat(ot.LastName, ' ' + ot.FirstName)
				end 
				) as 'AuthorName'
				from Authors as auth
				join Participants as p on auth.ParticipantId = p.Id
				left join AbpUsers as id on p.AccountId = id.Id
				left join Outsiders as ot on p.OutsiderId = ot.Id
				where auth.IsDeleted = 'false' and p.IsDeleted = 'false'
				and auth.SubmissionId = s.Id
				order by auth.IsPrimaryContact desc, AuthorName
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'Authors'
				from Submissions as s
				group by s.Id
				) as author on sstt.Id = author.Id
				left join (
				select s.Id,
				stuff (
				(select ', '+sa.Name as 'SubjectArea'
				from SubmissionSubjectAreas as ssa
				join SubjectAreas as sa on ssa.SubjectAreaId = sa.Id
				join Tracks as t on sa.TrackId = t.Id
				where t.IsDeleted = 'false' and sa.IsDeleted = 'false' and ssa.IsDeleted = 'false'
				and ssa.SubmissionId = s.Id
				order by ssa.SubmissionId, ssa.IsPrimary desc
				for xml path(''), type).value('.', 'varchar(1024)')
				,1,1,''
				) as 'SubjectAreas'
				from Submissions as s
				group by s.Id
				) as sbjarea on sstt.Id = sbjarea.Id
				where @InclusionText is null or
				(@InclusionText is not null and 
				(lower(sstt.Title) like '%'+@InclusionText+'%' or 
				lower(sstt.TrackName) like '%'+@InclusionText+'%' or
				lower(author.Authors) like '%'+@InclusionText+'%' or
				lower(sbjarea.SubjectAreas) like '%'+@InclusionText+'%'))
				order by
				case when sstt.LastModificationTime is not null then sstt.LastModificationTime
				end desc, sstt.CreationTime desc
				offset @SkipCount rows
				fetch next @MaxResultCount rows only
				) as stt1 on sc.SubmissionId = stt1.Id
				where sc.IsLast = 'true' and sc.IsDeleted = 'false') as lastsubclone on revi.Id = lastsubclone.CloneId
				where revi.RootFilePath is not null and revi.IsDeleted = 'false') as revision on ssstt.Id = revision.RevisionSubId
				left join (
				select cr.Id as 'CameraReadySubId' from CameraReadies as cr
				where cr.IsDeleted = 'false'
				) as cameraready on ssstt.Id = cameraready.CameraReadySubId
			END
            ";

			migrationBuilder.Sql(submissionAggregationSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			var submissionAggregationSP = @"
			DROP PROCEDURE [dbo].[GetSubmissionAggregation]
			";

            migrationBuilder.Sql(submissionAggregationSP);
        }
    }
}
