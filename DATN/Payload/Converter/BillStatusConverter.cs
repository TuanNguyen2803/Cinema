using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class BillStatusConverter
    {
        public BillStatusDTO EntityToDTO(BillStatus billStatus)
        {
            return new BillStatusDTO()
            {
                Id = billStatus.Id,
                Name = billStatus.Name,
            };
        }
    }
}
