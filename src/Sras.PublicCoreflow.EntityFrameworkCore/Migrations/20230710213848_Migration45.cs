using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var updateActivityTimelineSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[UpdateActivityTimeline]
			@UTCNowStr nvarchar(20)
			AS
			BEGIN
				begin transaction
					declare
						@CurrentTrackId uniqueidentifier

					declare 
						@ActiveTrackTable table
						(
							OrderId int identity(1,1),
							TrackId uniqueidentifier
						)
					insert into @ActiveTrackTable
					select Tracks.Id
					from Tracks
					join Conferences on Tracks.ConferenceId = Conferences.Id
					where Conferences.IsDeleted = 'false'
					and Tracks.IsDeleted = 'false'

					declare @order_id int = 1
					declare @max_order_id int = (select isnull(max(OrderId), 0) from @ActiveTrackTable)
					while (@order_id <= @max_order_id)
					begin
						select @CurrentTrackId = TrackId
						from @ActiveTrackTable
						where OrderId = @order_id
		
						execute [dbo].UpdateTrackActivityTimeline @TrackId=@CurrentTrackId, @UTCNowStr=@UTCNowStr

						set @order_id = @order_id + 1
					end
				commit transaction
			END
            ";

            migrationBuilder.Sql(updateActivityTimelineSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var updateActivityTimelineSP = @"
			DROP PROCEDURE [dbo].[UpdateActivityTimeline]
			";

            migrationBuilder.Sql(updateActivityTimelineSP);
        }
    }
}
