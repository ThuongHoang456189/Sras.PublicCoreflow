using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getSubmissionSummarySP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetSubmissionSummary]
            @SubmissionId uniqueidentifier
            AS
            BEGIN

				declare 
					@TrackId uniqueidentifier

				select 
					@TrackId = Submissions.TrackId 
				from Submissions
				join Tracks on Submissions.TrackId = Tracks.Id
				where Submissions.Id = @SubmissionId and Submissions.IsDeleted = 'false'
				and Tracks.IsDeleted = 'false'


				select 
					SelectedInfoPartSubmission.ConferenceFullName,
					SelectedInfoPartSubmission.ConferenceShortName,
					SelectedInfoPartSubmission.TrackName,
					SelectedInfoPartSubmission.PaperId,
					SelectedInfoPartSubmission.Title,
					SelectedInfoPartSubmission.Abstract,
					SelectedInfoPartSubmission.CreationTime,
					SelectedInfoPartSubmission.LastModificationTime,
					SelectedInfoPartSubmission.SelectedAuthors,
					SelectedInfoPartSubmission.SelectedSubmissionSubjectAreas,
					SelectedInfoPartSubmission.DomainConflicts,
					SelectedConflictsOfInterest.SelectedSubmissionConflictedIncumbents,
					SelectedInfoPartSubmission.SubmissionRootFilePath,
					SelectedRevision.CloneNo as 'SubmittedRevisionNo',
					SelectedRevision.RevisionRootFilePath,
					SelectedInfoPartSubmission.SubmissionQuestionsResponse
				from
				(
					-- info part
					select 
						Conferences.FullName as 'ConferenceFullName',
						Conferences.ShortName as 'ConferenceShortName',
						Tracks.Name as 'TrackName',
						Submissions.Id as 'PaperId',
						Submissions.Title,
						Submissions.Abstract,
						Submissions.CreationTime,
						Submissions.LastModificationTime,
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
							where Authors.IsDeleted = 'false' and Authors.SubmissionId = Submissions.Id
							and Participants.IsDeleted = 'false'
							and (Participants.AccountId is null or (Participants.AccountId is not null and AbpUsers.IsDeleted = 'false'))
							and (Participants.OutsiderId is null or (Participants.OutsiderId is not null and Outsiders.IsDeleted = 'false'))
							order by Authors.IsPrimaryContact desc, Author asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
						) as 'SelectedAuthors',
						stuff(
						(select ';' + 
							(
								concat(SubjectAreas.Name,'|',SubmissionSubjectAreas.IsPrimary)
							) as 'SubmissionSubjectArea'
							from SubmissionSubjectAreas
							join SubjectAreas on SubmissionSubjectAreas.SubjectAreaId = SubjectAreas.Id
							where (SubmissionSubjectAreas.IsDeleted is null or SubmissionSubjectAreas.IsDeleted = 'false') and SubmissionSubjectAreas.SubmissionId = Submissions.Id
							and SubjectAreas.IsDeleted = 'false'
							order by SubmissionSubjectAreas.IsPrimary desc, SubmissionSubjectArea asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
						) as 'SelectedSubmissionSubjectAreas',
						Submissions.DomainConflicts,
						Submissions.RootFilePath as 'SubmissionRootFilePath',
						Submissions.Answers as 'SubmissionQuestionsResponse'
					from
					Submissions
					join Tracks on Submissions.TrackId = Tracks.Id
					join Conferences on Tracks.ConferenceId = Conferences.Id
					where 
						Submissions.IsDeleted = 'false'and Submissions.Id = @SubmissionId
						and Tracks.IsDeleted = 'false'
						and Conferences.IsDeleted = 'false'
					group by
						Conferences.FullName,
						Conferences.ShortName,
						Tracks.Name,
						Submissions.Id,
						Submissions.Title,
						Submissions.Abstract,
						Submissions.CreationTime,
						Submissions.LastModificationTime,
						Submissions.DomainConflicts,
						Submissions.RootFilePath,
						Submissions.Answers
				) as SelectedInfoPartSubmission
				-- left join conflicts of interest
				left join
				(
					select 
						SelectedSubmissionConflictedIncumbents.SubmissionId,
						stuff(
						(select ';' + 
							(
								concat(SubmissionConflictedIncumbents.NamePrefix,'|',SubmissionConflictedIncumbents.IncumbentFullName,'|',SubmissionConflictedIncumbents.Organization,'|',SubmissionConflictedIncumbents.Email,'|',SubmissionConflictedIncumbents.SelectedSubmissionConflicts)
							)
							from
							(
								-- Submission Conflicted Incumbents
								select 
									SelectedConflicts.IncumbentId,
									SelectedConflicts.SubmissionId,
									SelectedConflicts.NamePrefix,
									SelectedConflicts.IncumbentFullName,
									SelectedConflicts.Organization,
									SelectedConflicts.Email,
									'['+
									stuff(
									(select ';' + 
										(
											SelectedConflictCases.ConflictCase
										) as 'ConflictCase'
										from
										(
											select 
												Conflicts.IncumbentId,
												Conflicts.SubmissionId,
												ConflictCases.Name as 'ConflictCase'
											from Conflicts
											join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
											join Incumbents on Conflicts.IncumbentId = Incumbents.Id
											join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
											join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
											where 
												Conflicts.SubmissionId = @SubmissionId and Conflicts.IsDefinedByReviewer = 'false' and Conflicts.IsDeleted = 'false'
												and ConflictCases.IsDeleted = 'false'
												and Incumbents.IsDeleted = 'false'
												and ConferenceAccounts.IsDeleted = 'false'
												and AbpUsers.IsDeleted = 'false'
										) as SelectedConflictCases
										order by ConflictCase asc
										for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
									) + ']'
									as 'SelectedSubmissionConflicts'
								from
								(
									select 
										Conflicts.IncumbentId,
										Conflicts.SubmissionId,
										AbpUsers.NamePrefix,
										(
											case
											when AbpUsers.MiddleName is not null
											then AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname
											when AbpUsers.MiddleName is null
											then AbpUsers.Name+' '+AbpUsers.Surname
											end
										) as 'IncumbentFullName',
										AbpUsers.Organization,
										AbpUsers.Email,
										ConflictCases.Name as 'ConflictCase'
									from Conflicts
									join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
									join Incumbents on Conflicts.IncumbentId = Incumbents.Id
									join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
									join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
									where 
										Conflicts.SubmissionId = @SubmissionId and Conflicts.IsDefinedByReviewer = 'false' and Conflicts.IsDeleted = 'false'
										and ConflictCases.IsDeleted = 'false'
										and Incumbents.IsDeleted = 'false'
										and ConferenceAccounts.IsDeleted = 'false'
										and AbpUsers.IsDeleted = 'false'
								) as SelectedConflicts
								group by
									SelectedConflicts.IncumbentId,
									SelectedConflicts.SubmissionId,
									SelectedConflicts.NamePrefix,
									SelectedConflicts.IncumbentFullName,
									SelectedConflicts.Organization,
									SelectedConflicts.Email
							) as SubmissionConflictedIncumbents
							order by SubmissionConflictedIncumbents.Email asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
						) as 'SelectedSubmissionConflictedIncumbents'
					from
					(
						-- Submission Conflicted Incumbents
						select 
							SelectedConflicts.IncumbentId,
							SelectedConflicts.SubmissionId,
							SelectedConflicts.NamePrefix,
							SelectedConflicts.IncumbentFullName,
							SelectedConflicts.Organization,
							SelectedConflicts.Email,
							'['+
							stuff(
							(select ';' + 
								(
									SelectedConflictCases.ConflictCase
								) as 'ConflictCase'
								from
								(
									select 
										Conflicts.IncumbentId,
										Conflicts.SubmissionId,
										ConflictCases.Name as 'ConflictCase'
									from Conflicts
									join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
									join Incumbents on Conflicts.IncumbentId = Incumbents.Id
									join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
									join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
									where 
										Conflicts.SubmissionId = @SubmissionId and Conflicts.IsDefinedByReviewer = 'false' and Conflicts.IsDeleted = 'false'
										and ConflictCases.IsDeleted = 'false'
										and Incumbents.IsDeleted = 'false'
										and ConferenceAccounts.IsDeleted = 'false'
										and AbpUsers.IsDeleted = 'false'
								) as SelectedConflictCases
								order by ConflictCase asc
								for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
							) + ']'
							as 'SelectedSubmissionConflicts'
						from
						(
							select 
								Conflicts.IncumbentId,
								Conflicts.SubmissionId,
								AbpUsers.NamePrefix,
								(
									case
									when AbpUsers.MiddleName is not null
									then AbpUsers.Name+' '+AbpUsers.MiddleName+' '+AbpUsers.Surname
									when AbpUsers.MiddleName is null
									then AbpUsers.Name+' '+AbpUsers.Surname
									end
								) as 'IncumbentFullName',
								AbpUsers.Organization,
								AbpUsers.Email,
								ConflictCases.Name as 'ConflictCase'
							from Conflicts
							join ConflictCases on Conflicts.ConflictCaseId = ConflictCases.Id
							join Incumbents on Conflicts.IncumbentId = Incumbents.Id
							join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
							join AbpUsers on ConferenceAccounts.AccountId = AbpUsers.Id
							where 
								Conflicts.SubmissionId = @SubmissionId and Conflicts.IsDefinedByReviewer = 'false' and Conflicts.IsDeleted = 'false'
								and ConflictCases.IsDeleted = 'false'
								and Incumbents.IsDeleted = 'false'
								and ConferenceAccounts.IsDeleted = 'false'
								and AbpUsers.IsDeleted = 'false'
						) as SelectedConflicts
						group by
							SelectedConflicts.IncumbentId,
							SelectedConflicts.SubmissionId,
							SelectedConflicts.NamePrefix,
							SelectedConflicts.IncumbentFullName,
							SelectedConflicts.Organization,
							SelectedConflicts.Email
					) as SelectedSubmissionConflictedIncumbents
					group by
						SelectedSubmissionConflictedIncumbents.SubmissionId
				) as SelectedConflictsOfInterest
				on SelectedInfoPartSubmission.PaperId = SelectedConflictsOfInterest.SubmissionId
				-- left join revision
				left join
				(
					select
						SubmissionClones.SubmissionId,
						SubmissionClones.CloneNo,
						Revisions.RootFilePath as 'RevisionRootFilePath'
					from SubmissionClones
					join Revisions
					on SubmissionClones.Id = Revisions.Id
					where 
						SubmissionClones.SubmissionId = @SubmissionId and SubmissionClones.IsLast = 'true' and SubmissionClones.IsDeleted = 'false'
						and Revisions.IsDeleted = 'false'
				) as SelectedRevision
				on SelectedInfoPartSubmission.PaperId = SelectedRevision.SubmissionId

			END
            ";

            migrationBuilder.Sql(getSubmissionSummarySP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getSubmissionSummarySP = @"
			DROP PROCEDURE [dbo].[GetSubmissionSummary]
			";

            migrationBuilder.Sql(getSubmissionSummarySP);
        }
    }
}
