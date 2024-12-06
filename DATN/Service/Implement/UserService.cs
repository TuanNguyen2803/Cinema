using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net;
using DATN.Service.Interface;
using DATN.DataContext;
using DATN.Payload.Converter;
using DATN.Payload.Response;
using DATN.Handle.Email;
using DATN.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DATN.Payload.Request.UserRequest;
using BcryptNet = BCrypt.Net.BCrypt;
using System;
using System.Reflection.Metadata.Ecma335;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Microsoft.AspNetCore.Http;
using DATN.Payload.DTO;
using DATN.Payload.Request.TokenRequest;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;  
using System.Text;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;

namespace DATN.Service.Implement
{
    public class UserSevice : IUserService
    {
        private readonly AppDbContext dbContext;
        private readonly UserConverter userConverter;
        private readonly ResponseObject<UserDTO> responseObject;
        private readonly ResponseObject<TokenDTO> responsetokenObject;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly GetUserConverter _userConverter;
        private int GenerateCodeActive()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }


        public UserSevice(AppDbContext DbContext, UserConverter UserConverter, GetUserConverter getUserConverter, ResponseObject<UserDTO> ResponseObject, ResponseObject<TokenDTO> ResponsetokenObject, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            dbContext = DbContext;
            userConverter = UserConverter;
            responseObject = ResponseObject;
            responsetokenObject = ResponsetokenObject;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userConverter = getUserConverter;
        }

        public string SendEmail(EmailTo emailTo)
        {
            // Kiểm tra tính hợp lệ của email
            if (!Validate.IsValidEmail(emailTo.Mail))
            {
                return "Định dạng email không hợp lệ";
            }

            // Cấu hình SMTP Client
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("tuannguyen28032002@gmail.com", "arnh sapf klhg offf"), // Thay đổi thông tin ở đây
                EnableSsl = true
            };

            try
            {
                // Tạo đối tượng MailMessage
                var message = new MailMessage
                {
                    From = new MailAddress("tuannguyen28032002@gmail.com"), // Địa chỉ email của bạn
                    Subject = emailTo.Subject,
                    Body = emailTo.Content,
                    IsBodyHtml = true
                };
                message.To.Add(emailTo.Mail);

                // Gửi email
                smtpClient.Send(message);
                return "Gửi email thành công";
            }
            catch (SmtpException smtpEx)
            {
                // Xử lý các lỗi SMTP cụ thể
                return "Lỗi SMTP: " + smtpEx.Message;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi chung
                return "Lỗi khi gửi email: " + ex.Message;
            }

        }
        public ResponseObject<UserDTO> Dangkytaikhoan(DangkyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName)
             || string.IsNullOrWhiteSpace(request.Password)
             || string.IsNullOrWhiteSpace(request.Email)
             || string.IsNullOrWhiteSpace(request.PhoneNumber)
             || string.IsNullOrWhiteSpace(request.Name)
                )
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            if (Validate.IsValidEmail(request.Email) == false)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng Email không hợp lệ", null);
            }

            if (dbContext.users.FirstOrDefault(x => x.UserName.Equals(request.UserName)) != null)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tên người dùng đã tồn tại ", null);
            }
            else
            {
                User user = new User();
                user.UserName = request.UserName;
                user.Email = request.Email;
                user.Name = request.Name;
                user.PhoneNumber = request.PhoneNumber;
                user.Password = BcryptNet.HashPassword(request.Password);
                user.UserStatusId = 1;
                user.RoleId = 4;
                user.RankCustomerId = 5;
                dbContext.users.Add(user);
                dbContext.SaveChanges();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    ConfirmCode = GenerateCodeActive().ToString(),
                    ExpiredDateTime = DateTime.Now.AddMinutes(30),
                    IsConfirm = false
                };
                dbContext.confirmEmails.Add(confirmEmail);
                dbContext.SaveChanges();
                string message = SendEmail(new EmailTo
                {
                    Mail = request.Email,
                    Subject = "Nhận mã xác nhận để xác nhận đăng ký tài khoản của bạn tại đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.ConfirmCode}, mã này có hiệu lực trong 30 phút"
                });
                return responseObject.ResponseSuccess("Đăng ký thành công!Vui lòng xác thực để sử dụng tài khoản! ", userConverter.EntitytoDTO(user));
            }

        }
        public ResponseObject<UserDTO> Xacthuctaikhoan(XacthucRequest request)
        {
            var confirmEmail = dbContext.confirmEmails.FirstOrDefault(x => x.ConfirmCode.Equals(request.ConfirmCode));
            if (confirmEmail is null)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác thực không đúng ", null);
            }
            if (confirmEmail.ExpiredDateTime < DateTime.Now)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }

            User user = dbContext.users.FirstOrDefault(x => x.Id == confirmEmail.UserId);
            user.UserStatusId = 2;
            user.IsActive = true;
            user.RankCustomerId = (dbContext.rankCustomers.FirstOrDefault(x => x.Point == 0)).Id;
            dbContext.users.Update(user);
            dbContext.SaveChanges();
            confirmEmail.IsConfirm = true;
            dbContext.confirmEmails.Update(confirmEmail);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xác thực tài khoản thành công ", userConverter.EntitytoDTO(user));
        }
        public ResponseObject<TokenDTO> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return responsetokenObject.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }
            var user = dbContext.users.FirstOrDefault(x => x.UserName.Equals(request.UserName));
            if (user is null)
            {
                return responsetokenObject.ResponseError(StatusCodes.Status404NotFound, "Tên tài khoản không tồn tại trên hệ thống", null);
            }
            if (user.UserStatusId == 1 || user.IsActive == false)
            {
                return responsetokenObject.ResponseError(StatusCodes.Status400BadRequest, "Tài khoản chưa được kích hoạt hoặc đã bị xóa, vui lòng kích hoạt tài khoản", null);
            }
            bool checkPass = BcryptNet.Verify(request.Password, user.Password);
            if (!checkPass)
            {
                return responsetokenObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không chính xác", null);
            }
            else
            {
                return responsetokenObject.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(user));
            }
        }
        public ResponseObject<UserDTO> Quenmatkhau(QuenmatkhauRequest request)
        {
            var user = dbContext.users.FirstOrDefault(x => x.Email == request.Email);
            if (user is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Email không tồn tại trong hệ thống", null);
            }
            else
            {
                var confirms = dbContext.confirmEmails.Where(x => x.UserId == user.Id).ToList();
                dbContext.confirmEmails.RemoveRange(confirms);
                dbContext.SaveChangesAsync();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    IsConfirm = false,
                    ExpiredDateTime = DateTime.Now.AddMinutes(30),
                    ConfirmCode = GenerateCodeActive().ToString()
                };
                dbContext.confirmEmails.AddAsync(confirmEmail);
                dbContext.SaveChangesAsync();
                string message = SendEmail(new EmailTo
                {
                    Mail = request.Email,
                    Subject = "Nhận mã xác nhận để tạo mật khẩu mới từ đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.ConfirmCode}, mã này sẽ hết hạn sau 4 tiếng"
                });
                return responseObject.ResponseSuccess("Gửi mã xác nhận về email thành công, vui lòng kiểm tra email", userConverter.EntitytoDTO(user));
            }
        }
        public ResponseObject<UserDTO> Taomatkhau(TaomatkhaumoiRequest request)
        {
            var confirmCode = dbContext.confirmEmails.FirstOrDefault(x => x.ConfirmCode.Contains(request.ConfirmCode));
            if (confirmCode is null)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận không chính xác", null);
            }
            if (confirmCode.ExpiredDateTime < DateTime.Now)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "mật khẩu không trùng khớp", null);
            }
            User user = dbContext.users.FirstOrDefault(x => x.Id == confirmCode.UserId);
            var Password = BcryptNet.HashPassword(request.NewPassword);
            user.Password = Password;
            //dbContext.confirmEmails.Remove(confirmEmail);
            dbContext.users.Update(user);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Tạo mật khẩu mới thành công", userConverter.EntitytoDTO(user));
        }
        public ResponseObject<UserDTO> Doimatkhau(DoimatkhauRequest request, int userId)
        {
            var user = dbContext.users.FirstOrDefault(x => x.Id == userId);
            if (!BcryptNet.Verify(request.OldPassword, user.Password))
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu cũ không chính xác", null);
            }
            if (request.NewPassword != request.ConfirmNewPassword)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mật khẩu không trùng nhau! Vui lòng thử lại", null);
            }
            user.Password = BcryptNet.HashPassword(request.NewPassword);
            dbContext.users.Update(user);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thay đổi mật khẩu thành công", userConverter.EntitytoDTO(user));
        }
        public ResponseObject<TokenDTO> RenewAccessToken(RequestToken request)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value;

                var tokenValidation = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSetting:SecretKey").Value!))
                };

                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);

                if (!(validatedToken is JwtSecurityToken jwtSecurityToken) || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return responsetokenObject.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }

                var refreshToken = dbContext.refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);

                if (refreshToken == null)
                {
                    return responsetokenObject.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }

                if (refreshToken.ExpiredTime < DateTime.Now)
                {
                    return responsetokenObject.ResponseError(StatusCodes.Status401Unauthorized, "Token đã hết hạn", null);
                }

                var user = dbContext.users.FirstOrDefault(x => x.Id == refreshToken.UserId);

                if (user == null)
                {
                    return responsetokenObject.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }

                var newToken = GenerateAccessToken(user);

                return responsetokenObject.ResponseSuccess("Làm mới token thành công", newToken);
            }
            catch (SecurityTokenValidationException ex)
            {
                return responsetokenObject.ResponseError(StatusCodes.Status400BadRequest, "Lỗi xác thực token: " + ex.Message, null);
            }
            catch (Exception ex)
            {
                return responsetokenObject.ResponseError(StatusCodes.Status500InternalServerError, "Lỗi không xác định: " + ex.Message, null);
            }
        }
        public TokenDTO GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSetting:SecretKey").Value!);

            var decentralization = dbContext.roles.FirstOrDefault(x => x.Id == user.RoleId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserName", user.UserName),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim(ClaimTypes.Role, decentralization?.Code ?? "")
                }),
                Expires = DateTime.Now.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ExpiredTime = DateTime.Now.AddHours(4),
                UserId = user.Id
            };

            dbContext.refreshTokens.Add(rf);
            dbContext.SaveChanges();

            TokenDTO tokenDTO = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                DataResponseUser = userConverter.EntitytoDTO(user),
            };
            return tokenDTO;
        }
        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public ResponseObject<UserDTO> Thaydoiquyenhan(PhanQuyenRequest request)
        {
            var user = dbContext.users.FirstOrDefault(x => x.Id == request.UserId && x.IsActive == true);
            var role = dbContext.roles.FirstOrDefault(x => x.Id == request.RoleId);
            if (role is null || user is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "User hoặc role không tồn tại", null);
            }

            user.RoleId = request.RoleId;
            dbContext.users.Update(user);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Thay đổi quyền thành công", null);


        }
        public ResponseObject<UserDTO> DeleteUser(int userid)
        {
            var check = dbContext.users.FirstOrDefault(x => x.Id == userid);
            if (check is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại userid {userid}", null);
            }
            check.IsActive = false;
            dbContext.users.Update(check);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Xóa thành công", null);
        }
        public IQueryable<UserDTO> GetAllUsers() // Đổi tên hàm
        {
            var allUsers = dbContext.users
                .Include(u => u.Role) // Bao gồm thông tin Role
                .Include(u => u.UserStatus) // Bao gồm thông tin UserStatus
                .AsNoTracking() // Không theo dõi thay đổi để cải thiện hiệu suất
                .Select(u => userConverter.EntitytoDTO(u)); // Chuyển đổi từ Entity sang DTO một lần

            return allUsers;
        }


        public ResponseObject<UserDTO> UpdateUser(UpdateUserRequest request)
        {
            var checkuser = dbContext.users.FirstOrDefault(x => x.Id == request.Userid);
            if (checkuser is null) { return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại userid{request.Userid} ", null); }
            var checkrank = dbContext.rankCustomers.FirstOrDefault(x => x.Id == request.RankCustomerId);
            if (checkrank is null) { return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại rank {request.RankCustomerId} ", null); }
            var checkuserstatus = dbContext.userStatuses.FirstOrDefault(x => x.Id == request.UserStatusId);
            if (checkuserstatus is null) { return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Không tồn tại userstatust{request.UserStatusId} ", null); }
            if (Validate.IsValidEmail(request.Email) == false)
            {
                return responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng Email không hợp lệ", null);
            }
            checkuser.UserName = request.UserName;
            checkuser.Password = BcryptNet.HashPassword(request.Password);
            checkuser.Email = request.Email;
            checkuser.RankCustomerId = request.RankCustomerId;
            checkuser.UserStatusId = request.UserStatusId;
            checkuser.Name = request.Name;
            checkuser.PhoneNumber = request.PhoneNumber;
            checkuser.IsActive = request.IsActive;
            dbContext.users.Update(checkuser);
            dbContext.SaveChanges();
            return responseObject.ResponseSuccess("Sửa thành công", userConverter.EntitytoDTO(checkuser));


        }

        public IQueryable<GetUserDTO> TimKiemNguoiDung(string keyword)
        {
            // Khởi tạo truy vấn
            IQueryable<User> query = dbContext.users.AsQueryable();

            // Kiểm tra xem keyword có tồn tại hay không
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string lowerKeyword = keyword.ToLower(); // Chuyển về chữ thường để tìm kiếm không phân biệt chữ hoa/thường

                // Tìm kiếm dựa trên các trường: Tên, Username, Email, Số điện thoại
                query = query.Where(user =>
                    user.Name.ToLower().Contains(lowerKeyword) ||           // Tìm theo tên
                    user.UserName.ToLower().Contains(lowerKeyword) ||       // Tìm theo username
                    user.Email.ToLower().Contains(lowerKeyword) ||          // Tìm theo email
                    user.PhoneNumber.ToLower().Contains(lowerKeyword));     // Tìm theo số điện thoại
            }

            // Chuyển đổi kết quả sang DTO
            var result = query.Select(u => _userConverter.EntitytoDTO(u));
            return result;
        }

    }
}

