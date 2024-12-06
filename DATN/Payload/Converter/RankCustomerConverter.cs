using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class RankCustomerConverter
    {
        public RankCustomerDTO EntityToDTO(RankCustomer rankCustomer)
        {
            return new RankCustomerDTO
            {
                Id = rankCustomer.Id,
                Point = rankCustomer.Point,
                Description = rankCustomer.Description,
                Name = rankCustomer.Name,
                IsActive = rankCustomer.IsActive,
            };
        }
    }
}
