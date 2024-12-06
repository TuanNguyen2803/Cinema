using System.ComponentModel.DataAnnotations;

namespace DATN.Payload.Request.FoodRequest
{
    public class SuaFoodRequest
    {
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
        public int Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public string NameOfFood { get; set; }
        public bool? IsActive { get; set; }


    }
}
