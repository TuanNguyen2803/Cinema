namespace DATN.Payload.Request.Room
{
    public class ThemRoomRequest
    {
        public int Capacity { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
        public int CinemaId { get; set; }
        public string Name { get; set; }
    }
}
