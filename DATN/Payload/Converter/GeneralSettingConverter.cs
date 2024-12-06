using DATN.Entities;
using DATN.Payload.DTO;

namespace DATN.Payload.Converter
{
    public class GeneralSettingConverter
    {
        public GeneralSettingDTO EntityToDTO(GeneralSetting generalSetting)
        {
            return new GeneralSettingDTO
            {
                Id = generalSetting.Id,
                BreakTime = generalSetting.BreakTime,
                BusinessHours = generalSetting.BusinessHours,
                CloseTime = generalSetting.CloseTime,
                FixedTiketPrice = generalSetting.FixedTiketPrice,
                PercentDay = generalSetting.PercentDay,
                PercenWeekend=generalSetting.PercenWeekend,
                TimeBeginToChage = generalSetting.TimeBeginToChage,
            };
        }
    }
}
