using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.Room;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DATN.Service.Implement
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext dbContext;
        private readonly RoomConverter roomConverter;
        private readonly ResponseObject<RoomDTO> responseObject;
        private readonly IConfiguration _configuration;

        public RoomService(AppDbContext dbContext, RoomConverter roomConverter, ResponseObject<RoomDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.roomConverter = roomConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public IQueryable<RoomDTO> GetRooms(int? cinemaid)
        {
            // Bắt đầu với việc truy vấn tất cả các phòng
            IQueryable<Room> query = dbContext.rooms
                      .Include(room => room.Cinema) // Bao gồm thông tin Cinema
                      .AsNoTracking() // Không theo dõi thay đổi để cải thiện hiệu suất
                      .AsQueryable();

            // Nếu cinemaid có giá trị, lọc các phòng theo cinemaid
            if (cinemaid.HasValue)
            {
                query = query.Where(room => room.CinemaId == cinemaid.Value);
            }

            // Chuyển đổi các phòng thành DTO và trả về kết quả
            var result = query.Select(room => roomConverter.EntityToDTO(room));
            return result;
        }

        public IQueryable<RoomDTO> Hienthiroomtat()
        {
            IQueryable<Room> query = dbContext.rooms
                      .Include(room => room.Cinema) // Bao gồm thông tin Cinema
                      .AsNoTracking() // Không theo dõi thay đổi để cải thiện hiệu suất
                      .AsQueryable();

            // Chọn các thuộc tính để chuyển đổi thành RoomDTO
            var result = query.Select(room => roomConverter.EntityToDTO(room));

            return result;
        }

        public IQueryable<RoomDTO> Hienthitheoid(int roomid)
        {
            IQueryable<Room> query = dbContext.rooms.Where(x=>x.Id==roomid).AsQueryable();

            var result = query.Select(room => roomConverter.EntityToDTO(room));
            return result;
        }

        public ResponseObject<RoomDTO> Sua(SuaRoomRequest request)
        {
            var room = dbContext.rooms.FirstOrDefault(x => x.Id == request.ID);
            if(room is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Khong ton tai room", null);
            }
            else
            {
                var checkcinema = dbContext.cinemas.FirstOrDefault(x => x.Id == request.CinemaId);
                if(checkcinema is null)
                {
                    return responseObject.ResponseError(StatusCodes.Status404NotFound, "Cinema khong ton tai", null);
                }
                
                room.Capacity= request.Capacity;
                room.Name= request.Name;
                room.Description= request.Description;
                room.Type= request.Type;
                room.CinemaId= request.CinemaId;
                room.Code = "Room_" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);
                dbContext.rooms.Update(room);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sua thanh cong", roomConverter.EntityToDTO(room));

            }

        }
        public ResponseObject<RoomDTO> Them(ThemRoomRequest request)
        {
            var check = dbContext.cinemas.FirstOrDefault(x => x.Id == request.CinemaId);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Cinema khong ton tai", null);
            }
            if (string.IsNullOrWhiteSpace(request.Description)
                || string.IsNullOrWhiteSpace(request.Name)

                )
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui long dien day du thong tin", null);
            }
            else
            {
                Room room = new Room();
                room.Capacity = request.Capacity;
                room.Type = request.Type;
                room.Description = request.Description;
                room.CinemaId = request.CinemaId;
                room.Code = "Room_" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);
                room.Name = request.Name;
                dbContext.rooms.Add(room);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Them thanh cong", roomConverter.EntityToDTO(room));
            }
        }
        public ResponseObject<List<RoomDTO>> ImportRoomFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "File không hợp lệ", null);
            }

            var rooms = new List<Room>();

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets["Sheetroom"];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            var cinemaName = worksheet.Cells[row, 1].Value?.ToString().Trim();
                            var roomName = worksheet.Cells[row, 2].Value?.ToString().Trim();
                            var capacityString = worksheet.Cells[row, 3].Value?.ToString().Trim();
                            var roomTypeString = worksheet.Cells[row, 4].Value?.ToString().Trim();
                            var roomDescription = worksheet.Cells[row, 5].Value?.ToString().Trim(); // Lấy mô tả từ cột 5

                            // Nếu roomDescription là null hoặc trống, gán giá trị mặc định
                            if (string.IsNullOrWhiteSpace(roomDescription))
                            {
                                roomDescription = "Không có mô tả"; // Gán giá trị mặc định
                            }
                            // Cung cấp giá trị mặc định

                            // Kiểm tra sức chứa
                            if (string.IsNullOrWhiteSpace(capacityString) || !int.TryParse(capacityString, out var capacity))
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, $"Sức chứa không hợp lệ tại hàng {row}", null);
                            }

                            // Xác định loại phòng (Type)
                            int roomType;
                            if (roomTypeString == "2D")
                            {
                                roomType = 2;
                            }
                            else if (roomTypeString == "3D")
                            {
                                roomType = 3;
                            }
                            else
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, $"Loại phòng '{roomTypeString}' không hợp lệ tại hàng {row}", null);
                            }

                            var cinema = dbContext.cinemas.FirstOrDefault(c => c.NameOfCinema.ToLower() == cinemaName.ToLower());

                            if (cinema == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Cinema với tên '{cinemaName}' không tồn tại tại hàng {row}", null);
                            }

                            var code = "Room_" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);

                            Room newRoom = new Room
                            {
                                CinemaId = cinema.Id,
                                Name = roomName,
                                Capacity = capacity,
                                Type = roomType,
                                Description = roomDescription, // Thêm mô tả vào đối tượng phòng
                                Code = code
                            };

                            rooms.Add(newRoom);
                        }
                    }
                }

                dbContext.rooms.AddRange(rooms);
                dbContext.SaveChanges();

                var roomDTOs = rooms.Select(r => roomConverter.EntityToDTO(r)).ToList();
                return responseObject.ResponseSuccessList("Thêm phòng từ file Excel thành công", roomDTOs);
            }
            catch (DbUpdateException dbEx)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi lưu dữ liệu: {dbEx.InnerException?.Message ?? dbEx.Message}", null);
            }
            catch (Exception ex)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi xử lý file: {ex.Message}", null);
            }
        }


        public ResponseObject<RoomDTO> Xoa(int roomid)
        {
            var room = dbContext.rooms.SingleOrDefault(x => x.Id == roomid);
            if (room is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Khong ton tai room", null);
            }
            else
            {
                room.IsActive = false;
                dbContext.rooms.Update(room);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Xoa thanh cong", null);
            }
        }
        public IQueryable<RoomDTO> TimKiemRoom(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<RoomDTO>().AsQueryable(); // Trả về danh sách trống nếu từ khóa rỗng
            }

            // Chuẩn hóa từ khóa tìm kiếm để loại bỏ các khoảng trắng không cần thiết và chuyển về chữ thường
            keyword = keyword.Trim().ToLower();

            // Thực hiện tìm kiếm theo tên phòng, mô tả phòng, sức chứa, loại phòng hoặc tên rạp
            var query = from room in dbContext.rooms
                        join cinema in dbContext.cinemas on room.CinemaId equals cinema.Id
                        where room.Name.ToLower().Contains(keyword)
                           || room.Description.ToLower().Contains(keyword)
                           || room.Capacity.ToString().Contains(keyword) // Tìm kiếm theo sức chứa
                           || (room.Type == 2 && keyword.Contains("2d"))
                           || (room.Type == 3 && keyword.Contains("3d"))
                           || cinema.NameOfCinema.ToLower().Contains(keyword) // Tìm kiếm theo tên rạp
                        select room;

            // Chuyển đổi Room thành RoomDTO và trả về kết quả
            var result = query.Select(room => roomConverter.EntityToDTO(room));
            return result;
        }


    }
}
