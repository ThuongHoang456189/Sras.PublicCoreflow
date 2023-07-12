using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration51 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getReviewerAssignmentSuggestionSubmissionPartSP = @"
			CREATE OR ALTER PROCEDURE [dbo].[GetReviewerAssignmentSuggestionSubmissionPart]
			@SubmissionId uniqueidentifier
			AS
			BEGIN
				select 
					Submissions.Id as 'PaperId',
					Submissions.Title,
					Tracks.Id as 'TrackId',
					Tracks.Name as 'TrackName',
					Tracks.SubjectAreaRelevanceCoefficients,
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
					) as 'SelectedSubmissionSubjectAreas'
				from
					Submissions
					join Tracks on Submissions.TrackId = Tracks.Id
				where
					Submissions.IsDeleted = 'false'and Submissions.Id = @SubmissionId
					and Tracks.IsDeleted = 'false'
				group by
					Submissions.Id,
					Submissions.Title,
					Tracks.Id,
					Tracks.Name,
					Tracks.SubjectAreaRelevanceCoefficients
			END
            ";

            migrationBuilder.Sql(getReviewerAssignmentSuggestionSubmissionPartSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getReviewerAssignmentSuggestionSubmissionPartSP = @"
			DROP PROCEDURE [dbo].[GetReviewerAssignmentSuggestionSubmissionPart]
			"
             ;

            migrationBuilder.Sql(getReviewerAssignmentSuggestionSubmissionPartSP);
        }
    }
}
