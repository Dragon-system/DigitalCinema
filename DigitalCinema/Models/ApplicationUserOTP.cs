namespace DigitalCinema.Models
{
    public class ApplicationUserOTP
    {
        public int Id { get; set; }
        public string OTP { get; set; } = string.Empty;
        public DateTime CerateAt { get; set; }= DateTime.UtcNow;
        public DateTime ExpirationTime { get; set; } = DateTime.Now.AddMinutes(30);
       
        public bool IsUsed { get; set; } = false;
        public string ApplicationUserId { get; set; } 
        public  ApplicationUser ApplicationUser { get; set; } 
        public bool IsValid => (ExpirationTime - CerateAt).TotalMinutes > 0 && !IsUsed;
    }
}
