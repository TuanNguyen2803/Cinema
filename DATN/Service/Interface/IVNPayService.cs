using DATN.Payload.DTO;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IVNPayService
    {
        string TaoDuongDanThanhToan(int hoaDonId, HttpContext httpContext, int userid);
        string VNPayReturn(IQueryCollection vnpayData);
    }
}
