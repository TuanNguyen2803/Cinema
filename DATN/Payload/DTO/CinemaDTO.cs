namespace DATN.Payload.DTO
{
    public class CinemaDTO
    {
        public int Id {  get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string NameOfCinema { get; set; }
        public bool? IsAtive { get; set; }
    }
    public class CinemaMonthlyRevenueDTO
    {
        public int Year { get; set; } // Năm
        public int Month { get; set; } // Tháng
        public string CinemaName { get; set; } // Tên rạp
        public double TotalRevenue { get; set; } // Tổng doanh thu
    }


    public class CinemaStatisticsDTO
    {
        public int? CinemaId { get; set; }
        public string CinemaName { get; set; }
        public int TotalTicketsSold { get; set; }
        public double TotalRevenue { get; set; }
    }
    public class CinemaRevenueDTO
    {
        public int? CinemaId { get; set; }
        public string CinemaName { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public class CinemaScheduleDTO
    {
        public string CinemaName { get; set; } // Tên của rạp chiếu
        public List<ScheduleDTO> Schedules { get; set; } // Danh sách các lịch chiếu trong rạp này
    }


}
