namespace DATN.Payload.DTO
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime PremiereDate { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public string Image { get; set; }
        public string HeroImage { get; set; }
        public string Language { get; set; }
        public string MovieTypeName { get; set; }
        public string Name { get; set; }
        public string DanhGia{ get; set; }
        public string Trailer { get; set; }
        public bool? IsActive { get; set; }
    }
    public class MovieRevenueDTO
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } // Thêm thuộc tính này
        public double TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; }
    }


}
