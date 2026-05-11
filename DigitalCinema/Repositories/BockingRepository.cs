using DigitalCinema.Repositories.IRepositoreis;
using DigitalCinema.Repositories;

namespace DigitalCinema.Repositories
{
    public class BockingRepository : Repository<Booking> , IBockingRepository
    {
        public BockingRepository(ApplicationDbContext context)
        : base(context)
        { 
        
        }
        public void DeleteRange(IEnumerable<Booking> bookings)
             =>   _context.Bookings.RemoveRange(bookings);
         
       
    }
}
