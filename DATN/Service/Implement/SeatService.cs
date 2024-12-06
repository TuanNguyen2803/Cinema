using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.SeatRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DATN.Service.Implement
{
    public class SeatService : ISeatService
    {
        private readonly AppDbContext dbContext;
        private readonly SeatConverter seatConverter;
        private readonly ResponseObject<SeatDTO> responseObject;
        private readonly IConfiguration _configuration;

        public SeatService(AppDbContext dbContext, SeatConverter seatConverter, ResponseObject<SeatDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.seatConverter = seatConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public IQueryable<SeatDTO> Hienthi()
        {
            IQueryable<Seat> check = dbContext.seats.AsQueryable();
            var result = check.Select(x => seatConverter.EntityToDTO(x));

            return result;
        }

        public IQueryable<SeatDTO> hienthitheoid(int seatid)
        {
            var check = dbContext.seats
                           .Where(s => s.Id == seatid);
            var result = check.Select(x => seatConverter.EntityToDTO(x));

            return result;
        }

        public IQueryable<SeatDTO> hienthitheophong(int roomid)
        {
            var check = dbContext.seats
                            .Where(s => s.RoomId == roomid && s.IsActive == true);
            //.Select(s => new
            //{
            //    s.Number,
            //    s.Line,
            //    s.SeatStatusId,
            //    SeatType = s.SeatType.NameType
            //});
            var result = check.Select(x => seatConverter.EntityToDTO(x));    

            return result;
        }

        public ResponseObject<SeatDTO> Sua(SuaSeatRequest request)
        {
            var room = dbContext.rooms.FirstOrDefault(x => x.Id == request.RoomId);
            var seatstatus = dbContext.seatsStatus.FirstOrDefault(x => x.Id == request.SeatStatusId);
            var seattpye = dbContext.seatTypes.FirstOrDefault(x => x.Id == request.SeatTypeId);
            var check = dbContext.seats.FirstOrDefault(x => x.Id == request.ID);

            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy ghế", null);
            }

            if (room is null || seatstatus is null || seattpye is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại dữ liệu", null);
            }

            // Check for duplicate seat number in the same line
            var duplicateSeat = dbContext.seats.FirstOrDefault(s => s.Line == request.Line && s.RoomId == request.RoomId && s.Number == request.Number && s.Id != request.ID);
            if (duplicateSeat != null)
            {
                return responseObject.ResponseError(StatusCodes.Status409Conflict, "Ghế với số này đã tồn tại trong cùng một hàng", null);
            }

            check.Number = request.Number;
            check.SeatStatusId = request.SeatStatusId;
            check.Line = request.Line;
            check.RoomId = request.RoomId;
            check.SeatTypeId = request.SeatTypeId;
            check.IsActive = request.IsActive;

            dbContext.seats.Update(check);
            dbContext.SaveChanges();

            return responseObject.ResponseSuccess("Sửa thành công", seatConverter.EntityToDTO(check));
        }

        public ResponseObject<SeatDTO> Them(ThemSeatRequest request)
        {
            var room = dbContext.rooms.FirstOrDefault(x => x.Id == request.RoomId);
            var seatstatus = dbContext.seatsStatus.FirstOrDefault(x => x.Id == request.SeatStatusId);
            var seattpye = dbContext.seatTypes.FirstOrDefault(x => x.Id == request.SeatTypeId);

            if (string.IsNullOrWhiteSpace(request.Line)
               || string.IsNullOrWhiteSpace(request.Number.ToString())
               || string.IsNullOrWhiteSpace(request.SeatStatusId.ToString())
               || string.IsNullOrWhiteSpace(request.RoomId.ToString())
               || string.IsNullOrWhiteSpace(request.SeatTypeId.ToString())
               )
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }

            if (room is null || seatstatus is null || seattpye is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại dữ liệu", null);
            }

            // Check for duplicate seat number in the same line
            var duplicateSeat = dbContext.seats.FirstOrDefault(s => s.Line == request.Line && s.RoomId == request.RoomId && s.Number == request.Number);
            if (duplicateSeat != null)
            {
                return responseObject.ResponseError(StatusCodes.Status409Conflict, "Ghế với số này đã tồn tại trong cùng một hàng", null);
            }

            Seat seat = new Seat
            {
                Number = request.Number,
                SeatStatusId = request.SeatStatusId,
                Line = request.Line,
                RoomId = request.RoomId,
                SeatTypeId = request.SeatTypeId,
                IsActive = request.IsActive
            };

            dbContext.seats.Add(seat);
            dbContext.SaveChanges();

            return responseObject.ResponseSuccess("Thêm thành công", seatConverter.EntityToDTO(seat));
        }

        public ResponseObject<List<SeatDTO>> ThemGheTuFileExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "File không hợp lệ", null);
            }

            var seats = new List<Seat>();

            try
            {
                // Đọc file Excel
                using (var stream = file.OpenReadStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Sử dụng bản miễn phí cho mục đích không thương mại
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets["SheetGhe"];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng 2, bỏ qua tiêu đề
                        {
                            var roomName = worksheet.Cells[row, 1].Value?.ToString().Trim(); // Tên phòng ở cột 1
                            var line = worksheet.Cells[row, 2].Value?.ToString().Trim();     // Hàng ghế ở cột 2
                            var number = Convert.ToInt32(worksheet.Cells[row, 3].Value?.ToString().Trim()); // Số ghế ở cột 3
                            var seatTypeName = worksheet.Cells[row, 4].Value?.ToString().Trim(); // Loại ghế ở cột 4

                            // Tìm kiếm RoomId dựa trên tên phòng
                            var room = dbContext.rooms.Where(r =>r.Name.ToLower() == roomName.ToLower())
                                                        .FirstOrDefault();
                            if (room == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Phòng với tên '{roomName}' không tồn tại", null);
                            }

                            // Tìm kiếm SeatTypeId dựa trên tên loại ghế
                            var seatType = dbContext.seatTypes.Where(s=>s.NameType.ToLower() == seatTypeName.ToLower())
                                                        .FirstOrDefault();
                            if (seatType == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Loại ghế '{seatTypeName}' không tồn tại", null);
                            }

                            // Luôn đặt SeatStatusId là 1 (ví dụ như trạng thái "Available")
                            int seatStatusId = 1;

                            // Luôn đặt IsActive là true
                            bool isActive = true;

                            // Kiểm tra trùng lặp ghế trong cùng hàng và phòng
                            var duplicateSeat = dbContext.seats.FirstOrDefault(s => s.Line == line && s.RoomId == room.Id && s.Number == number);
                            if (duplicateSeat != null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status409Conflict, $"Ghế với số '{number}' đã tồn tại trong hàng '{line}' của phòng '{roomName}'", null);
                            }

                            // Tạo ghế mới
                            Seat seat = new Seat
                            {
                                Number = number,
                                Line = line,
                                RoomId = room.Id,
                                SeatTypeId = seatType.Id,
                                SeatStatusId = seatStatusId, // Trạng thái ghế luôn là 1
                                IsActive = isActive // Trạng thái kích hoạt luôn là true
                            };

                            seats.Add(seat);
                        }
                    }
                }

                dbContext.seats.AddRange(seats);
                dbContext.SaveChanges();

                // Chuyển đổi thành DTOs và trả về phản hồi thành công
                var seatDTOs = seats.Select(s => seatConverter.EntityToDTO(s)).ToList();
                return responseObject.ResponseSuccessList("Thêm ghế từ file Excel thành công", seatDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi xử lý file: {ex.Message}", null);
            }
        }
        public ResponseObject<SeatDTO> Xoa(int seatid)
        {
            var check = dbContext.seats.FirstOrDefault(x => x.Id == seatid);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tim thay seat ", null);
            }
            check.IsActive = false;
            dbContext.seats.Update(check);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xoa thanh cong",seatConverter.EntityToDTO(check));
        }
 

    }
}
