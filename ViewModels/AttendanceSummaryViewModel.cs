namespace Campus360.ViewModels
{
    public class AttendanceSummaryViewModel
    {
        public string CourseName { get; set; }
        public int TotalClasses { get; set; }
        public int Presents { get; set; }
        public decimal Percentage { get; set; }
    }
}
