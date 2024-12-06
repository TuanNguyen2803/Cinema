using DATN.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Import for IConfiguration

namespace DATN.DataContext
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Constructor nhận DbContextOptions và inject IConfiguration
        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        // Định nghĩa các DbSet cho các thực thể trong hệ thống
        public DbSet<Banner> banners { get; set; }
        public DbSet<Bill> bills { get; set; }
        public DbSet<BillFood> billFoods { get; set; }
        public DbSet<BillStatus> billStatuses { get; set; }
        public DbSet<BillTicket> billTickets { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<ConfirmEmail> confirmEmails { get; set; }
        public DbSet<Food> foods { get; set; }
        public DbSet<GeneralSetting> generalSettings { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<MovieType> movieTypes { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<RankCustomer> rankCustomers { get; set; }
        public DbSet<Rate> rates { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<Room> rooms { get; set; }
        public DbSet<Schedule> schedules { get; set; }
        public DbSet<Seat> seats { get; set; }
        public DbSet<SeatStatus> seatsStatus { get; set; }
        public DbSet<SeatType> seatTypes { get; set; }
        public DbSet<Ticket> tickets { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<UserStatus> userStatuses { get; set; }

        // Sử dụng IConfiguration để lấy chuỗi kết nối từ appsettings.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
