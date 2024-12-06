using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.PromotionRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATN.Service.Implement
{
    public class PromotionService : IPromotionService
    {
        private readonly AppDbContext _context;
        private readonly PromotionConverter _promotionConverter;
        private readonly ResponseObject<PromotionDTO> _responseObject;

        public PromotionService(AppDbContext context, PromotionConverter promotionConverter, ResponseObject<PromotionDTO> responseObject)
        {
            _context = context;
            _promotionConverter = promotionConverter;
            _responseObject = responseObject;
        }

        public ResponseObject<PromotionDTO> Them(CreatePromotion request)
        {
            var rankCustomer = _context.rankCustomers.FirstOrDefault(x => x.Id == request.RankCustomerId);
            if (rankCustomer is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại rank customer", null);
            }

            var promotion = new Promotion
            {
                Percent = request.Percent,
                Quantity = request.Quantity,
                Type = request.Type,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Description = request.Description,
                Name = request.Name,
                IsActive = true,
                RankCustomerId = rankCustomer.Id
            };

            _context.promotions.Add(promotion);
            _context.SaveChanges();

            return _responseObject.ResponseSuccess("Thêm thành công", _promotionConverter.EntityToDTO(promotion));
        }

        public ResponseObject<PromotionDTO> Delete(int promotionId)
        {
            var promotion = _context.promotions.FirstOrDefault(x => x.Id == promotionId);
            if (promotion is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại promotionId {promotionId}", null);
            }

            promotion.IsActive = false; // Đánh dấu là không còn hoạt động
            _context.promotions.Update(promotion);
            _context.SaveChanges();

            return _responseObject.ResponseSuccess("Xóa thành công", null);
        }

        public IQueryable<PromotionDTO> GetAll()
        {
            IQueryable<Promotion> check=_context.promotions
                     .Include(pr => pr.RankCustomer)
                     .AsNoTracking() 
                     .AsQueryable();
            var result = check.Select(pro => _promotionConverter.EntityToDTO(pro));
            return result;
        }

        public ResponseObject<PromotionDTO> Sua(UpdatePromtion request)
        {
         
            var promotion = _context.promotions.Find(request.Id);
            if (promotion is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại promotionId {request.Id}", null);
            }
            else
            {
                var rankCustomer = _context.rankCustomers.FirstOrDefault(x => x.Id == request.RankCustomerId);
                if (rankCustomer is null)
                {
                    return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại rank customer", null);
                }
                // Cập nhật thông tin khuyến mãi
                promotion.Percent = request.Percent;
                promotion.Quantity = request.Quantity;
                promotion.Type = request.Type;
                promotion.StartTime = request.StartTime;
                promotion.EndTime = request.EndTime;
                promotion.Description = request.Description;
                promotion.Name = request.Name;
                promotion.RankCustomerId = request.RankCustomerId;
                promotion.IsActive = request.IsActive;

                _context.promotions.Update(promotion);
                _context.SaveChanges();

                return _responseObject.ResponseSuccess("Sửa thành công", null);
            }
           
        }

        public IQueryable<PromotionDTO> HienThiTheoId(int promotionId)
        {
            return _context.promotions
                           .Where(x => x.Id == promotionId)
                           .Select(x => _promotionConverter.EntityToDTO(x));
        }

        public IQueryable<PromotionDTO> hienthitheoid(int promotionid)
        {
            IQueryable<Promotion> check = _context.promotions.Where(x => x.Id == promotionid).AsQueryable();
            return check.Select(x => _promotionConverter.EntityToDTO(x));
        }
    }
}
