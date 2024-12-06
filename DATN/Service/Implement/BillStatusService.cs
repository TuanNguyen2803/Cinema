using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.BillStatusRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    public class BillStatusService : IBillStatusService
    {
        private readonly AppDbContext dbContext;
        private readonly BillStatusConverter billstatusConverter;
        private readonly ResponseObject<BillStatusDTO> responseObject;
        private readonly IConfiguration _configuration;

        public BillStatusService(AppDbContext dbContext, BillStatusConverter billstatusConverter, ResponseObject<BillStatusDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.billstatusConverter = billstatusConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }


        public IQueryable<BillStatusDTO> HienThi(int? id, string? name, int pageSize, int pageNumber)
        {

            IQueryable<BillStatus> check = dbContext.billStatuses.AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                check = check.Where(x => x.Name.ToLower().Contains(name.ToLower()));
            }
            if (id.HasValue)
            {
                check=check.Where(x=>x.Id == id.Value);
            }
            var result = check.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => billstatusConverter.EntityToDTO(x));
            return result;
        }
        public async Task<ResponseObject<BillStatusDTO>> Sua(SuaBillStatusRequest request)
        {
            var check=dbContext.billStatuses.Find(request.Id);
            if(check != null)
            {
                check.Name = request.Name;
                dbContext.billStatuses.Update(check);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công",billstatusConverter.EntityToDTO(check));
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại id", null);
        }
        public async Task<ResponseObject<BillStatusDTO>> Them(ThemBillStatusRequest request)
        {
            BillStatus billStatus = new BillStatus();
            billStatus.Name=request.Name;
            dbContext.billStatuses.Add(billStatus);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", billstatusConverter.EntityToDTO(billStatus));          
        }
        public ResponseObject<BillStatusDTO> Xoa(XoaBillStatusRequest request)
        {
            var check = dbContext.billStatuses.Find(request.Id);
            if (check != null)
            {
                dbContext.billStatuses.Remove(check);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Xóa thành công", null);
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại id", null);
        }
    }
}
