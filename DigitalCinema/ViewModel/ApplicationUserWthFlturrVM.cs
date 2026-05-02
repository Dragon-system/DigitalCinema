namespace DigitalCinema.ViewModels
{
    public class ApplicationUserWthFlturrVM
    {
        public Dictionary<ApplicationUser, string> UsersRoles { get; set; } = null!;

        public double TotalPages { get; set; }
        public int CurrentPage { get; set; }
        //public string Query { get; set; } 
    }
}
