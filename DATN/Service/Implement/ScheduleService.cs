using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.ScheduleRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATN.Service.Implement
{
    public class ScheduleService : IScheduleService
    {
        private readonly AppDbContext dbContext;
        private readonly ScheduleConverter scheduleConverter;
        private readonly ResponseObject<ScheduleDTO> responseObject;
        private readonly IConfiguration _configuration;

        public ScheduleService(AppDbContext dbContext, ScheduleConverter scheduleConverter, ResponseObject<ScheduleDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.scheduleConverter = scheduleConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }
        public ResponseObject<ScheduleDTO> Them(ThemScheduleRequest request)
        {
            var checkmovie = dbContext.movies.Find(request.MovieId);
            var checkroom = dbContext.rooms.Find(request.RoomId);

            if (checkmovie is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Movie không tồn tại", null);
            }
            if (checkroom is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Room không tồn tại", null);
            }
            // Tự động cập nhật EndAt bằng StartAt + Duration của bộ phim
            DateTime calculatedEndAt = request.StartAt.AddMinutes(checkmovie.MovieDuration);
            // Kiểm tra lịch chiếu trùng lặp
            var checkschedule = dbContext.schedules.FirstOrDefault(x =>
        x.RoomId == request.RoomId &&
        (x.StartAt < calculatedEndAt && x.EndAt > request.StartAt));

            if (checkschedule != null)
            {
                return responseObject.ResponseError(StatusCodes.Status409Conflict, "Phòng đã có lịch chiếu trong khoảng thời gian này", null);
            }

            if (DateTime.Compare(request.StartAt, calculatedEndAt) >= 0)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc", null);
            }

            // Tạo lịch chiếu mới
            Schedule sch = new Schedule
            {
                Price = request.Price,
                StartAt = request.StartAt,
                EndAt = calculatedEndAt,
                Code = "MyBugs__" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999),
                MovieId = request.MovieId,
                Name = request.Name,
                RoomId = request.RoomId
            };

            dbContext.schedules.Add(sch);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", scheduleConverter.EntityToDTO(sch));
        }
        public ResponseObject<List<ScheduleDTO>> ThemLichChieuTuFileExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "File không hợp lệ", null);
            }

            var schedules = new List<Schedule>();

            try
            {
                // Đọc file Excel
                using (var stream = file.OpenReadStream())
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Sử dụng bản miễn phí cho mục đích không thương mại
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets["SheetLichchieu"];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Bắt đầu từ hàng 2, bỏ qua tiêu đề
                        {
                            var movieName = worksheet.Cells[row, 1].Value?.ToString().Trim();  // Lấy tên phim từ cột 1
                            var roomName = worksheet.Cells[row, 2].Value?.ToString().Trim();   // Lấy tên phòng từ cột 2
                            var startAt = DateTime.Parse(worksheet.Cells[row, 3].Value?.ToString().Trim()); // Lấy thời gian bắt đầu từ cột 3
                            var endAt = DateTime.Parse(worksheet.Cells[row, 4].Value?.ToString().Trim());   // Lấy thời gian kết thúc từ cột 4
                            var price = Convert.ToDouble(worksheet.Cells[row, 5].Value?.ToString().Trim()); // Lấy giá từ cột 5
                            var name = worksheet.Cells[row, 6].Value?.ToString().Trim();  // Lấy tên lịch chiếu từ cột 6

                            // Tìm kiếm movieId dựa trên tên phim
                            var movie = dbContext.movies.Where(m => m.Name.ToLower() == movieName.ToLower())
                                                        .FirstOrDefault();
                            if (movie == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Phim với tên '{movieName}' không tồn tại", null);
                            }

                            // Tìm kiếm roomId dựa trên tên phòng
                            var room = dbContext.rooms.Where(r => r.Name.ToLower() == roomName.ToLower())
                                                        .FirstOrDefault();
                            if (room == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Phòng với tên '{roomName}' không tồn tại", null);
                            }

                            // Kiểm tra lịch chiếu trùng lặp
                            var checkschedule = dbContext.schedules.FirstOrDefault(x =>
                                x.RoomId == room.Id &&
                                (x.StartAt < endAt && x.EndAt > startAt));

                            if (checkschedule != null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status409Conflict, $"Phòng đã có lịch chiếu trong khoảng thời gian {startAt} - {endAt}", null);
                            }

                            if (DateTime.Compare(startAt, endAt) >= 0)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc", null);
                            }

                            // Tạo lịch chiếu mới
                            Schedule sch = new Schedule
                            {
                                Price = price,
                                StartAt = startAt,
                                EndAt = endAt,
                                Code = "MyBugs__" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999),
                                MovieId = movie.Id,   // Sử dụng MovieId lấy được từ tên phim
                                Name = name,
                                RoomId = room.Id       // Sử dụng RoomId lấy được từ tên phòng
                            };

                            schedules.Add(sch);
                        }
                    }
                }

                dbContext.schedules.AddRange(schedules);
                dbContext.SaveChanges();

                // Chuyển đổi thành DTOs và trả về phản hồi thành công
                var scheduleDTOs = schedules.Select(s => scheduleConverter.EntityToDTO(s)).ToList();
                return responseObject.ResponseSuccessList("Thêm lịch chiếu từ file Excel thành công", scheduleDTOs);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về phản hồi lỗi
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, $"Có lỗi xảy ra khi xử lý file: {ex.Message}", null);
            }
        }



        public ResponseObject<ScheduleDTO> Sua(SuaScheduleRequest request)
        {
            var check = dbContext.schedules.Find(request.Id);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại lịch chiếu", null);
            }

            var checkmovie = dbContext.movies.Find(request.MovieId);
            var checkroom = dbContext.rooms.Find(request.RoomId);

            if (checkmovie is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Movie không tồn tại", null);
            }
            if (checkroom is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Room không tồn tại", null);
            }
            DateTime calculatedEndAt = request.StartAt.AddMinutes(checkmovie.MovieDuration);
            // Kiểm tra lịch chiếu trùng lặp
            var checkschedule = dbContext.schedules.FirstOrDefault(x =>
        x.RoomId == request.RoomId &&
        (x.StartAt < calculatedEndAt && x.EndAt > request.StartAt));

            if (checkschedule != null)
            {
                return responseObject.ResponseError(StatusCodes.Status409Conflict, "Phòng đã có lịch chiếu trong khoảng thời gian này", null);
            }

            if (DateTime.Compare(request.StartAt, calculatedEndAt) >= 0)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Thời gian bắt đầu phải sớm hơn thời gian kết thúc", null);
            }

            // Cập nhật thông tin lịch chiếu
            check.Price = request.Price;
            check.StartAt = request.StartAt;
            check.EndAt = calculatedEndAt;
            check.Code = "MyBugs__" + DateTime.Now.Ticks.ToString() + "_xyz_" + new Random().Next(100, 999);
            check.MovieId = request.MovieId;
            check.Name = request.Name;
            check.RoomId = request.RoomId;
            check.IsActive = request.IsActive;

            dbContext.schedules.Update(check);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công", scheduleConverter.EntityToDTO(check));
        }

        public ResponseObject<ScheduleDTO> Xoa(int scheduleid)
        {
            var check = dbContext.schedules.Find(scheduleid);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Khong ton tai lich chieu", null);
            }
            check.IsActive = false;
            dbContext.schedules.Update(check);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xoa thanh cong", null);
        }

        public IQueryable<ScheduleDTO> Getall(int movieid, int? cinemaid)
        {
            IQueryable<Schedule> query = dbContext.schedules.AsQueryable();

            // Lọc theo movieid
            query = query.Where(schedule => schedule.MovieId == movieid);

            // Nếu cinemaid có giá trị, lọc thêm theo cinemaid
            if (cinemaid.HasValue)
            {
                // Giả sử có một thuộc tính RoomId trong Schedule để liên kết với Room
                query = query.Where(schedule => schedule.Room.CinemaId == cinemaid.Value);
            }

            // Chuyển đổi các phòng thành DTO và trả về kết quả
            var result = query.Select(schedule => scheduleConverter.EntityToDTO(schedule));
            return result;
        }
        public IQueryable<ScheduleDTO> GetbyRap(int movieid, int? cinemaid)
        {
            IQueryable<Schedule> query = dbContext.schedules.AsQueryable();

            // Lọc theo movieid
            query = query.Where(schedule => schedule.MovieId == movieid);

            // Nếu cinemaid có giá trị, lọc thêm theo cinemaid
            if (cinemaid.HasValue)
            {
                // Giả sử có một thuộc tính RoomId trong Schedule để liên kết với Room
                query = query.Where(schedule => schedule.Room.CinemaId == cinemaid.Value);
            }

            // Chuyển đổi các phòng thành DTO và trả về kết quả
            var result = query.Select(schedule => scheduleConverter.EntityToDTO(schedule));
            return result;
        }

        public IQueryable<ScheduleDTO> hienthitheoid(int scheduleid)
        {
            IQueryable<Schedule> query = dbContext.schedules.Where(x => x.Id == scheduleid).AsQueryable();

            var result = query.Select(x => scheduleConverter.EntityToDTO(x));
            return result;
        }


        public IQueryable<ScheduleDTO> GetSchedules(int? cinemaId, int? roomId, int? movieId)
        {
            IQueryable<Schedule> query = dbContext.schedules.AsQueryable();

            // Nếu cinemaId có giá trị
            if (cinemaId.HasValue)
            {
                // Nếu roomId có giá trị
                if (roomId.HasValue)
                {
                    // Nếu movieId có giá trị
                    if (movieId.HasValue)
                    {
                        // Hiển thị lịch chiếu theo cinemaId, roomId, movieId
                        query = query.Where(schedule => schedule.RoomId == roomId.Value && schedule.MovieId == movieId.Value);
                    }
                    else
                    {
                        // Hiển thị lịch chiếu theo cinemaId, roomId
                        query = query.Where(schedule => schedule.RoomId == roomId.Value);
                    }
                }
                else
                {
                    // Nếu không có roomId
                    if (movieId.HasValue)
                    {
                        // Hiển thị lịch chiếu theo cinemaId và movieId
                        query = query.Where(schedule => schedule.MovieId == movieId.Value);
                    }
                    else
                    {
                        // Hiển thị tất cả lịch chiếu theo cinemaId
                        query = query.Where(schedule => schedule.Room.CinemaId == cinemaId.Value);
                    }
                }
            }
            else
            {
                // Nếu không có cinemaId
                if (roomId.HasValue)
                {
                    // Nếu movieId có giá trị
                    if (movieId.HasValue)
                    {
                        // Hiển thị lịch chiếu theo roomId và movieId
                        query = query.Where(schedule => schedule.RoomId == roomId.Value && schedule.MovieId == movieId.Value);
                    }
                    else
                    {
                        // Hiển thị lịch chiếu theo roomId
                        query = query.Where(schedule => schedule.RoomId == roomId.Value);
                    }
                }
                else
                {
                    // Nếu không có roomId
                    if (movieId.HasValue)
                    {
                        // Hiển thị lịch chiếu theo movieId
                        query = query.Where(schedule => schedule.MovieId == movieId.Value);
                    }
                    // Nếu không có cinemaId, roomId, movieId
                    // Trả về tất cả lịch chiếu
                }
            }

            // Chuyển đổi lịch chiếu thành DTO và trả về kết quả
            return query.Select(x => scheduleConverter.EntityToDTO(x));
        }

        public IQueryable<ScheduleDTO> TimKiemLichChieu(string keyword)
        {
            // Khởi tạo truy vấn cơ bản
            IQueryable<Schedule> query = dbContext.schedules
                .Include(s => s.Movie) // Bao gồm thông tin về phim
                .Include(s => s.Room)  // Bao gồm thông tin về phòng
                .AsQueryable();

            // Tìm kiếm theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string lowerKeyword = keyword.ToLower(); // Chuyển về chữ thường để tìm kiếm không phân biệt chữ hoa/thường
                query = query.Where(schedule =>
                    schedule.Movie.Name.ToLower().Contains(lowerKeyword) || // Tìm kiếm theo tên phim
                    schedule.Name.ToLower().Contains(lowerKeyword) || // Tìm kiếm theo tên lịch chiếu
                    schedule.Room.Name.ToLower().Contains(lowerKeyword)); // Tìm kiếm theo tên phòng
            }

            // Chuyển đổi kết quả thành DTO và trả về
            var result = query.Select(schedule => scheduleConverter.EntityToDTO(schedule));
            return result;
        }
    }
}
