using CineSync.Core.Logger;
using CineSync.Core.Logger.Enums;
using CineSync.Core.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace CineSync.DbManagers
{
    /// <summary>
    /// Manages user roles, providing functionality to assign and manipulate roles for users in the system.
    /// </summary>
    /// <typeparam name="TUser">The type of the user object, derived from IdentityUser.</typeparam>
    public class UserRoleManager<TUser> : DbManager<IdentityRole> where TUser : IdentityUser
    {
        private readonly IRepositoryAsync<IdentityUserRole<string>> _userRoleRepository;
        private readonly IRepositoryAsync<TUser> _userRepository;

        private Lazy<IEnumerable<IdentityRole>> _lazyRoles;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleManager{TUser}"/> class that manages user roles using Identity framework.
        /// </summary>
        /// <param name="unitOfWork">Provides a way to access the repositories and commit changes to the database.</param>
        /// <param name="logger">The logger used to log information, warnings, and errors during operations.</param>
        public UserRoleManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger)
            : base(unitOfWork, logger)
        {
            _userRoleRepository = _unitOfWork.GetRepositoryAsync<IdentityUserRole<string>>();
            _userRepository = _unitOfWork.GetRepositoryAsync<TUser>();
            _lazyRoles = new Lazy<IEnumerable<IdentityRole>>(() => Task.Run(async () => await _repository.GetAllAsync()).Result);
        }

        public IEnumerable<IdentityRole> Roles => _lazyRoles.Value;

        /// <summary>
        /// Retrieves all roles from the database.
        /// </summary>
        /// <returns>A list of all IdentityRole instances.</returns>
        public List<IdentityRole> GetRoles()
        {
            return _repository.GetAll().ToList();
        }

        /// <summary>
        /// Asynchronously retrieves all roles from the database.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of IdentityRole.</returns>
        public async Task<ICollection<IdentityRole>> GetRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllAsync() as ICollection<IdentityRole>;
        }

        /// <summary>
        /// Asynchronously retrieves all roles associated with a specific user.
        /// </summary>
        /// <param name="user">The user whose roles are to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation, returning a collection of role names.</returns>
        public async Task<ICollection<string>> GetRolesOfUserAsync(TUser user, CancellationToken cancellationToken = default)
        {
            IEnumerable<IdentityUserRole<string>> roles = await _userRoleRepository.GetByConditionAsync(userRole => userRole.UserId == user.Id, cancellationToken);
            IEnumerable<string> roleIds = roles.Select(ur => ur.RoleId).Distinct();
            IEnumerable<IdentityRole> userRoles = Roles.Where(role => roleIds.Contains(role.Id));
            ImmutableList<string> userRolesNames = userRoles.Select(role => role.Name).ToImmutableList();

            return userRolesNames;
        }

        /// <summary>
        /// Determines asynchronously if a user is in a specified role.
        /// </summary>
        /// <param name="user">The user to check the role for.</param>
        /// <param name="roleName">The name of the role to check.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation, returning true if the user is in the role; otherwise false.</returns>
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

        /// <summary>
        /// Adds a user to a specified role asynchronously.
        /// </summary>
        /// <param name="user">The user to add to the role.</param>
        /// <param name="roleName">The name of the role to add the user to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation, returning true if the role was added successfully; otherwise false.</returns>
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

        /// <summary>
        /// Removes a user from a specified role asynchronously.
        /// </summary>
        /// <param name="user">The user to remove from the role.</param>
        /// <param name="roleName">The name of the role to remove the user from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
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

        /// <summary>
        /// Retrieves all users in a specified role asynchronously.
        /// </summary>
        /// <param name="roleName">The name of the role to retrieve users from.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation, returning a list of users in the specified role.</returns>
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

        /// <summary>
        /// Replaces a user's current role with a new role asynchronously.
        /// </summary>
        /// <param name="user">The user whose role is to be replaced.</param>
        /// <param name="actualRoleName">The current role name to replace.</param>
        /// <param name="newRoleName">The new role name to assign.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
        /// <returns>A task that represents the asynchronous operation, returning true if the role replacement was successful; otherwise false.</returns>
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
