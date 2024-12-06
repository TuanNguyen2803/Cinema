using DATN.Payload.DTO;
using DATN.Payload.Request.GeneralSettingRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IGeneralSettingService
    {
        Task<ResponseObject<GeneralSettingDTO>> Them(ThemGeneralSettingRequest request);
        Task<ResponseObject<GeneralSettingDTO>> Sua(SuaGeneralSettingRequest request);
        ResponseObject<GeneralSettingDTO> Xoa(XoaGeneralSettingRequest request);
        IQueryable<GeneralSettingDTO> Hienthi(int pageSize, int pageNumber);
    }
}
