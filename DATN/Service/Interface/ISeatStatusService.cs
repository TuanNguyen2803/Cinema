using DATN.Payload.DTO;
using DATN.Payload.Request.SeatStatusRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface ISeatStatusService
    {
        ResponseObject<SeatStatusDTO> Create(CreateSeatStatus request);
        ResponseObject<SeatStatusDTO> Update(UpdateSeatStatus request); 
        ResponseObject<SeatStatusDTO> Delete(DeleteSeatStatus request);
        IQueryable<SeatStatusDTO> GetAll(int pageSize,int pageNumber);
    }
}
