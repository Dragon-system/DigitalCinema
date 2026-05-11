using System.ComponentModel.DataAnnotations;

namespace DigitalCinema.ViewModel
{
    public class BookingConfirmVM
    {
        // بيانات الـ Show — hidden fields
        public int ShowId { get; set; }
        public string MovieTitle { get; set; }
        public string HallName { get; set; }
        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }

        // المقاعد المختارة كـ string "3,7,12"
        [Required(ErrorMessage = "اختر مقعداً على الأقل")]
        public string SelectedShowSeatIds { get; set; }

        // أرقام المقاعد للعرض "A1","B3"
        public List<string> SelectedSeatNumbers { get; set; } = new();

        // بيانات اليوزر
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        public string UserPhone { get; set; }

        // Computed
        public List<int> ShowSeatIdList
        {
            get
            {
                var list = new List<int>();
                if (string.IsNullOrEmpty(SelectedShowSeatIds)) return list;
                foreach (var s in SelectedShowSeatIds.Split(','))
                    if (int.TryParse(s.Trim(), out int id)) list.Add(id);
                return list;
            }
        }

        public decimal TotalPrice => ShowSeatIdList.Count * TicketPrice;
    }
}
