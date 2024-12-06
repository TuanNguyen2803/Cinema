using DATN.DataContext;
using DATN.Entities;
using DATN.Handle.Image;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.BannerRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using System.Drawing;
using static DATN.Handle.Image.HandleUpdateImage;

namespace DATN.Service.Implement
{
    public class BannerService : IBannerService
    {
        private readonly AppDbContext dbContext;
        private readonly BannerConverter bannerConverter;
        private readonly ResponseObject<BannerDTO> responseObject;
        private readonly IConfiguration _configuration;

        public BannerService(AppDbContext dbContext, BannerConverter bannerConverter, ResponseObject<BannerDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.bannerConverter = bannerConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public async Task<ResponseObject<BannerDTO>> ThemBanner(ThemBannerRequest request)
        {

            if (string.IsNullOrWhiteSpace(request.Title)              
                )
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            int imageSize = 4 * 4000 * 4000;
            try
            {
                Banner banner = new Banner();
                banner.Title = request.Title;
                string ImageURL = "";
                if (request.ImageUrl!= null)
                {
                    if (!HandleCheckImage.IsImage(request.ImageUrl, imageSize))
                    {
                        return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                    }
                    else
                    {
                        var avatarFile = await HandleUploadImage.Upfile(request.ImageUrl);
                        banner.ImageUrl = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                    }
                }

                dbContext.banners.Add(banner);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Thêm banner thành công", bannerConverter.EntityToDTO(banner));
            }
            catch (Exception ex)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        public async Task<ResponseObject<BannerDTO>> SuaBanner(SuaBannerRequest request)
        {
            var checkbanner=dbContext.banners.Find(request.Id);
            if(checkbanner is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy banner", null);
            }
            int imageSize = 4 * 4000 * 4000;
            try
            {
                checkbanner.Title= request.Title;
                
                string ImageUrl = "";
                if (request.ImageUrl != null)
                {
                    if (!HandleCheckImage.IsImage(request.ImageUrl, imageSize))
                    {
                        return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                    }
                    else
                    {
                        var avatarFile = await HandleUploadImage.UpdateFile(checkbanner.ImageUrl, request.ImageUrl);
                        checkbanner.ImageUrl = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                    }
                }
               
                dbContext.banners.Update(checkbanner);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công", bannerConverter.EntityToDTO(checkbanner));
            }
            catch (Exception ex)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }

        }
        public ResponseObject<BannerDTO> XoaBanner(int bannerid)
        {
            var checkbanner = dbContext.banners.Find(bannerid);
            if (checkbanner is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy banner", null);
            }
            dbContext.banners.Remove(checkbanner);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công banner", null);
        }
        public IQueryable<BannerDTO> Hienthi()
        {
            IQueryable<Banner> check = dbContext.banners.AsQueryable();

            var result = check.Select(x => bannerConverter.EntityToDTO(x));

            return result;
        }
        public IQueryable<BannerDTO> Hienthitheoid(int bannerid)
        {
            IQueryable<Banner> check = dbContext.banners.Where(x=>x.Id==bannerid).AsQueryable();

            var result = check.Select(x => bannerConverter.EntityToDTO(x));

            return result;
        }
    }
}
