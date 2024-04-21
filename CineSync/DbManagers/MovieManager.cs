using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class MovieManager(ApplicationDbContext dbContext) : DbManager<Movie>(dbContext)
	{

	}
}
