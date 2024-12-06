using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.FoodRequest;
using DATN.Payload.Response;

namespace DATN.Service.Interface
{
    public interface IFoodService
    {
        Task<ResponseObject<FoodDTO>> Them(ThemFoodRequest request);
        Task<ResponseObject<FoodDTO>> Sua(SuaFoodRequest request);
        ResponseObject<FoodDTO> Xoa(int foodid);
        IQueryable<FoodDTO> Hienthi();
        IQueryable<FoodDTO> Hienthitheoid(int foodid);
    }
}
