using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.RankCustomerRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    public class RankCustomerService : IRankCustomerService
    {
        private readonly AppDbContext _context;
        private readonly ResponseObject<RankCustomerDTO> responseObject;
        private readonly RankCustomerConverter rankCustomerConverter;
        private readonly IConfiguration _configuration;

        public RankCustomerService(AppDbContext context, ResponseObject<RankCustomerDTO> responseObject, RankCustomerConverter rankCustomerConverter, IConfiguration configuration)
        {
            _context = context;
            this.responseObject = responseObject;
            this.rankCustomerConverter = rankCustomerConverter;
            _configuration = configuration;
        }
        public ResponseObject<RankCustomerDTO> Create(CreateRankCustomerRequest request)
        {
           if(string.IsNullOrWhiteSpace(request.Name))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            RankCustomer rank = new RankCustomer
            {
                Point = request.Point,
                Description = request.Description,
                Name = request.Name,
                IsActive = true
            };
            _context.rankCustomers.Add(rank);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", rankCustomerConverter.EntityToDTO(rank));
        }

        public ResponseObject<RankCustomerDTO> Delete(int Id)
        {
            var check=_context.rankCustomers.FirstOrDefault(x=>x.Id == Id);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại rankcustomerid này", null);
            }
            _context.rankCustomers.Remove(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công", null);
            
        }

        public IQueryable<RankCustomerDTO> HienThi()
        {
            var check=_context.rankCustomers.AsQueryable();
            var result=check.Select(x=>rankCustomerConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<RankCustomerDTO> Update(UpdateRankCustomerRequest request)
        {
            var check = _context.rankCustomers.Find(request.Id);
            if(check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại rankcustomerid này", null);
            }
            check.Point= request.Point;
            check.Description= request.Description;
            check.Name= request.Name;
            check.IsActive= request.IsActive;
            _context.rankCustomers.Update(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công",rankCustomerConverter.EntityToDTO(check));
        }
    }
}
