
using DATN.ConfigModels.VnPayPayment;
using DATN.ConfigModels.VnPayPayment.Config;
using DATN.Controller;
using DATN.DataContext;
using DATN.HandleVNPayPayment;
using DATN.Implements;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Response;
using DATN.Service.Implement;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.Text;


internal class Program
{
    private static void Main(string[] args)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Hoặc LicenseContext.Commercial nếu bạn có giấy phép thương mại


        var builder = WebApplication.CreateBuilder(args);

        // Logging Configuration
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        // Services Configuration
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyCorsPolicy", build =>
            {
                build.WithOrigins("*")
                     .AllowAnyHeader()
                     .AllowAnyMethod();
            });
        });

        // Authentication Configuration
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration.GetSection("AppSetting:SecretKey").Value!))
            };
        });


        // Swagger Configuration
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger eShop Solution", Version = "v1" });
            x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Làm theo mẫu này. Example: Bearer {Token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        // DbContext Configuration
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value));
        var Vnpay = builder.Configuration.GetSection("VnPay").Get<DataVnpay>();
        builder.Services.AddScoped<DataVnpay>();
        builder.Services.AddScoped<VNPayLibrary>();
        builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection(VnPayConfig.ConfigName));
        builder.Services.AddScoped<UserSevice>();
        builder.Services.AddScoped<IVNPayService, VNPayService>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICinemaService, CinemaService>();
        builder.Services.AddScoped<IRoomService, RoomService>();
        builder.Services.AddScoped<ISeatService, SeatService>();
        builder.Services.AddScoped<IBillService, BillService>();
        builder.Services.AddScoped<IFoodService, FoodService>();
        builder.Services.AddScoped<IMovieService, MovieService>();
        builder.Services.AddScoped<IVNPayService, VNPayService>();
        builder.Services.AddScoped<IBannerService, BannerService>();
        builder.Services.AddScoped<IScheduleService, ScheduleService>();
        builder.Services.AddScoped<IRateService, RateService>();
        builder.Services.AddScoped<IUserService, UserSevice>();
        builder.Services.AddScoped<ISeatTypeService, SeatTypeService>();
        builder.Services.AddScoped<IUserStatusService, UserStatusService>();
        builder.Services.AddScoped<IBillStatusService, BillStatusService>();
        builder.Services.AddScoped<IMovieTypeService, MovieTypeService>();
        builder.Services.AddScoped<IPromotionService, PromotionService>();
        builder.Services.AddScoped<IRankCustomerService, RankCustomerService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<ISeatStatusService, SeatStatusService>();
        builder.Services.AddScoped<IGeneralSettingService, GeneralSettingService>();
        builder.Services.AddScoped<ResponseObject<TokenDTO>>();
        builder.Services.AddScoped<UserConverter>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ResponseObject<BillDTO>>();
        builder.Services.AddScoped<BillConverter>();
        builder.Services.AddScoped<ResponseObject<UserDTO>>();
        builder.Services.AddScoped<ResponseObject<CinemaDTO>>();
        builder.Services.AddScoped<CinemaConverter>();
        builder.Services.AddScoped<ResponseObject<FoodDTO>>();
        builder.Services.AddScoped<FoodConverter>();
        builder.Services.AddScoped<ResponseObject<MovieDTO>>();
        builder.Services.AddScoped<MovieConverter>();
        builder.Services.AddScoped<ResponseObject<RoomDTO>>();
        builder.Services.AddScoped<RoomConverter>();
        builder.Services.AddScoped<ScheduleConverter>();
        builder.Services.AddScoped<ResponseObject<ScheduleDTO>>();
        builder.Services.AddScoped<ResponseObject<SeatDTO>>();
        builder.Services.AddScoped<SeatConverter>();
        builder.Services.AddScoped<ResponseObject<BannerDTO>>();
        builder.Services.AddScoped<BannerConverter>();
        builder.Services.AddScoped<BannerDTO>();
        builder.Services.AddScoped<CinemaStatisticsDTO>();
        builder.Services.AddScoped<CinemaMonthlyRevenueDTO>();
        builder.Services.AddScoped<RateDTO>();
        builder.Services.AddScoped<RateConverter>();
        builder.Services.AddScoped<ResponseObject<RateDTO>>();
        builder.Services.AddScoped<UserDTO>();
        builder.Services.AddScoped<UserConverter>();
        builder.Services.AddScoped<ResponseObject<UserDTO>>();
        builder.Services.AddScoped<SeatTypeDTO>();
        builder.Services.AddScoped<SeatTypeConverter>();
        builder.Services.AddScoped<ResponseObject<SeatTypeDTO>>();
        builder.Services.AddScoped<UserStatusDTO>();
        builder.Services.AddScoped<UserStatusConverter>();
        builder.Services.AddScoped<ResponseObject<UserStatusDTO>>();
        builder.Services.AddScoped<GeneralSettingConverter>();
        builder.Services.AddScoped<GeneralSettingDTO>();
        builder.Services.AddScoped<ResponseObject<GeneralSettingDTO>>();
        builder.Services.AddScoped<RoleConverter>();
        builder.Services.AddScoped<RoleDTO>();
        builder.Services.AddScoped<ResponseObject<RoleDTO>>();
        builder.Services.AddScoped<SeatStatusConverter>();
        builder.Services.AddScoped<SeatStatusDTO>();
        builder.Services.AddScoped<ResponseObject<SeatStatusDTO>>();
        builder.Services.AddScoped<BillStatusConverter>();
        builder.Services.AddScoped<BillStatusDTO>();
        builder.Services.AddScoped<ResponseObject<BillStatusDTO>>();
        builder.Services.AddScoped<MovieTypeConverter>();
        builder.Services.AddScoped<MovieTypeDTO>();
        builder.Services.AddScoped<ResponseObject<MovieTypeDTO>>();
        builder.Services.AddScoped<PromotionConverter>();
        builder.Services.AddScoped<PromotionDTO>();
        builder.Services.AddScoped<ResponseObject<PromotionDTO>>();
        builder.Services.AddScoped<RankCustomerConverter>();
        builder.Services.AddScoped<RankCustomerDTO>();
        builder.Services.AddScoped<ResponseObject<RankCustomerDTO>>();
        builder.Services.AddScoped<GetUserConverter>();
        builder.Services.AddScoped<GetUserDTO>();



        // Build and Run the Application
        var app = builder.Build();

        // Middleware Configuration
        // Middleware Configuration
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("MyCorsPolicy"); // Giữ lại chỉ một middleware CORS

        app.MapControllers();

        app.Run();

    }
}
