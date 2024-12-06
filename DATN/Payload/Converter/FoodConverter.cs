using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class FoodConverter
    {
        public FoodDTO EntityToDTO(Food food)
        {

            return new FoodDTO
            {
                Id = food.Id,
                Price = food.Price,
                Description = food.Description,
                Image = food.Image,
                NameOfFood = food.NameOfFood,
                IsActive = food.IsActive,
            };
        }
        public List<FoodDTO> ListEntityToDTO(List<Food> listfood)
        {
            var listFoodDTO = new List<FoodDTO>();
            foreach (var item in listfood)
            {
                listFoodDTO.Add(EntityToDTO(item));
            }

            return listFoodDTO;
        }
    }
}
