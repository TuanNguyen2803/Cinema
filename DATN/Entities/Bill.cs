namespace DATN.Entities
{
    public class Bill : BaseEntity
    {
        public double TotalMoney { get; set; }
        public string TradingCode { get; set; }
        public DateTime CreateTime { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public DateTime? UpdateTime{ get; set; }= DateTime.Now;
        public int PromotionId { get; set; }
        public int BillStatusId { get; set; }
        public bool? IsActive { get; set; } = true;
        public IEnumerable<BillFood>? BillFoods { get; set; }
        public IEnumerable<BillTicket>? BillTickets { get; set; }
        public IEnumerable<BillStatus>? BillStatus { get; set; }

    }
}
