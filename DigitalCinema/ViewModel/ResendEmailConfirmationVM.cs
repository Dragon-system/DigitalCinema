using System.ComponentModel.DataAnnotations;
namespace DigitalCinema.ViewModel
{
    public class ResendEmailConfirmationVM
    {
        public int Id { get; set; }
        [Required]
        public string EmailOrUsername { get; set; } = string.Empty;
    }
}
