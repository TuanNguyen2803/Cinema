using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class MovieTypeConverter
    {
        public MovieTypeDTO EntityToDTO (MovieType movieType)
        {
            return new MovieTypeDTO
            {
                Id = movieType.Id,
                MovieTypeName = movieType.MovieTypeName,
                IsActive = movieType.IsActive,
            };
        }
    }
}
