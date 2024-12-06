using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class BannerConverter
    {
        public BannerDTO EntityToDTO(Banner banner)
        {
            return new BannerDTO
            {
                Id=banner.Id,
                ImageUrl = banner.ImageUrl,
                Title = banner.Title,

            };
        }
    }
}
