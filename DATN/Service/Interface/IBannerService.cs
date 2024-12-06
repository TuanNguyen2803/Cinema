using DATN.Payload.DTO;
using DATN.Payload.Request.BannerRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IBannerService
    {
        Task<ResponseObject<BannerDTO>> ThemBanner(ThemBannerRequest request);
        Task<ResponseObject<BannerDTO>> SuaBanner(SuaBannerRequest request);
        ResponseObject<BannerDTO> XoaBanner(int bannerid);
        IQueryable<BannerDTO> Hienthi();
        IQueryable<BannerDTO> Hienthitheoid(int bannerid);
    }
}
