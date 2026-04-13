using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DigitalCinema.Repositories.IRepositoreis
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity, CancellationToken cancellationToken = default);

        void Update(T entity);


        Task<int> CommitAsync(CancellationToken cancellationToken = default);



        void Delete(T entity);




        Task<IEnumerable<T>> GetAsync(
           Expression<Func<T, bool>>? expression = null,
           Expression<Func<T, object>>?[]? includes = null,
           bool tracking = true,
           CancellationToken cancellationToken = default);


        Task<T?> GetOneAsync(
             Expression<Func<T, bool>>? expression = null,
             Expression<Func<T, object>>?[]? includes = null,
             bool tracking = true,
             CancellationToken cancellationToken = default);


    }
}



