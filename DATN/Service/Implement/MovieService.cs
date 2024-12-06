using DATN.DataContext;
using DATN.Entities;
using DATN.Handle.Image;
using DATN.Handle.InputMovie;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.MovieRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;
using static DATN.Handle.Image.HandleUpdateImage;
using OfficeOpenXml; // Sử dụng EPPlus để thao tác với file Excel
using System.Globalization;
using iTextSharp.text;
namespace DATN.Service.Implement
{
    public class MovieService : IMovieService
    {
        private readonly AppDbContext dbContext;
        private readonly MovieConverter movieConverter;
        private readonly ResponseObject<MovieDTO> responseObject;
        private readonly IConfiguration _configuration;

        public MovieService(AppDbContext dbContext, MovieConverter movieConverter, ResponseObject<MovieDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.movieConverter = movieConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }


        public IQueryable<MovieDTO> HienThiPhim(InputMovie input, int pageSize, int pageNumber)
        {
            var query = dbContext.movies.Where(movie => movie.IsActive == true)
                                        .Include(movie => movie.Schedules)
                                        .ThenInclude(schedule => schedule.Tickets)
                                        .ThenInclude(ticket => ticket.Seat)
                                        .ThenInclude(seat => seat.SeatStatus)
                                        .AsNoTracking()
                                        .AsQueryable();

            if (input.CinemaId.HasValue)
            {
                query = query.Where(movie => movie.Schedules.Any(schedule => schedule.IsActive == true
                                                                       && schedule.Room.CinemaId == input.CinemaId
                                                                       && schedule.Tickets.Any(ticket => ticket.IsActive == true)));
            }

            if (input.RoomId.HasValue)
            {
                query = query.Where(movie => movie.Schedules.Any(schedule => schedule.IsActive == true
                                                                       && schedule.RoomId == input.RoomId
                                                                       && schedule.Tickets.Any(ticket => ticket.IsActive == true)));
            }

            if (input.SeatstatusID.HasValue)
            {
                query = query.Where(movie => movie.Schedules.Any(schedule => schedule.IsActive == true
                                                                       && schedule.Tickets.Any(ticket => ticket.IsActive == true
                                                                       && ticket.Seat.SeatStatusId == input.SeatstatusID)));
            }

            else
            {
                query = dbContext.movies.Where(movie => movie.IsActive == true)
                                        .Include(schedule => schedule.Schedules)
                                        .ThenInclude(ticket => ticket.Tickets)
                                        .ThenInclude(bill => bill.BillTickets.OrderByDescending(bill => bill.Quantity).AsQueryable());
            }

            var result = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => movieConverter.EntityToDTO(x));

            return result;
        }
        public async Task<ResponseObject<MovieDTO>> Sua(SuaMovieRequest request)
        {
            // Kiểm tra ID có trong yêu cầu
            var checkmovie = await dbContext.movies.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (checkmovie == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Phim không tồn tại", null);
            }

            var movieType = await dbContext.movieTypes.FirstOrDefaultAsync(x => x.Id == request.MovieTypeId);
            if (movieType == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Kiểu phim không tồn tại", null);
            }
            checkmovie.MovieTypeId = movieType.Id;

            // Cập nhật các thuộc tính chỉ khi chúng được cung cấp
            checkmovie.MovieDuration = request.MovieDuration;
            checkmovie.EndTime = request.EndTime;
            checkmovie.PremiereDate = request.PremiereDate;
            checkmovie.Description = request.Description;
            checkmovie.Director = request.Director;
            checkmovie.Language = request.Language;
            checkmovie.Name = request.Name;
            checkmovie.Trailer = request.Trailer;
            checkmovie.IsActive = request.IsActive;

            // Xử lý cập nhật hình ảnh chỉ khi được cung cấp
            if (request.Image != null)
            {
                int imageSize = 2 * 1024 * 768;
                if (!HandleCheckImage.IsImage(request.Image, imageSize))
                {
                    return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                }
                checkmovie.Image = await HandleUploadImage.UpdateFile(checkmovie.Image, request.Image);
            }

            if (request.HeroImage != null)
            {
                int hrimage = 2 * 2000 * 1100;
                if (!HandleCheckImage.IsImage(request.HeroImage, hrimage))
                {
                    return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                }
                checkmovie.HeroImage = await HandleUploadImage.UpdateFile(checkmovie.HeroImage, request.HeroImage);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                dbContext.movies.Update(checkmovie);
                await dbContext.SaveChangesAsync();
                return responseObject.ResponseSuccess("Sửa phim thành công", movieConverter.EntityToDTO(checkmovie));
            }
            catch (Exception ex)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<ResponseObject<MovieDTO>> Them(ThemMovieRequest request)
        {
            var movirTypeid = dbContext.movieTypes.FirstOrDefault(x => x.Id == request.MovieTypeId);
            if (movirTypeid == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Kieu phim khong ton tai", null);
            }
            else
            {
                int imageSize = 2 * 1024 * 768;
                int hrimage = 2 * 2000 * 1100;
                try
                {
                    Movie movie = new Movie();
                    movie.MovieDuration = request.MovieDuration;
                    movie.EndTime = request.EndTime;
                    movie.PremiereDate = request.PremiereDate;
                    movie.Description = request.Description;
                    movie.Director = request.Director;
                    string Image = "";
                    if (request.Image != null)
                    {
                        if (!HandleCheckImage.IsImage(request.Image, imageSize))
                        {
                            return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                        }
                        else
                        {
                            var avatarFile = await HandleUploadImage.Upfile(request.Image);
                            movie.Image = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                        }

                    }
                    string HeroImage = "";
                    if (request.HeroImage != null)
                    {
                        if (!HandleCheckImage.IsImage(request.HeroImage, hrimage))
                        {
                            return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                        }
                        else
                        {
                            var avatarFile = await HandleUploadImage.Upfile(request.HeroImage);
                            movie.HeroImage = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                        }

                    }
                    movie.Language = request.Language;
                    movie.MovieTypeId = request.MovieTypeId;
                    movie.Name = request.Name;
                    movie.Trailer = request.Trailer;
                    movie.RateId = request.RateId;
                    dbContext.movies.Add(movie);
                    dbContext.SaveChanges();
                    return responseObject.ResponseSuccess("Them movie thanh cong", movieConverter.EntityToDTO(movie));
                }
                catch (Exception ex)
                {
                    return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
                }
            }
        }
        public async Task<ResponseObject<List<MovieDTO>>> ImportMoviesFromExcel(IFormFile excelFile)
        {
            var moviesList = new List<MovieDTO>();

            // Kiểm tra file có hợp lệ không
            if (excelFile == null || excelFile.Length <= 0)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "File không hợp lệ", null);
            }

            // Kiểm tra định dạng file
            if (!excelFile.FileName.EndsWith(".xlsx") && !excelFile.FileName.EndsWith(".xls"))
            {
                return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, "Chỉ hỗ trợ định dạng Excel (.xlsx hoặc .xls)", null);
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets["Sheetmovie"];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++)
                        {
                            int imageSize = 2 * 1024 * 768; // Kích thước ảnh nhỏ
                            int hrimageSize = 2 * 2000 * 1100; // Kích thước ảnh lớn

                            var movie = new Movie
                            {
                                Name = worksheet.Cells[row, 1].Value?.ToString(),
                                Director = worksheet.Cells[row, 2].Value?.ToString(),
                                Language = worksheet.Cells[row, 3].Value?.ToString(),
                                MovieDuration = int.Parse(worksheet.Cells[row, 4].Value?.ToString()),
                                EndTime = DateTime.Parse(worksheet.Cells[row, 5].Value?.ToString()),
                                PremiereDate = DateTime.Parse(worksheet.Cells[row, 6].Value?.ToString()),
                                Description = worksheet.Cells[row, 7].Value?.ToString(),
                                Trailer = worksheet.Cells[row, 12].Value?.ToString()
                            };

                            // Đọc và xử lý cột 8 (Ảnh Poster)
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 8].Text) && worksheet.Cells[row, 8].Text != "#VALUE!")
                            {
                                var posterPath = worksheet.Cells[row, 8].Text; // Dùng .Text thay vì .Value
                                var posterBytes = await LoadImageFromUrlAsync(posterPath); // Tải hình ảnh từ URL
                                var posterFile = new FormFile(new MemoryStream(posterBytes), 0, posterBytes.Length, null, Path.GetFileName(posterPath)); // Khai báo posterFile

                                if (!HandleCheckImage.IsImage(posterFile, imageSize))
                                {
                                    return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, $"Ảnh Poster không hợp lệ tại dòng {row}", null);
                                }

                                var uploadedPosterFile = await HandleUploadImage.Upfile(posterFile);
                                movie.Image = string.IsNullOrEmpty(uploadedPosterFile) ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : uploadedPosterFile;
                            }

                            // Đọc và xử lý cột 9 (Ảnh Hero)
                            if (!string.IsNullOrEmpty(worksheet.Cells[row, 9].Value?.ToString()))
                            {
                                var heroImagePath = worksheet.Cells[row, 9].Value.ToString();
                                var posterBytes = await LoadImageFromUrlAsync(heroImagePath); // Tải hình ảnh từ URL
                                var heroImageFile = new FormFile(new MemoryStream(posterBytes), 0, posterBytes.Length, null, Path.GetFileName(heroImagePath));

                                if (!HandleCheckImage.IsImage(heroImageFile, hrimageSize))
                                {
                                    return responseObject.ResponseErrorList(StatusCodes.Status400BadRequest, $"Ảnh Hero không hợp lệ tại dòng {row}", null);
                                }

                                var uploadedHeroImageFile = await HandleUploadImage.Upfile(heroImageFile);
                                movie.HeroImage = uploadedHeroImageFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : uploadedHeroImageFile; // Đặt URL mặc định
                            }

                            // Lấy movieTypeName từ cột 10 và tìm MovieTypeId tương ứng
                            var movieTypeName = worksheet.Cells[row, 10].Value?.ToString();
                            var movieType = await dbContext.movieTypes
                                                            .Where(m => m.MovieTypeName.ToLower() == movieTypeName.ToLower())
                                                            .FirstOrDefaultAsync();
                            if (movieType == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Không tìm thấy loại phim '{movieTypeName}' tại dòng {row}", null);
                            }
                            movie.MovieTypeId = movieType.Id;

                            // Lấy description từ cột 11 và tìm RateId tương ứng
                            var rateDescription = worksheet.Cells[row, 11].Value?.ToString();
                            var rate = await dbContext.rates
                                                        .Where(r => r.Description.ToLower() == rateDescription.ToLower())
                                                        .FirstOrDefaultAsync();

                            if (rate == null)
                            {
                                return responseObject.ResponseErrorList(StatusCodes.Status404NotFound, $"Không tìm thấy đánh giá với mô tả '{rateDescription}' tại dòng {row}", null);
                            }
                            movie.RateId = rate.Id;

                            // Thêm phim vào cơ sở dữ liệu
                            dbContext.movies.Add(movie);
                        }

                        // Lưu các thay đổi vào cơ sở dữ liệu
                        await dbContext.SaveChangesAsync();

                        // Chuyển đổi danh sách phim sang DTO
                        moviesList = dbContext.movies.Select(movieConverter.EntityToDTO).ToList();
                    }
                }
                return responseObject.ResponseSuccessList("Import thành công", moviesList);
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, innerExceptionMessage, null);
            }
            catch (Exception ex)
            {
                return responseObject.ResponseErrorList(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        private async Task<byte[]> LoadImageFromUrlAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                throw new ArgumentException("URL không hợp lệ: " + url);
            }

            using (var httpClient = new HttpClient())
            {
                // Thêm User-Agent để tránh bị chặn
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MyApp/1.0)");
                return await httpClient.GetByteArrayAsync(url);
            }
        }




        public ResponseObject<MovieDTO> Xoa(int movieid)
        {
            var movie = dbContext.movies.FirstOrDefault(x => x.Id == movieid);
            if (movie == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Movie khong ton tai", null);
            }
            movie.IsActive = false;
            dbContext.movies.Update(movie);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xoa thanh cong ", null);
        }
        public IQueryable<MovieDTO> HienThiTatCaPhim()
        {
            var query = dbContext.movies
                         .Where(movie => movie.IsActive == true)
                         .Include(movie => movie.MovieType) // Include MovieType to access MovieTypeName
                         .Include(movie => movie.Rate)
                         .Include(movie => movie.Schedules)
                             .ThenInclude(schedule => schedule.Tickets)
                             .ThenInclude(ticket => ticket.Seat)
                             .ThenInclude(seat => seat.SeatStatus)
                         .AsNoTracking()
                         .AsQueryable();

            return query.Select(x => movieConverter.EntityToDTO(x));

        }


        public IQueryable<MovieDTO> Hienthithongtinphim(int movieid)
        {
            var query = dbContext.movies
                .Where(x => x.Id == movieid)
                  .Include(movie => movie.MovieType) // Include MovieType to access MovieTypeName
                         .Include(movie => movie.Rate)

                .Select(x => movieConverter.EntityToDTO(x)); // Convert to DTO

            return query;
        }

        public IQueryable<MovieDTO> TimKiemPhim(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<MovieDTO>().AsQueryable(); // Trả về danh sách trống nếu từ khóa rỗng
            }

            // Chuẩn hóa từ khóa tìm kiếm: loại bỏ khoảng trắng không cần thiết và chuyển về chữ thường
            keyword = keyword.Trim().ToLower();

            // Tìm kiếm các phim có tên, đạo diễn, ngôn ngữ, loại phim, đánh giá hoặc mô tả chứa từ khóa
            var query = dbContext.movies
                        .Where(movie => movie.IsActive == true)
                        .Include(movie => movie.MovieType) // Include để có thể truy xuất tên loại phim
                        .Include(movie => movie.Rate) // Include để có thể truy xuất mô tả đánh giá
                        .Where(movie =>
                               movie.Name.ToLower().Contains(keyword) || // Tìm theo tên phim
                               movie.Director.ToLower().Contains(keyword) || // Tìm theo đạo diễn
                               movie.Language.ToLower().Contains(keyword) || // Tìm theo ngôn ngữ
                               movie.MovieType.MovieTypeName.ToLower().Contains(keyword) || // Tìm theo loại phim
                               movie.Rate.Description.ToLower().Contains(keyword) || // Tìm theo mô tả đánh giá
                               movie.Description.ToLower().Contains(keyword)) // Tìm theo mô tả phim
                        .AsNoTracking(); // Không tracking các thay đổi để cải thiện hiệu suất

            // Chuyển đổi kết quả thành DTO
            var result = query.Select(x => movieConverter.EntityToDTO(x));

            return result;
        }

        public IQueryable<MovieDTO> LocPhimTheoTheLoai(int movieTypeId)
        {
            // Truy vấn phim theo MovieTypeId và trạng thái IsActive
            var query = dbContext.movies
                        .Where(movie => movie.IsActive == true && movie.MovieTypeId == movieTypeId)
                        .Include(movie => movie.MovieType) // Để có thể truy cập thông tin MovieType
                        .Include(movie => movie.Rate)
                        .Include(movie => movie.Schedules)
                            .ThenInclude(schedule => schedule.Tickets)
                            .ThenInclude(ticket => ticket.Seat)
                            .ThenInclude(seat => seat.SeatStatus)
                        .AsNoTracking()
                        .AsQueryable();

            // Chuyển đổi kết quả thành DTO
            var result = query.Select(x => movieConverter.EntityToDTO(x));

            return result;
        }

    }
}
