using DATN.Payload.DTO;
using DATN.Payload.Request.GeneralSettingRequest;
using DATN.Payload.Request.RankCustomerRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IRankCustomerService
    {
        ResponseObject<RankCustomerDTO> Create(CreateRankCustomerRequest request);
        ResponseObject<RankCustomerDTO> Update(UpdateRankCustomerRequest request);
        ResponseObject<RankCustomerDTO> Delete(int Id);
        IQueryable<RankCustomerDTO> HienThi();
    }
}
