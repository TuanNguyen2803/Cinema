using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class CinemaConverter
    {
        public CinemaDTO EntityToDTO(Cinema cinema)
        {
            return new CinemaDTO
            {
                Id = cinema.Id,
                Address = cinema.Address,
                Description = cinema.Description,
                Code = cinema.Code,
                NameOfCinema = cinema.NameOfCinema,
                IsAtive = cinema.IsActive
            };

        }
    }
}
