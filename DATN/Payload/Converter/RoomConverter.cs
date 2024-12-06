using DATN.Entities;
using DATN.Payload.DTO;
using DATN.Payload.Response;
using Microsoft.EntityFrameworkCore;

namespace DATN.Payload.Converter
{
    public class RoomConverter
    {
        public RoomDTO EntityToDTO(Room room)
        {
            return new RoomDTO
            {
                Id = room.Id,
                Capacity = room.Capacity,
                Type = room.Type,
                Description = room.Description,
                CinemaId = room.CinemaId,
                Code = room.Code,
                Name = room.Name,
                IsActive=room.IsActive,
                cinemaName = room.Cinema?.NameOfCinema
            };
        }
    }
}
