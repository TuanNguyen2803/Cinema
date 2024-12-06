using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.UserStatusRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;

namespace DATN.Service.Implement
{
    
    public class UserStatusService : IUserStatusService
    {
        private readonly AppDbContext dbContext;
        private readonly UserStatusConverter userStatusConverter;
        private readonly ResponseObject<UserStatusDTO> responseObject;
        private readonly IConfiguration _configuration;

        public UserStatusService(AppDbContext dbContext, UserStatusConverter userStatusConverter, ResponseObject<UserStatusDTO> responseObject, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.userStatusConverter = userStatusConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }


        public ResponseObject<UserStatusDTO> CreateUserStatus(CreateUserStatus request)
        {
            if(string.IsNullOrWhiteSpace(request.Code)
               || string.IsNullOrWhiteSpace(request.Name))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            UserStatus userStatus = new UserStatus();
            userStatus.Code = request.Code;
            userStatus.Name = request.Name; 
            dbContext.userStatuses.Add(userStatus);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công",userStatusConverter.EntityToDTO(userStatus));
        }
        public ResponseObject<UserStatusDTO> UpdateUserStatus(UpdateUserStatus request)
        {
            var userStatus = dbContext.userStatuses.Find(request.Id);
            if(userStatus != null)
            {
                userStatus.Code = request.Code;
                userStatus.Name = request.Name;
                dbContext.userStatuses.Update(userStatus);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Sửa thành công",userStatusConverter.EntityToDTO(userStatus));
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy id ", null);
        }

        public ResponseObject<UserStatusDTO> DeleteUserStatus(DeleteUserStatus request)
        {
            var userStatus = dbContext.userStatuses.Find(request.Id);
            if (userStatus != null)
            {
                
                dbContext.userStatuses.Remove(userStatus);
                dbContext.SaveChanges();
                return responseObject.ResponseSuccess("Xóa thành công",null);
            }
            return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tìm thấy id ", null);
        }

        public IQueryable<UserStatusDTO> HienThi(int pageSize, int pageNumber)
        {
            IQueryable<UserStatus> check = dbContext.userStatuses.AsQueryable();
            var result = check.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => userStatusConverter.EntityToDTO(x));
            return result;
        }
    }
}
