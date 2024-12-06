using DATN.Payload.DTO;
using DATN.Payload.Request.PromotionRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IPromotionService
    {
        ResponseObject<PromotionDTO> Them(CreatePromotion request);
        ResponseObject<PromotionDTO> Sua(UpdatePromtion request);
        ResponseObject<PromotionDTO> Delete(int promotionId);
        IQueryable<PromotionDTO> GetAll();
        IQueryable<PromotionDTO> hienthitheoid(int promotionid);
    }
}
