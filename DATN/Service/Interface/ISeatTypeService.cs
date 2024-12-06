using DATN.Payload.DTO;
using DATN.Payload.Request.SeatType;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface ISeatTypeService
    {
        ResponseObject<SeatTypeDTO> Create(ThemSeatTypeRequest request);
        ResponseObject<SeatTypeDTO> Update(SuaSeatTypeRequest request);
        ResponseObject<SeatTypeDTO> Delete(XoaSeatTypeRequest request);
        IQueryable<SeatTypeDTO> HienThi(int pageSize,int pageNumber);
    }
}
