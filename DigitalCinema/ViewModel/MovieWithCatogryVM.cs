namespace DigitalCinema.ViewModel
{
    public class MovieWithCatogryVM
    {
        // الأفلام اللي هتتعرض في الصفحة
        public List<Movie> Movies { get; set; } = new();

        // عدد الأفلام في كل فئة للفلترة الجانبية
        public Dictionary<string, int> CategoryWithTotal { get; set; } = new();

        // الفئة المختارة حالياً (للـ active state في الفلتر)
        public string? SelectedCategory { get; set; }
    }
}
