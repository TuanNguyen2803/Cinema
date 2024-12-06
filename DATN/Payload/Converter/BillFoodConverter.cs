using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class BillFoodConverter
    {
        public BillFoodDTO EntityToDTO(BillFood billfood)
        {
            return new BillFoodDTO
            {
                Id = billfood.Id,
               BillId= billfood.BillId,
               Quantity = billfood.Quantity,
               FoodId = billfood.FoodId,
               
            };
        }
    }
}
