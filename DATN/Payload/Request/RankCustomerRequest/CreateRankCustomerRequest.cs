namespace DATN.Payload.Request.RankCustomerRequest
{
    public class CreateRankCustomerRequest
    {
        public int Point { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; } = true;
    }
}
