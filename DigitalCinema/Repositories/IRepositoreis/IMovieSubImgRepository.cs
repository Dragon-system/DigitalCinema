using Microsoft.EntityFrameworkCore;

namespace DigitalCinema.Repositories.IRepositoreis
{
    public interface IMovieSubImgRepository : IRepository<SupImg> 
    {
        void DeleteRange(IEnumerable<SupImg> movieSubImg);
     
    }
}
