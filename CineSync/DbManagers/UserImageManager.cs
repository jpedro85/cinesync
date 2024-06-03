using CineSync.Core.Logger;
using CineSync.Core.Repository;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{

    public class UserImageManager : DbManager<UserImage>
    {
        /// <summary>
        /// Initializes a new instance of the UserImageManager class.
        /// </summary>
        /// <param name="unitOfWork">Provides an interface for a unit of work mechanism to handle transactions.</param>
        /// <param name="logger">Provides logging capabilities to trace the operations and errors.</param>
        public UserImageManager(IUnitOfWorkAsync unitOfWork, ILoggerStrategy logger) : base(unitOfWork, logger)
        {
        }


    }
}
