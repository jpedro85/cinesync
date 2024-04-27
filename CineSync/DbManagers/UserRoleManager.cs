using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace CineSync.DbManagers
{
    public class UserRoleManager<TUser> : DbManager<IdentityRole> where TUser : IdentityUser
    {
        private readonly IRepositoryAsync<IdentityUserRole<string>> _userRoleRepository;
        private readonly IRepositoryAsync<TUser> _userRepository;

        private Lazy<IEnumerable<IdentityRole>> _lazyRoles;

        public UserRoleManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger)
            : base(unitOfWork, logger)
        {
            _userRoleRepository = _unitOfWork.GetRepositoryAsync<IdentityUserRole<string>>();
            _userRepository = _unitOfWork.GetRepositoryAsync<TUser>();
            _lazyRoles = new Lazy<IEnumerable<IdentityRole>>(() => Task.Run(async () => await _repository.GetAllAsync()).Result);
        }

        public IEnumerable<IdentityRole> Roles => _lazyRoles.Value;

        public List<IdentityRole> GetRoles()
        {
            return _repository.GetAll().ToList();
        }

        public async Task<ICollection<IdentityRole>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync() as ICollection<IdentityRole>;
        }

        public async Task<ICollection<string>> GetRolesOfUserAsync(TUser user, CancellationToken cancellationToken = default)
        {
            IEnumerable<IdentityUserRole<string>> roles = await _userRoleRepository.GetByConditionAsync(userRole => userRole.UserId == user.Id, cancellationToken);
            IEnumerable<string> roleIds = roles.Select(ur => ur.RoleId).Distinct();
            IEnumerable<IdentityRole> userRoles = Roles.Where(role => roleIds.Contains(role.Id));
            ImmutableList<string> userRolesNames = userRoles.Select(role => role.Name).ToImmutableList();

            return userRolesNames;
        }

        public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            IdentityRole? role = Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }

            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(ur => ur.RoleId == role.Id && ur.UserId == user.Id, cancellationToken);
            return userRoles.Any();

        }

        public async Task<bool> AddRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default)
        {
            if (!await IsInRoleAsync(user, roleName, cancellationToken))
            {
                IdentityRole? role = Roles.FirstOrDefault(r => r.Name == roleName);
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
            IdentityRole? role = Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }

            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(
                ur => ur.RoleId == role.Id && ur.UserId == user.Id, cancellationToken);

            foreach (IdentityUserRole<string> userRole in userRoles)
            {
                _userRoleRepository.Delete(userRole);
            }

            await _unitOfWork.SaveChangesAsync();

            _logger.Log($" Tried to remove Role {roleName} to a user that already has the role.", LogTypes.WARN);
        }

        public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IdentityRole? role = Roles.FirstOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new ArgumentException($"Role {roleName} doesn't exist in the database.");
            }

            IEnumerable<IdentityUserRole<string>> userRoles = await _userRoleRepository.GetByConditionAsync(ur => ur.RoleId == role.Id, cancellationToken);
            IEnumerable<string> userIds = userRoles.Select(ur => ur.UserId).Distinct();

            IEnumerable<TUser> users = await _userRepository.GetByConditionAsync(
                user => userIds.Contains(user.Id), cancellationToken);
            return users.ToList();
        }

        public async Task<bool> ReplaceRoleAsync(TUser user, string actualRoleName, string newRoleName, CancellationToken cancellationToken = default)
        {
            IdentityRole? actualRole = Roles.FirstOrDefault(r => r.Name == actualRoleName);
            IdentityRole? newRole = Roles.FirstOrDefault(r => r.Name == newRoleName);

            if (actualRole == null || newRole == null)
            {
                throw new ArgumentException($"Role does not exist in the database.");
            }

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
