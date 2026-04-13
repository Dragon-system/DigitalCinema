using DigitalCinema.Repositories.IRepositoreis;
using DigitalCinema.Repositories;

namespace DigitalCinema.Repositories
{
    public class MovieSubImgRepository : Repository<SupImg> , IMovieSubImgRepository
    {
        public MovieSubImgRepository(ApplicationDbContext context)
        : base(context)
        { 
        
        }
        public void DeleteRange(IEnumerable<SupImg> movieSubImg)
        {
            foreach (var item in movieSubImg)
            {
                _context.SupImgs.Remove(item);
            }
        }
    }
}
