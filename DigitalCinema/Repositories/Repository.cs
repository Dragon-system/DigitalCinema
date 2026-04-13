using DigitalCinema.Repositories.IRepositoreis;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading;

namespace DigitalCinema.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        //CRUD

        public async Task CreateAsync(T entity , CancellationToken cancellationToken = default) 
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity) 
        {
            _dbSet.Update(entity);
        }

        public async Task<int> CommitAsync( CancellationToken cancellationToken = default) 
        {
        
            try
            {
               return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Handle exceptions as needed
               Console.WriteLine($" Error : {ex.Message}");
                return 0; // Indicate failure
            }
        }
        

        public void Delete(T entity) 
        {
            _context.Remove(entity);
        }
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>?[]? includes = null,
            bool tracking = true,
            CancellationToken cancellationToken = default)//get all data
        {
            var entities = _dbSet.AsQueryable();
            if (expression is not null)
                entities = entities.Where(expression);

            if (includes is not  null)
            {
                foreach (var item in includes)
                {   if (item is not null)
                        entities = entities.Include(item);
                }
            }

            if (!tracking)
                entities = entities.AsNoTracking();

            return await entities.ToListAsync(cancellationToken);
        }
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null, 
            Expression<Func<T, object>>?[]? includes = null, 
            bool tracking = true,
            CancellationToken cancellationToken = default)
        {
            return (await GetAsync(expression, includes, tracking, cancellationToken)).FirstOrDefault();
        }

       
    }
}
