namespace DATN.Payload.Request.ScheduleRequest
{
    public class ThemScheduleRequest
    {
        public double Price { get; set; }
        public DateTime StartAt { get; set; }
        public int MovieId { get; set; }
        public string Name { get; set; }
        public int RoomId { get; set; }
    }
}
