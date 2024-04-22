using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CineSync.Data
{
    /* 
     * <summary>
     * Class <c>DbInitializer</c> initializes a DbSet<TEntity> with the given Entities.y.
     * <param name="TEntity"> Class type of the Entity Set to initialize.</param
     * </summary>
     */
    public abstract class DbInitializer<TEntity,TUser> where TEntity : class where TUser : IdentityUser
	{
        public IdentityDbContext<TUser> context { get; private set; }

        private List<TEntity> entities;
        public DbInitializer(IdentityDbContext<TUser> context ) 
        { 
            this.context = context;
            this.entities = new List<TEntity>();
        }

        public DbInitializer<TEntity, TUser> WithEntity(TEntity entity )
        {
            if ( !entities.Contains(entity) )
                entities.Add( entity );

            return this;
        }

        public abstract bool isDuplicate( TEntity entity );

        public void Initialize()
        {
            foreach (TEntity entity in entities)
            {
                if ( !isDuplicate( entity ) )
                    context.Add( entity );
            }
            context.SaveChanges();
        }
    }
}
