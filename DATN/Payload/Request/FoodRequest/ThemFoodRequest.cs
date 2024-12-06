using System.ComponentModel.DataAnnotations;

namespace DATN.Payload.Request.FoodRequest
{
    public class ThemFoodRequest
    {
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string NameOfFood { get; set; }
     
    }
}
