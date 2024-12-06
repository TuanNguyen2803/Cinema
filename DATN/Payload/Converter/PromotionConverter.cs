using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class PromotionConverter
    {
        public PromotionDTO EntityToDTO(Promotion promotion)
        {
            return new PromotionDTO
            {
                Id = promotion.Id,
                Percent= promotion.Percent,
                Quantity= promotion.Quantity,
                Type = promotion.Type,
                StartTime = promotion.StartTime,
                EndTime = promotion.EndTime,
                Description = promotion.Description,
                Name = promotion.Name,
                IsActive = promotion.IsActive,
                RankCustomerId = promotion.RankCustomerId,
                RankCustomerName=promotion.RankCustomer?.Name,
            };
        }
    }
}
