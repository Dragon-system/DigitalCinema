namespace DigitalCinema.Repositories.IRepositoreis
{
    public interface IBockingRepository
    {
        void DeleteRange(IEnumerable<Booking> bookings);
    }
}
