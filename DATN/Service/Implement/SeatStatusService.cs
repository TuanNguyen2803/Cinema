using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.SeatStatusRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    public class SeatStatusService : ISeatStatusService
    {
        private readonly AppDbContext _context;
        private readonly SeatStatusConverter seatStatusConverter;
        private readonly ResponseObject<SeatStatusDTO> responseObject;
        private readonly IConfiguration _configuration;

        public SeatStatusService(AppDbContext context, SeatStatusConverter seatStatusConverter, ResponseObject<SeatStatusDTO> responseObject, IConfiguration configuration)
        {
            _context = context;
            this.seatStatusConverter = seatStatusConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public ResponseObject<SeatStatusDTO> Create(CreateSeatStatus request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.NameStatus))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            SeatStatus ss= new SeatStatus();
            ss.Code = request.Code;
            ss.NameStatus = request.NameStatus;
            _context.seatsStatus.Add(ss);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", seatStatusConverter.EntityToDTO(ss));
        }

        public ResponseObject<SeatStatusDTO> Delete(DeleteSeatStatus request)
        {
            var check = _context.seatsStatus.Find(request.Id);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại seatStatus id {request.Id}", null);
            }
           
            _context.seatsStatus.Remove(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công",null);
        }

        public IQueryable<SeatStatusDTO> GetAll(int pageSize, int pageNumber)
        {
           var check= _context.seatsStatus.AsQueryable();
            var result = check.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => seatStatusConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<SeatStatusDTO> Update(UpdateSeatStatus request)
        {
            var check = _context.seatsStatus.Find(request.Id);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại seatStatus id {request.Id}", null);
            }
            check.NameStatus = request.NameStatus;
            check.Code = request.Code;
            _context.seatsStatus.Update(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công",seatStatusConverter.EntityToDTO(check));
        }
    }
}
