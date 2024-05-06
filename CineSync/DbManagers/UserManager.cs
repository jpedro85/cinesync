using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data;

namespace CineSync.DbManagers
{
    public class UserManager : DbManager<ApplicationUser>
    {

        public UserManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
        }

        public async Task<bool> ChangeUsernameAsync(string userId, string username)
        {
            ApplicationUser user = await GetFirstByConditionAsync(u => u.Id == userId);
            if (user == null || string.IsNullOrEmpty(username) || await GetFirstByConditionAsync(user => user.UserName == username) != null)
            {
                return false;
            }

            user.UserName = username;
            return await _unitOfWork.SaveChangesAsync();
        }

    }
}
