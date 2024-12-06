using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.GeneralSettingRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DATN.Service.Implement
{
    public class GeneralSettingService : IGeneralSettingService
    {
        private readonly AppDbContext dbContext;
        private readonly GeneralSettingConverter generalsettingConverter;
        private readonly ResponseObject<GeneralSettingDTO> responseObject;
        private readonly IConfiguration _configuration;

        public GeneralSettingService(AppDbContext dbContext, GeneralSettingConverter generalsettingConverter, ResponseObject<GeneralSettingDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.generalsettingConverter = generalsettingConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public async Task<ResponseObject<GeneralSettingDTO>> Them(ThemGeneralSettingRequest request)
        {
            GeneralSetting general = new GeneralSetting();
            general.BreakTime = request.BreakTime;
            general.BusinessHours = request.BusinessHours;
            general.CloseTime = request.CloseTime;
            general.FixedTiketPrice = request.FixedTiketPrice;
            general.PercentDay= request.PercentDay;
            general.PercenWeekend= request.PercenWeekend;   
            general.TimeBeginToChage= request.TimeBeginToChage;
            dbContext.generalSettings.Add(general);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", null);

        }
        public async Task<ResponseObject<GeneralSettingDTO>> Sua(SuaGeneralSettingRequest request)
        {
           var check=dbContext.generalSettings.Find(request.Id); 
            if (check != null)
            {
                check.BreakTime = request.BreakTime;
                check.BusinessHours = request.BusinessHours;
                check.CloseTime = request.CloseTime;
                check.FixedTiketPrice= request.FixedTiketPrice;
                check.PercentDay= request.PercentDay;
                check.PercenWeekend = request.PercenWeekend;    
                check.TimeBeginToChage = request.TimeBeginToChage;
                dbContext.generalSettings.Update(check);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công",generalsettingConverter.EntityToDTO(check));
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại id", null);
        }
        public ResponseObject<GeneralSettingDTO> Xoa(XoaGeneralSettingRequest request)
        {
            var check = dbContext.generalSettings.Find(request.Id);
            if (check != null)
            {
                dbContext.generalSettings.Remove(check);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Xoa thanh cong", null);
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy ID", null);

         }
        public IQueryable<GeneralSettingDTO> Hienthi(int pageSize, int pageNumber)
        {
            IQueryable<GeneralSetting> check=dbContext.generalSettings.AsQueryable();
            var result = check.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => generalsettingConverter.EntityToDTO(x));
            return result;
        }
    }
}
