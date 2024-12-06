using DATN.Payload.DTO;
using DATN.Payload.Request.BillStatusRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IBillStatusService
    {
        Task<ResponseObject<BillStatusDTO>> Them(ThemBillStatusRequest request);
        Task<ResponseObject<BillStatusDTO>> Sua(SuaBillStatusRequest request);
        ResponseObject<BillStatusDTO> Xoa(XoaBillStatusRequest request);
        IQueryable<BillStatusDTO> HienThi( int? id,string? name,int pageSize, int pageNumber);
    }
}
