using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class BillConverter
    {
        public BillDTO EntityToDTO(Bill bill)
        {
            return new BillDTO
            {
                Id = bill.Id,
                TotalMoney = bill.TotalMoney,
                Name = bill.Name,
                CreateTime = bill.CreateTime,
                UserId = bill.UserId,
                TradingCode = bill.TradingCode,   
                PromotionId = bill.PromotionId,
                UpdateTime = bill.UpdateTime,
                BillStatusId=bill.BillStatusId,
            };
        }
    }
}
