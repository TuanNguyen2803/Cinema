﻿namespace DATN.Payload.Request.SeatRequest
{
    public class SuaSeatRequest
    {
        public int ID { get; set; }
        public int Number { get; set; }
        public int SeatStatusId { get; set; }
        public string Line { get; set; }
        public int RoomId { get; set; }
        public bool? IsActive { get; set; }
        public int SeatTypeId { get; set; }
    }
}