namespace DATN.Payload.Request.RankCustomerRequest
{
    public class UpdateRankCustomerRequest
    {
        public int Id { get; set; } 
        public int Point { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }
}
