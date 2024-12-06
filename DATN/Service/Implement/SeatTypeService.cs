using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.SeatType;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    public class SeatTypeService : ISeatTypeService
    {
        private readonly AppDbContext _context;
        private readonly SeatTypeConverter seatTypeConverter;
        private readonly ResponseObject<SeatTypeDTO> responseObject;
        private readonly IConfiguration _configuration;

        public SeatTypeService(AppDbContext context, SeatTypeConverter seatTypeConverter, ResponseObject<SeatTypeDTO> responseObject, IConfiguration configuration)
        {
            this._context = context;
            this.seatTypeConverter = seatTypeConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public ResponseObject<SeatTypeDTO> Create(ThemSeatTypeRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.NameType))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            SeatType seatType=new SeatType();
            seatType.NameType = request.NameType;
            _context.seatTypes.Add(seatType);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công",seatTypeConverter.EntityToDTO(seatType));
        }

        public ResponseObject<SeatTypeDTO> Delete(XoaSeatTypeRequest request)
        {
            var check = _context.seatTypes.Find(request.Id);
            if(check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại seatType", null);
            }
            _context.seatTypes.Remove(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công", null);
        }

        public IQueryable<SeatTypeDTO> HienThi(int pageSize, int pageNumber)
        {
            IQueryable<SeatType> check = _context.seatTypes.AsQueryable();
            var result=check.Skip((pageNumber-1)*pageSize).Take(pageSize).Select(x=>seatTypeConverter.EntityToDTO(x));
            return result;
        }

        public ResponseObject<SeatTypeDTO> Update(SuaSeatTypeRequest request)
        {
            var check = _context.seatTypes.Find(request.Id);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại seatType", null);
            }
            check.NameType = request.NameType;
            _context.seatTypes.Update(check);
            _context.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công", seatTypeConverter.EntityToDTO(check));
        }
    }
}
