using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineSync.DbManagers
{
    public class UserRoleManager<TUser> : DbManager<IdentityRole> where TUser : IdentityUser
    {
        private readonly List<IdentityRole> _roles;

        public UserRoleManager(ApplicationDbContext dbContext, ILoggerStrategy logger)
            : base(dbContext, logger)
        {
            _roles = GetRoles();
        }

        public List<IdentityRole> GetRoles()
        {
            return _dbContext.Roles.ToList();
        }

        public async Task<ICollection<IdentityRole>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Roles.ToListAsync(cancellationToken);
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            foreach (var role in _roles)
            {
                if (role.Name == roleName)
                {
                    var userRole = await _dbContext.UserRoles
                        .Where(userRole => userRole.RoleId == role.Id && userRole.UserId == user.Id)
                        .Select(p => p)
                        .ToListAsync(cancellationToken);

                    return userRole != null;
                }
            }

            throw new ArgumentException($" Role {roleName} doesn´t exist on data base.");
        }

        public async Task<bool> AddRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {

            if (!await IsInRoleAsync(user, roleName, cancellationToken))
            {
                foreach (var role in _roles)
                {
                    if (role.Name == roleName)
                    {
                        _dbContext.UserRoles.Add(new IdentityUserRole<string>() { UserId = user.Id, RoleId = role.Id });
                        return await _dbContext.SaveChangesAsync() > 0;
                    }
                }

                throw new ArgumentException($" Role {roleName} doesn´t exist on data base.");
            }

            _logger.Log($" Tryed to add Role {roleName} to a user that already has the role.", LogTypes.WARN);
            return true;
        }

        public async Task<ICollection<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
            var roles = await _dbContext.Roles
                .Join(
                    _dbContext.UserRoles, r => r.Id, ur => ur.RoleId,
                    (r, ur) => new
                    {
                        RoleName = r.Name,
                        UserRoleUserId = ur.UserId

                    })
                .Where(roleUserRole => roleUserRole.UserRoleUserId == user.Id)
                .Select(p => p.RoleName)
                .Distinct()
                .ToListAsync(cancellationToken);

            return roles!;
        }

        public async void RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            if (await IsInRoleAsync(user, roleName, cancellationToken))
            {
                var result = await _dbContext.UserRoles
                    .Join(
                        _dbContext.Roles, ur => ur.RoleId, r => r.Id,
                        (ur, r) => new
                        {
                            UserRoleRoleId = ur.RoleId,
                            UserRoleuserId = ur.UserId,
                            RoleName = r.Name,
                            UersRole = ur
                        })
                    .Where(p => p.UserRoleuserId == user.Id)
                    .Select(p => p.UersRole)
                    .ToListAsync(cancellationToken);

                foreach (var identityUserRole in result)
                {
                    _dbContext.UserRoles.Remove(identityUserRole);
                }

                _dbContext.SaveChanges();
            }

            _logger.Log($" Tryed to remove Role {roleName} to a user that already has the role.", LogTypes.WARN);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            var users = await _dbContext.Roles
                .Where(role => role.Name == roleName)
                .Join(
                    _dbContext.UserRoles, role => role.Id, uRole => uRole.RoleId,
                    (role, uRole) => new
                    {
                        UserRoleUserId = uRole.UserId
                    })
                .Join(
                    _dbContext.Users, uRole => uRole.UserRoleUserId, user => user.Id,
                    (uRole, user) => new
                    {
                        User = user
                    })
                .Select(p => p.User)
                .Distinct()
                .ToListAsync(cancellationToken);

            if (users != null)
                return users.Cast<TUser>().ToList();
            else
                return null!;
        }

        public async Task<bool> ReplaceRoleAsync(TUser user, string actualRoleName, string newRoleName, CancellationToken cancellationToken = default)
        {
            foreach (var newRole in _roles)
            {
                if (newRole.Name == newRoleName)
                {
                    foreach (var role in _roles)
                    {
                        if (role.Name == actualRoleName)
                        {
                            var results = await _dbContext.UserRoles
                                .Where(ur => ur.RoleId == role.Id && ur.UserId == user.Id)
                                .Select(p => p)
                                .ToListAsync();

                            foreach (var result in results)
                            {
                                result.RoleId = newRole.Id;
                                _dbContext.Update(result);
                            }
                            return await _dbContext.SaveChangesAsync() > 0;
                        }
                    }

                    throw new ArgumentException($" Role {actualRoleName} doesn´t exist on data base.");
                }

            }

            throw new ArgumentException($" Role {newRoleName} doesn´t exist on data base.");
        }

    }
}
