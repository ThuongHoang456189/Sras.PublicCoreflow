using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration44 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var updateTrackActivityTimelineSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[UpdateTrackActivityTimeline]
			@TrackId uniqueidentifier,
			@UTCNowStr nvarchar(20)
			AS
			BEGIN
				if exists
				(
					select ActivityDeadlines.*
					from ActivityDeadlines
					join Tracks on ActivityDeadlines.TrackId = Tracks.Id
					join Conferences on Tracks.ConferenceId = Conferences.Id
					where 
						ActivityDeadlines.TrackId = @TrackId and ActivityDeadlines.IsDeleted = 'false'
						and Tracks.Id = @TrackId and Tracks.IsDeleted = 'false'
						and Conferences.IsDeleted = 'false'
				)
				begin
					begin transaction
						declare
							@UTCNow datetime2(7),
							@LocalTimeZoneId nvarchar(512),
							@LocalNow datetime2(7),
							@LocalToday datetime2(7),
							@OldCurrentPlanDeadline datetime2(7),
							@OldMaxNextFactor int,
							@NewNextPlanDeadline datetime2(7)

						select @UTCNow = convert(datetime2(7), @UTCNowStr, 126)

						select @LocalTimeZoneId = Conferences.TimeZoneId
						from Conferences
						join Tracks on Conferences.Id = Tracks.ConferenceId
						where Tracks.Id = @TrackId

						select @LocalNow = cast((@UTCNow at time zone 'UTC') at time zone @LocalTimeZoneId as datetime2(7))

						select @LocalToday = cast(cast(@LocalNow as date) as datetime2(7))

						select @OldCurrentPlanDeadline = ActivityDeadlines.PlanDeadline
						from ActivityDeadlines
						where ActivityDeadlines.IsCurrent = 'true'
						and ActivityDeadlines.TrackId = @TrackId
						order by ActivityDeadlines.Factor asc
						offset 0 rows
						fetch next 1 rows only

						select @OldMaxNextFactor = ActivityDeadlines.Factor
						from ActivityDeadlines
						where ActivityDeadlines.IsNext = 'true'
						and ActivityDeadlines.TrackId = @TrackId
						order by ActivityDeadlines.Factor desc
						offset 0 rows
						fetch next 1 rows only

						select @NewNextPlanDeadline = ActivityDeadlines.PlanDeadline
						from ActivityDeadlines
						where ActivityDeadlines.Factor = @OldMaxNextFactor + 1
						and ActivityDeadlines.TrackId = @TrackId
						order by ActivityDeadlines.Factor asc
						offset 0 rows
						fetch next 1 rows only

						-- update completed status
						update ActivityDeadlines
						set 
							ActivityDeadlines.Status = 2,
							ActivityDeadlines.CompletionTime = ActivityDeadlines.Deadline,
							ActivityDeadlines.LastModificationTime = @UTCNow
						where
							ActivityDeadlines.Status = 1
							and cast(cast(ActivityDeadlines.Deadline as date) as datetime2(7)) < @LocalToday
							and ActivityDeadlines.TrackId = @TrackId

						if @LocalToday > cast(cast(@OldCurrentPlanDeadline as date) as datetime2(7))
						begin
							-- update current
							update ActivityDeadlines
							set 
								ActivityDeadlines.IsCurrent = 'false',
								ActivityDeadlines.LastModificationTime = @UTCNow
							where
								ActivityDeadlines.IsCurrent = 'true'
								and ActivityDeadlines.TrackId = @TrackId

							update ActivityDeadlines
							set 
								ActivityDeadlines.IsNext = 'false',
								ActivityDeadlines.IsCurrent = 'true',
								ActivityDeadlines.LastModificationTime = @UTCNow
							where
								ActivityDeadlines.IsNext = 'true'
								and ActivityDeadlines.TrackId = @TrackId

							-- update next
							update ActivityDeadlines
							set 
								ActivityDeadlines.IsNext = 'true',
								ActivityDeadlines.LastModificationTime = @UTCNow
							where
								cast(cast(ActivityDeadlines.PlanDeadline as date) as datetime2(7)) = cast(cast(@NewNextPlanDeadline as date) as datetime2(7))
								and ActivityDeadlines.TrackId = @TrackId
						end
					commit transaction
				end
			END
            ";

			migrationBuilder.Sql(updateTrackActivityTimelineSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var updateTrackActivityTimelineSP = @"
			DROP PROCEDURE [dbo].[UpdateTrackActivityTimeline]
			";

            migrationBuilder.Sql(updateTrackActivityTimelineSP);
        }
    }
}
