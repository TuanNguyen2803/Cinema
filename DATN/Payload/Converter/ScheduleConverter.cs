using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class ScheduleConverter
    {
        public ScheduleDTO EntityToDTO(Schedule  schedule)
        {
            return new ScheduleDTO
            {
                Id = schedule.Id,
                Price = schedule.Price,
                StartAt = schedule.StartAt,
                EndAt = schedule.EndAt,
                Code = schedule.Code,
                MovieId = schedule.MovieId,
                Name = schedule.Name,
                RoomId = schedule.RoomId,
                IsActive = schedule.IsActive,
            };
        }
    }
}
