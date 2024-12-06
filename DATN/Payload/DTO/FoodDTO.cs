namespace DATN.Payload.DTO
{
    public class FoodDTO
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string NameOfFood { get; set; }
        public bool? IsActive { get; set; } 
    }
    public class FoodRDTO
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int TotalQuantitySold { get; set; }
    }
}
