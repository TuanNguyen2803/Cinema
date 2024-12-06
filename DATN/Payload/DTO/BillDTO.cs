namespace DATN.Payload.DTO
{
    public class BillDTO
    {
        public int Id { get; set; }
        public double TotalMoney { get; set; }
        public string TradingCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int PromotionId { get; set; }
        public int BillStatusId { get; set; }
    }
    public class RevenueByMonthDTO
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTicketsSold { get; set; } // Thêm thuộc tính này
    }

}
