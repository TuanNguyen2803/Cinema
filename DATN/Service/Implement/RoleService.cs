using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.RoleRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.EntityFrameworkCore;

namespace DATN.Service.Implement
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ResponseObject<RoleDTO> responseObject;
        private readonly RoleConverter roleConverter;
        private readonly IConfiguration _configuration;

        public RoleService(AppDbContext appDbContext, ResponseObject<RoleDTO> responseObject, RoleConverter roleConverter, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            this.responseObject = responseObject;
            this.roleConverter = roleConverter;
            _configuration = configuration;
        }

        public ResponseObject<RoleDTO> CreateRole(CreateRole request)
        {
           if(string.IsNullOrWhiteSpace(request.RoleName)|| string.IsNullOrWhiteSpace(request.Code))
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Vui lòng điền đầy đủ thông tin", null);
            }
            Role role=new Role();
            role.Code = request.Code;
            role.RoleName = request.RoleName;
            _appDbContext.roles.Add(role);
            _appDbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thêm thành công", roleConverter.EntityToDTO(role));
        }
        public ResponseObject<RoleDTO> UpdateRole(UpdateRole request)
        {
            var check = _appDbContext.roles.Find(request.Id);
            if (check == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại roleid", null);
            }
            check.Code = request.Code;
            check.RoleName = request.RoleName;
            _appDbContext.roles.Update(check);
            _appDbContext.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công",roleConverter.EntityToDTO(check));
        }

        public ResponseObject<RoleDTO> DeleteRole(DeleteRole request)
        {
            var check=_appDbContext.roles.Find(request.Id);
            if(check == null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Không tồn tại roleid", null);
            }
            _appDbContext.roles.Remove(check);
            _appDbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công", null);
        }

        public IQueryable<RoleDTO> HienThi()
        {
            IQueryable<Role> check = _appDbContext.roles.AsQueryable();
            var result = check.Select(x => roleConverter.EntityToDTO(x));
            return result;
        }
    }
}
