using DATN.Payload.DTO;
using DATN.Payload.Request.RateRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IRateService
    {
        ResponseObject<RateDTO> CreateRate(CreateRate request);
        ResponseObject<RateDTO> UpdateRate(UpdateRate request);
        ResponseObject<RateDTO> DeleteRate(DeleteRate request);
        IQueryable<RateDTO> HienThi();
    }
}
