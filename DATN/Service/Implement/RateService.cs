using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.RateRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace DATN.Service.Implement
{
    public class RateService : IRateService
    {
        private readonly AppDbContext dbContext;
        private readonly RateConverter rateConverter;
        private readonly ResponseObject<RateDTO> responseObject;
        private readonly IConfiguration _configuration;

        public RateService(AppDbContext dbContext, RateConverter rateConverter, ResponseObject<RateDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.rateConverter = rateConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

  

        public ResponseObject<RateDTO> CreateRate(CreateRate request)
        {
            if(string.IsNullOrWhiteSpace(request.Description)|| string.IsNullOrWhiteSpace(request.Code))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            Rate rate=new Rate();
            rate.Description = request.Description;
            rate.Code = request.Code;
            dbContext.rates.Add(rate);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công",rateConverter.EntityToDTO(rate));
        }

        public ResponseObject<RateDTO> DeleteRate(DeleteRate request)
        {
            var rate = dbContext.rates.Find(request.Id);
            if(rate == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy rateId", null);
            }
            dbContext.rates.Remove(rate);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công", null);
        }

        public IQueryable<RateDTO> HienThi()
        {
            IQueryable < Rate> check= dbContext.rates.AsQueryable();
            var result = check.Select(x => rateConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<RateDTO> UpdateRate(UpdateRate request)
        {
            var rate = dbContext.rates.Find(request.Id);
            if (rate == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy rateId", null);
            }
            rate.Description = request.Description;
            rate.Code = request.Code;
            dbContext.rates.Update(rate);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công",rateConverter.EntityToDTO(rate));
        }
    }
}
