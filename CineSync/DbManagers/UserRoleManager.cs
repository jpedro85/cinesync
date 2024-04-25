using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using CineSync.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CineSync.DbManagers
{
    public class UserRoleManager<TUser> : DbManager<IdentityRole> where TUser : IdentityUser
    {
        private readonly IRepositoryAsync<IdentityUserRole<string>> _userRoleRepository;
        private readonly IRepositoryAsync<TUser> _userRepository; 
        private readonly List<IdentityRole> _roles;
        
        private Lazy<List<IdentityRole>> _lazyRoles;

        public UserRoleManager( IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger)
            : base(unitOfWork, logger)
        {
            _userRoleRepository =  _unitOfWork.GetRepositoryAsync<IdentityUserRole<string>>();
            _userRepository =  _unitOfWork.GetRepositoryAsync<TUser>();
            _roles = GetRoles();
            _lazyRoles = new Lazy<List<IdentityRole>>(() => _repository.GetAll().ToList());
 
        }
        
        public List<IdentityRole> Roles => _lazyRoles.Value;

        public List<IdentityRole> GetRoles()
        {
            return  _repository.GetAll().ToList();
        }

        public async Task<ICollection<IdentityRole>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync() as ICollection<IdentityRole>;
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            IdentityRole? role = _roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }
            
            // TODO: Check the function for other type of includes
            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(ur => ur.RoleId == role.Id && ur.UserId == user.Id, cancellationToken);
            return userRoles.Any();
 
        }

        public async Task<bool> AddRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            if (!await IsInRoleAsync(user, roleName, cancellationToken))
            {
                IdentityRole? role = _roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
                }

                _userRoleRepository.Insert(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
                return await _unitOfWork.SaveChangesAsync();
            }

            _logger.Log($"Tried to add Role {roleName} to a user that already has the role.", LogTypes.WARN);
            return false;
        }

        public async void RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            IdentityRole? role = _roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }

            // TODO: Check the function for other type of includes
            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(
                ur => ur.RoleId == role.Id && ur.UserId == user.Id, cancellationToken);

            foreach (IdentityUserRole<string> userRole in userRoles)
            {
                _userRoleRepository.Delete(userRole);
            }

            await _unitOfWork.SaveChangesAsync(); 
            
            _logger.Log($" Tried to remove Role {roleName} to a user that already has the role.", LogTypes.WARN);
        }
        
        // TODO: Check with ricardo about this implementation
        public async Task<ICollection<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken = default)
        {
 
            /*
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
                */

            // return roles!;
            return null;
        }
        
        // TODO: Check with ricardo about this implementation
        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IdentityRole? role = _roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }
            
            // TODO: Check the function for other type of includes
            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(ur => ur.RoleId == role.Id, cancellationToken);
            IEnumerable<string> userIds = userRoles.Select(ur => ur.UserId).Distinct();
            
            // TODO: Check the function for other type of includes
            IEnumerable<TUser> users = await _userRepository.GetByConditionAsync(
                user => userIds.Contains(user.Id), cancellationToken);
            return users.ToList();
            /*
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
                return null!;*/
        }

        public async Task<bool> ReplaceRoleAsync(TUser user, string actualRoleName, string newRoleName, CancellationToken cancellationToken = default)
        {
            IdentityRole? actualRole = Roles.FirstOrDefault(r => r.Name == actualRoleName);
            IdentityRole? newRole = Roles.FirstOrDefault(r => r.Name == newRoleName);

            if (actualRole == null || newRole == null)
            {
                throw new ArgumentException($"Role does not exist in the database.");
            }

            // TODO: Check with ricardo about this implementation
            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(
                ur => ur.RoleId == actualRole.Id && ur.UserId == user.Id, cancellationToken);

            foreach (IdentityUserRole<string> userRole in userRoles)
            {
                userRole.RoleId = newRole.Id;  // Update the role ID
                _userRoleRepository.Update(userRole);
            }

            return await _unitOfWork.SaveChangesAsync(); 
        }

    }
}
