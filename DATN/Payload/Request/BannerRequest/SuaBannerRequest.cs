namespace DATN.Payload.Request.BannerRequest
{
    public class SuaBannerRequest
    {
        public int Id { get; set; }
        public IFormFile ImageUrl { get; set; }
        public string Title { get; set; }
    }
}
