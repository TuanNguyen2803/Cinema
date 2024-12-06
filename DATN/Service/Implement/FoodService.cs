using DATN.DataContext;
using DATN.Entities;
using DATN.Handle.Image;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.CinemaRequest;
using DATN.Payload.Request.FoodRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;
using static DATN.Handle.Image.HandleUpdateImage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATN.Service.Implement
{
    public class FoodService : IFoodService
    {
        private readonly AppDbContext dbContext;
        private readonly FoodConverter foodConverter;
        private readonly ResponseObject<FoodDTO> responseObject;
        private readonly IConfiguration _configuration;

        public FoodService(AppDbContext dbContext, FoodConverter foodConverter, ResponseObject<FoodDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.foodConverter = foodConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }

        public IQueryable<FoodDTO> Hienthi()
        {
           IQueryable<Food> check=dbContext.foods.AsQueryable();
            var result = check.Select(x => foodConverter.EntityToDTO(x));
            return result;
        }

        public IQueryable<FoodDTO> Hienthitheoid(int foodid)
        {
            IQueryable<Food> check = dbContext.foods.Where(x=>x.Id==foodid).AsQueryable();
            var result = check.Select(x => foodConverter.EntityToDTO(x));
            return result;
        }

        public async Task<ResponseObject<FoodDTO>> Sua(SuaFoodRequest request)
        {
            var check = dbContext.foods.FirstOrDefault(x => x.Id == request.Id);
            if (check == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Food khong ton tai", null);
            }
            int imageSize = 2 * 1024 * 768;
            try
            {
                check.Price = request.Price;
                check.Description = request.Description;
                //check.Image = await HandleUploadImage.UpdateFile(check.Image, request.Image);
                string Image = "";
                if (request.Image != null)
                {
                    if (!HandleCheckImage.IsImage(request.Image, imageSize))
                    {
                        return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                    }
                    else
                    {
                        var avatarFile = await HandleUploadImage.UpdateFile(check.Image, request.Image);
                        check.Image = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                    }
                }
                check.NameOfFood = request.NameOfFood;
                check.IsActive = request.IsActive;
                dbContext.foods.Update(check);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sua thanh cong", foodConverter.EntityToDTO(check));
            }
            catch (Exception ex)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }
        public async Task<ResponseObject<FoodDTO>> Them(ThemFoodRequest request)
        {
           
            if (string.IsNullOrWhiteSpace(request.Description)
                || string.IsNullOrWhiteSpace(request.Price.ToString())
                || string.IsNullOrWhiteSpace(request.NameOfFood)
                )
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui long dien day du thong tin", null);
            }
            else
            {
                int imageSize = 2 * 1024 * 768;
                try
                {
                    Food food = new Food();
                    food.Price = request.Price;
                    food.Description = request.Description;
                    food.IsActive = true;
                    food.NameOfFood = request.NameOfFood;
                    string Image ="";
                    if (request.Image != null)
                    {
                        if (!HandleCheckImage.IsImage(request.Image, imageSize))
                        {
                            return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                        }
                       else
                        {
                            var avatarFile = await HandleUploadImage.Upfile(request.Image);
                            food.Image = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                        }
                    }

                    dbContext.foods.Add(food);
                    dbContext.SaveChanges();
                    return responseObject.ResponseSuccess("Them food thành công", foodConverter.EntityToDTO(food));
                }
                catch (Exception ex)
                {
                    return responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
                }

            }
        }

        public ResponseObject<FoodDTO> Xoa(int foodid)
        {
            var check=dbContext.foods.FirstOrDefault(x=>x.Id==foodid);
            if (check == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Food khong ton tai", null);
            }
            check.IsActive = false;
            dbContext.foods.Update(check);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xoa food thanh cong", null);
        }
    }
}
