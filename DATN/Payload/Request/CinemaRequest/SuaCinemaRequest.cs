namespace DATN.Payload.Request.CinemaRequest
{
    public class SuaCinemaRequest
    {
        public int ID { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string NameOfCinema { get; set; }
        public bool IsActive { get; set; }
    }
}
