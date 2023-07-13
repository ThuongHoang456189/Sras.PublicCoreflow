using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sras.PublicCoreflow.Migrations
{
    /// <inheritdoc />
    public partial class Migration52 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var getConferenceUserSP = @"
            CREATE OR ALTER PROCEDURE [dbo].[GetConferenceUser]
            @InclusionText nvarchar(1024),
			@ConferenceId uniqueidentifier,
			@TrackId uniqueidentifier,
			@ConferenceRoleId uniqueidentifier,
			@SkipCount int,
			@MaxResultCount int
			AS
			BEGIN

			declare 
				@TotalCount int


			select 
				@TotalCount = count(*)
			from
			(
				-- selected ConferenceAccountId with SelectedRoles
				select
					ConferenceAccounts.Id as 'ConferenceAccountId',
					ConferenceAccounts.AccountId,
					stuff(
						(select ';' + 
							(
								ConferenceRoles.Name
							) as 'Role'
							from ConferenceRoles
							join 
							(
								select 
									ConferenceAccounts.Id as 'ConferenceAccountId',
									Incumbents.ConferenceRoleId
								from Incumbents
								join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
								join Conferences on ConferenceAccounts.ConferenceId = Conferences.Id
								join ConferenceRoles on Incumbents.ConferenceRoleId = ConferenceRoles.Id
								where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
								and ConferenceAccounts.IsDeleted = 'false'
								and Incumbents.IsDeleted = 'false'
								and (@TrackId is null or (@TrackId is not null and Incumbents.TrackId = @TrackId))
								and (@ConferenceRoleId is null or (@ConferenceRoleId is not null and Incumbents.ConferenceRoleId = @ConferenceRoleId))
								group by
									Incumbents.ConferenceRoleId,
									ConferenceAccounts.Id
							) as SelectedIncumbents
							on SelectedIncumbents.ConferenceRoleId = ConferenceRoles.Id
							where SelectedIncumbents.ConferenceAccountId = ConferenceAccounts.Id
							order by ConferenceRoles.Name asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
					) as 'SelectedRoles'
				from Incumbents
				join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
				join Conferences on ConferenceAccounts.ConferenceId = Conferences.Id
				join ConferenceRoles on Incumbents.ConferenceRoleId = ConferenceRoles.Id
				where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
				and ConferenceAccounts.IsDeleted = 'false'
				and Incumbents.IsDeleted = 'false'
				and (@TrackId is null or (@TrackId is not null and Incumbents.TrackId = @TrackId))
				and (@ConferenceRoleId is null or (@ConferenceRoleId is not null and Incumbents.ConferenceRoleId = @ConferenceRoleId))
				group by
					ConferenceAccounts.Id,
					ConferenceAccounts.AccountId
			) as SelectedConferenceAccountIdWithRolesConferenceAccount
			join AbpUsers
			on SelectedConferenceAccountIdWithRolesConferenceAccount.AccountId = AbpUsers.Id
			where @InclusionText is null or (
				lower(AbpUsers.Name) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.MiddleName) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Surname) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Email) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Organization) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Country) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedConferenceAccountIdWithRolesConferenceAccount.SelectedRoles) like '%'+lower(@InclusionText)+'%'
			)

			select 
				@TotalCount as 'TotalCount',
				SelectedConferenceAccountIdWithRolesConferenceAccount.ConferenceAccountId,
				SelectedConferenceAccountIdWithRolesConferenceAccount.AccountId,
				AbpUsers.Name as 'FirstName',
				AbpUsers.MiddleName,
				AbpUsers.Surname as 'LastName',
				AbpUsers.Email,
				AbpUsers.Organization,
				AbpUsers.Country,
				SelectedConferenceAccountIdWithRolesConferenceAccount.SelectedRoles
			from
			(
				-- selected ConferenceAccountId with SelectedRoles
				select
					ConferenceAccounts.Id as 'ConferenceAccountId',
					ConferenceAccounts.AccountId,
					ConferenceAccounts.CreationTime,
					ConferenceAccounts.LastModificationTime,
					stuff(
						(select ';' + 
							(
								ConferenceRoles.Name
							) as 'Role'
							from ConferenceRoles
							join 
							(
								select 
									ConferenceAccounts.Id as 'ConferenceAccountId',
									Incumbents.ConferenceRoleId
								from Incumbents
								join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
								join Conferences on ConferenceAccounts.ConferenceId = Conferences.Id
								join ConferenceRoles on Incumbents.ConferenceRoleId = ConferenceRoles.Id
								where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
								and ConferenceAccounts.IsDeleted = 'false'
								and Incumbents.IsDeleted = 'false'
								and (@TrackId is null or (@TrackId is not null and Incumbents.TrackId = @TrackId))
								and (@ConferenceRoleId is null or (@ConferenceRoleId is not null and Incumbents.ConferenceRoleId = @ConferenceRoleId))
								group by
									Incumbents.ConferenceRoleId,
									ConferenceAccounts.Id
							) as SelectedIncumbents
							on SelectedIncumbents.ConferenceRoleId = ConferenceRoles.Id
							where SelectedIncumbents.ConferenceAccountId = ConferenceAccounts.Id
							order by ConferenceRoles.Name asc
							for xml path(''), type).value('.', 'nvarchar(2048)'),1,1,''
					) as 'SelectedRoles'
				from Incumbents
				join ConferenceAccounts on Incumbents.ConferenceAccountId = ConferenceAccounts.Id
				join Conferences on ConferenceAccounts.ConferenceId = Conferences.Id
				join ConferenceRoles on Incumbents.ConferenceRoleId = ConferenceRoles.Id
				where Conferences.Id = @ConferenceId and Conferences.IsDeleted = 'false'
				and ConferenceAccounts.IsDeleted = 'false'
				and Incumbents.IsDeleted = 'false'
				and (@TrackId is null or (@TrackId is not null and Incumbents.TrackId = @TrackId))
				and (@ConferenceRoleId is null or (@ConferenceRoleId is not null and Incumbents.ConferenceRoleId = @ConferenceRoleId))
				group by
					ConferenceAccounts.Id,
					ConferenceAccounts.AccountId,
					ConferenceAccounts.CreationTime,
					ConferenceAccounts.LastModificationTime
			) as SelectedConferenceAccountIdWithRolesConferenceAccount
			join AbpUsers
			on SelectedConferenceAccountIdWithRolesConferenceAccount.AccountId = AbpUsers.Id
			where @InclusionText is null or (
				lower(AbpUsers.Name) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.MiddleName) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Surname) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Email) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Organization) like '%'+lower(@InclusionText)+'%' or
				lower(AbpUsers.Country) like '%'+lower(@InclusionText)+'%' or
				lower(SelectedConferenceAccountIdWithRolesConferenceAccount.SelectedRoles) like '%'+lower(@InclusionText)+'%'
			)
			order by
				case when SelectedConferenceAccountIdWithRolesConferenceAccount.LastModificationTime is not null then SelectedConferenceAccountIdWithRolesConferenceAccount.LastModificationTime end desc,
				SelectedConferenceAccountIdWithRolesConferenceAccount.CreationTime
			offset @SkipCount rows
			fetch next @MaxResultCount rows only
			END
            ";

            migrationBuilder.Sql(getConferenceUserSP);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var getConferenceUserSP = @"
			DROP PROCEDURE [dbo].[GetConferenceUser]
			"
            ;

            migrationBuilder.Sql(getConferenceUserSP);
        }
    }
}
