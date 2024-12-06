using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class RateConverter
    {
        public RateDTO EntityToDTO(Rate rate)
        {
            return new RateDTO
            {
                Id = rate.Id,
                Description = rate.Description,
                Code = rate.Code,
            };
        }
    }
}
