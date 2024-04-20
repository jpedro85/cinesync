using CineSync.Data;
using CineSync.Data.Models;

namespace CineSync.DbManagers
{
	public class MovieManager<IMovie>(ApplicationDbContext dbContext) : DbManager<Movie>(dbContext)
	{

	}
}
