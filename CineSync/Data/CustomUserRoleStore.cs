using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Collections.Generic;

namespace CineSync.Data
{
    public class CustomUserRoleStore : IUserRoleStore<ApplicationUser>
    {

		readonly ApplicationDbContext DbContext;

		readonly IUserStore<ApplicationUser> UserStore;

		public CustomUserRoleStore( IUserStore<ApplicationUser> UserStore, ApplicationDbContext DbContext)
        {
            this.DbContext = DbContext;
            this.UserStore= UserStore;
        }

		public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
			return Task.Run( async () => 
			{ 
				List<string?> roles = await DbContext.Roles.Select(role => role.Name).ToListAsync( cancellationToken ); 
					
				foreach (var role in roles) 
				{
					if ( role == roleName)
					{
						IdentityRole? roleToAdd = await DbContext.Roles.FirstOrDefaultAsync( role => role.Name == roleName, cancellationToken);

						if (roleToAdd != null) 
						{ 
							DbContext.UserRoles.Add(new IdentityUserRole<string>() { RoleId = roleToAdd.Id, UserId = user.Id });
							await DbContext.SaveChangesAsync();
							return;

						}else
							throw new ApplicationException($"Could not find Role {roleName}.");
					}
				}

				throw new ArgumentException($" Role {roleName} doesn´t exist on data base.");

			} );
        }

		public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
		{

			var roles = await DbContext.Roles
				.Join(
					DbContext.UserRoles, r => r.Id , ur => ur.RoleId,
					(r,ur) => new { 
						RoleName = r.Name,
						UserRoleUserId = ur.UserId

					}  )
				.Where( roleUserRole => roleUserRole.UserRoleUserId == user.Id).Select(p => p.RoleName)
				.Distinct()
				.ToListAsync( cancellationToken );

			return roles!;

		}

		public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
		{
			var users = await DbContext.Roles
				.Where( role => role.Name == roleName)
				.Join(
					DbContext.UserRoles, role => role.Id, uRole => uRole.RoleId,
					(role, uRole) => new {
						UserRoleUserId = uRole.UserId
					})
				.Join(
					DbContext.Users, uRole => uRole.UserRoleUserId, user => user.Id,
					(uRole, user) => new {
						User = user
					})
				.Select(p => p.User)
				.Distinct()
				.ToListAsync(cancellationToken);

			return users!;
		}

		public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
		{
			return Task.Run( async () =>
			{
				var users = await DbContext.Roles
				.Where(role => role.Name == roleName)
				.Join(
					DbContext.UserRoles, role => role.Id, uRole => uRole.RoleId,
					(role, uRole) => new {
						UserRoleUserId = uRole.UserId
					})
				.Join(
					DbContext.Users, uRole => uRole.UserRoleUserId, u => u.Id,
					(uRole, u) => new {
						User = u
					})
				.Select(p => p.User)
				.Distinct()
				.Where(p => p.Id == user.Id)
				.ToListAsync(cancellationToken);

				return users != null;
			});
		}

		public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
		{
			return Task.Run(async () =>
			{
				if ( await IsInRoleAsync(user, roleName, cancellationToken) )
				{
					var result = await DbContext.UserRoles
						.Join(
							DbContext.Roles, ur => ur.RoleId, r => r.Id,
							(ur, r) => new
							{
								UserRoleRoleId = ur.RoleId,
								UserRoleuserId = ur.UserId,
								RoleName = r.Name,
								UersRole = ur
							})
						.Where(p => p.UserRoleuserId == user.Id)
						.Select(p=>p.UersRole)
						.ToListAsync(cancellationToken);

					foreach( var identityUserRole in result)
					{
						DbContext.UserRoles.Remove(identityUserRole);
					}

					DbContext.SaveChanges();
				}

			});
		}

		public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
			return UserStore.CreateAsync(user, cancellationToken);
		}

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return UserStore.DeleteAsync(user, cancellationToken);
        }

        public void Dispose()
        {
            UserStore.Dispose();
		}

        public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
			return UserStore.FindByIdAsync(userId, cancellationToken);
		}

        public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
			return UserStore.FindByNameAsync(normalizedUserName, cancellationToken);
		}

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
			return UserStore.GetNormalizedUserNameAsync(user, cancellationToken);
		}

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
			return UserStore.GetUserIdAsync(user, cancellationToken);
		}

        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
			return UserStore.GetUserNameAsync(user, cancellationToken);
		}

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
			return UserStore.SetNormalizedUserNameAsync(user, normalizedName, cancellationToken);
		}

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            return UserStore.SetUserNameAsync(user, userName, cancellationToken);
		}

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
			return UserStore.UpdateAsync(user, cancellationToken);
		}
    }
}
