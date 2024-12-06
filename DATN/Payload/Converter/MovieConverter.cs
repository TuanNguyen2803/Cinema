using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class MovieConverter
    {
        public MovieDTO EntityToDTO(Movie movie)
        {
            return new MovieDTO
            {
                Id = movie.Id,
                Duration = movie.MovieDuration,
                EndTime = movie.EndTime,
                PremiereDate = movie.PremiereDate,
                Description = movie.Description,
                Director = movie.Director,
                Image = movie.Image,
                HeroImage = movie.HeroImage,
                Language = movie.Language,
                MovieTypeName = movie.MovieType?.MovieTypeName,
                Name = movie.Name,
                DanhGia = movie.Rate?.Description ?? "Chưa có đánh giá", // Kiểm tra null
                Trailer = movie.Trailer,
                IsActive = movie.IsActive // Chuyển đổi trạng thái
            };
        }
    }
}
