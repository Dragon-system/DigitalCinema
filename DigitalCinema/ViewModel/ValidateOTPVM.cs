using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.ViewModels
{
    public class ValidateOTPVM
    {
        public int Id { get; set; }
        [Required]
        public string OTP { get; set; } = string.Empty;
       
    }
}
