﻿namespace DATN.Payload.Request.PromotionRequest
{
    public class UpdatePromtion
    {
        public int Id { get; set; }
        public int RankCustomerId { get; set; }
        public int Percent { get; set; }
        public int Quantity { get; set; }
        public int Type { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime StartTime { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
       
    }
}