
using CloudinaryDotNet.Actions;
using DATN.DataContext;
using DATN.Entities;
using DATN.Payload.Converter;
using DATN.Payload.DTO;
using DATN.Payload.Request.BillFoodRequest;
using DATN.Payload.Request.BillRequest;
using DATN.Payload.Response;
using DATN.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Text;
using System;
using System.Drawing;
using System.Linq;


namespace DATN.Service.Implement
{
    public class BillService : IBillService
    {
        private readonly AppDbContext _context;
        private readonly BillConverter billConverter;
        private readonly ResponseObject<BillDTO> responseObject;
        private readonly IConfiguration _configuration;

        public BillService(AppDbContext context, BillConverter billConverter, ResponseObject<BillDTO> responseObject, IConfiguration configuration)
        {
            _context = context;
            this.billConverter = billConverter;
            this.responseObject = responseObject;
            _configuration = configuration;
        }


        public void UpdateSeatStatusAfterShowTime(Schedule schedule,int roomid)
        {
            // Tính thời gian kết thúc của suất chiếu
            DateTime endTime = schedule.StartAt.AddMinutes(schedule.Movie.MovieDuration + 10);

            // Kiểm tra nếu đã qua thời gian kết thúc suất chiếu
            if (DateTime.Now >= endTime)
            {
                // Lấy danh sách các ghế đã được đặt trong suất chiếu này
                var seatsToUpdate = _context.seats
                    .Where(s => s.RoomId == roomid && s.SeatStatusId == 2)
                    .ToList();

                // Cập nhật trạng thái ghế về lại trạng thái 1 (trống)
                foreach (var seat in seatsToUpdate)
                {
                    seat.SeatStatusId = 1;
                    _context.seats.Update(seat);
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
        }


        public ResponseObject<BillDTO> Create(int userId, CreateBillRequest bookingRequest)
        {
            var user = _context.users.FirstOrDefault(x => x.Id == userId);
            if (user is null)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
            }

            var movie = _context.movies.Find(bookingRequest.MovieId);
            if (movie is null || movie.IsActive == false)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Phim không tồn tại", null);
            }

            var cinema = _context.cinemas.Find(bookingRequest.CinemaId);
            if (cinema is null || cinema.IsActive == false)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Rạp chiếu không tồn tại", null);
            }

            var room = _context.rooms.Find(bookingRequest.RoomId);
            if (room is null || room.IsActive == false)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Phòng chiếu không tồn tại", null);
            }

            var schedule = _context.schedules.Find(bookingRequest.ScheduleId);
            if (schedule is null || schedule.IsActive == false)
            {
                return responseObject.ResponseError(StatusCodes.Status404NotFound, "Suất chiếu không tồn tại", null);
            }

            foreach (var seat in bookingRequest.SeatList)
            {
                var s = _context.seats.Find(seat.seatId);
                if (s == null || s.SeatStatusId == 2)
                {
                    return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Ghế có id là {seat.seatId} không tồn tại hoặc đã được đặt", null);
                }
            }

            var promotion = _context.promotions.FirstOrDefault(x => x.RankCustomerId == user.RankCustomerId);

            var bill = new Bill
            {
                TotalMoney = TotalPrice(schedule.Price, bookingRequest.SeatList, bookingRequest.billFoods ?? new List<CreateBillFoodRequest>(), promotion),
                TradingCode = GenerateTradingCode(),
                CreateTime = DateTime.Now,
                UserId = user.Id,
                Name = TaoName(),
                BillStatusId = 1,
                PromotionId = _context.promotions
                .Include(x => x.RankCustomer)
                .ThenInclude(x => x.Users.Where(x => x.Id == userId))
                .Select(x => x.Id).FirstOrDefault(),
                IsActive = true
            };
            _context.bills.Add(bill);
            _context.SaveChanges();

            var tickets = CreateTickets(schedule, room, bookingRequest.SeatList, bill.Id);
            if (tickets == null || tickets.Count != bookingRequest.SeatList.Count)
            {
                return responseObject.ResponseError(StatusCodes.Status500InternalServerError, "Không thể tạo đủ số vé hoặc không có vé", null);
            }

            _context.SaveChanges();

            if (bookingRequest.billFoods != null && bookingRequest.billFoods.Any())
            {
                foreach (var foodItem in bookingRequest.billFoods)
                {
                    var food = _context.foods.Find(foodItem.FoodId);
                    if (food is null || food.IsActive == false)
                    {
                        return responseObject.ResponseError(StatusCodes.Status404NotFound, $"Món ăn có id là {foodItem.FoodId} không tồn tại hoặc không hoạt động", null);
                    }

                    var billfood = new BillFood
                    {
                        Quantity = foodItem.Quantity,
                        FoodId = foodItem.FoodId,
                        BillId = bill.Id
                    };
                    _context.billFoods.Add(billfood);
                }

                _context.SaveChanges();
            }

            // Cập nhật ghế cho hóa đơn hết hạn
            UpdateSeatsForExpiredBills();
            //UpdateSeatsAfterMovieEnd();

            return responseObject.ResponseSuccess("Tạo hóa đơn thành công vui lòng thanh toán", billConverter.EntityToDTO(bill));
        }

        private void UpdateSeatsForExpiredBills()
        {
            // Lấy tất cả các hóa đơn chưa thanh toán và đã tạo hơn 10 phút
            var expiredBills = _context.bills
                .Where(b => b.BillStatusId == 1 && b.CreateTime <= DateTime.Now.AddMinutes(-10))
                .ToList();

            // Lưu trữ danh sách ghế cần cập nhật
            var seatsToUpdate = new List<Seat>();

            foreach (var bill in expiredBills)
            {
                // Lấy tất cả các vé liên quan đến hóa đơn này
                var seatsInBill = _context.billTickets
                    .Where(bt => bt.BillId == bill.Id)
                    .Select(bt => bt.Ticket.Seat)
                    .ToList();

                // Cập nhật trạng thái của các ghế về trạng thái 1 (trống)
                foreach (var seat in seatsInBill)
                {
                    seat.SeatStatusId = 1; // Cập nhật trạng thái ghế về trống
                    seatsToUpdate.Add(seat);
                }

                // Đánh dấu hóa đơn là đã hết hạn (hoặc bị hủy)
                bill.BillStatusId = 4; // Trạng thái 4: Đã hết hạn
                _context.bills.Update(bill);
            }

            // Cập nhật tất cả các ghế và hóa đơn vào cơ sở dữ liệu
            if (seatsToUpdate.Any())
            {
                _context.seats.UpdateRange(seatsToUpdate);
            }

            if (expiredBills.Any())
            {
                _context.bills.UpdateRange(expiredBills);
            }

            _context.SaveChanges();
        }

        public void UpdateSeatsAfterMovieEnd()
        {
            // Lấy tất cả các hóa đơn đã thanh toán (BillStatusId == 2)
            var paidBills = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true)
                .Include(b => b.BillTickets) // Bao gồm các vé trong hóa đơn
                .ThenInclude(bt => bt.Ticket) // Bao gồm thông tin vé
                .ThenInclude(t => t.Seat) // Bao gồm thông tin ghế
                .Include(b => b.BillTickets.Select(bt => bt.Ticket.Schedule)) // Bao gồm thông tin suất chiếu
                .ThenInclude(s => s.Movie) // Bao gồm thông tin phim
                .ToList();

            // Lưu danh sách ghế cần cập nhật
            var seatsToUpdate = new List<Seat>();

            foreach (var bill in paidBills)
            {
                foreach (var billTicket in bill.BillTickets)
                {
                    var schedule = billTicket.Ticket.Schedule;
                    var movie = schedule.Movie;

                    // Tính thời gian kết thúc của phim + 10 phút
                    DateTime movieEndTime = schedule.StartAt.AddMinutes(movie.MovieDuration + 10);

                    // Kiểm tra xem đã hết thời gian chiếu + 10 phút hay chưa
                    if (DateTime.Now >= movieEndTime)
                    {
                        // Cập nhật trạng thái ghế về 1 (trống)
                        var seat = billTicket.Ticket.Seat;
                        seat.SeatStatusId = 1;
                        seatsToUpdate.Add(seat);
                    }
                }
            }

            // Cập nhật trạng thái tất cả các ghế
            if (seatsToUpdate.Any())
            {
                _context.seats.UpdateRange(seatsToUpdate);
                _context.SaveChanges();
            }
        }


        private List<Ticket> CreateTickets(Schedule schedule, Room room, List<SeatRequest> seatRequests, int billId)
        {
            var tickets = new List<Ticket>();

            foreach (var seatRequest in seatRequests)
            {
                var seat = _context.seats.FirstOrDefault(s => s.Id == seatRequest.seatId && s.RoomId == room.Id && s.IsActive == true);
                if (seat == null)
                {
                    return null; // Bỏ qua nếu ghế không tồn tại
                }

                // Tính giá vé dựa trên SeatType
                double priceTicket = schedule.Price;  // Mặc định giá vé bằng giá lịch chiếu
                if (seat.SeatTypeId == 2)
                {
                    priceTicket = 2 * schedule.Price;  // Ghế loại 2: giá vé gấp 2 lần lịch chiếu
                }
                else if (seat.SeatTypeId == 3)
                {
                    priceTicket = 1.5 * schedule.Price;  // Ghế loại 3: giá vé gấp 1.5 lần lịch chiếu
                }

                var ticket = new Ticket
                {
                    Code = GenerateTradingCode(),
                    ScheduleId = schedule.Id,
                    SeatId = seat.Id,
                    PriceTicket = priceTicket,  // Gán giá vé đã tính
                    IsActive = true
                };

                _context.tickets.Add(ticket);
                tickets.Add(ticket);
                _context.SaveChanges();

                _context.billTickets.Add(new BillTicket
                {
                    Quantity = 1,
                    BillId = billId,
                    TicketId = ticket.Id
                });
            }

            return tickets;
        }


        private double TotalPrice(double ticketPrice, List<SeatRequest> seatlist, List<CreateBillFoodRequest> billFoodRequests, Promotion promotion)
        {
            double foodTotalPrice = 0;

            foreach (var foodItem in billFoodRequests)
            {
                var food = _context.foods.Find(foodItem.FoodId);
                if (food != null && food.IsActive == true)
                {
                    foodTotalPrice += food.Price * foodItem.Quantity;
                }
            }
            double totalPrice = (ticketPrice * seatlist.Count) + foodTotalPrice;
            if (promotion != null)
            {
                totalPrice -= (totalPrice * promotion.Percent / 100);
            }
            return totalPrice;
        }

    
    private string TaoName()
        {
            var res = "HD" + "_";
            var countSohoadon = _context.bills.Count(x => x.CreateTime.Date == DateTime.Now.Date);
            if (countSohoadon > 0)
            {
                int tmp = countSohoadon + 1;
                if (tmp < 10)
                {

                    return res + "00" + tmp.ToString();
                }
                else if (tmp < 100)
                {
                    return res + "0" + tmp.ToString();
                }
                else
                {
                    return res + tmp.ToString();
                }
            }
            else
            {
                return res + "001".ToString();
            }
        }
         private string GenerateTradingCode(int length = 8)
        {
            string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();
            char[] tradingCodeArray = new char[length];
            for (int i = 0; i < length; i++)
            {
                tradingCodeArray[i] = characters[random.Next(characters.Length)];
            }

            string tradingCode = new string(tradingCodeArray);

            return tradingCode;
        }

        public IQueryable<CinemaStatisticsDTO> ThongKeDoanhSo()
        {
            // Lấy tất cả các hóa đơn đã thanh toán và còn hoạt động từ trước đến nay
            IQueryable<Bill> query = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true); // Chỉ chọn hóa đơn đã thanh toán và còn hoạt động

            // Nhóm hóa đơn theo rạp chiếu và tính tổng doanh thu và số vé bán từ trước đến nay
            var result = query
                .SelectMany(b => b.BillTickets) // Lấy tất cả các vé từ các hóa đơn
                .GroupBy(bt => bt.Ticket.Schedule.Room.CinemaId) // Nhóm theo CinemaId
                .Where(g => g.Key != null) // Lọc ra những nhóm có CinemaId khác null
                .Select(g => new CinemaStatisticsDTO
                {
                    CinemaId = g.Key,
                    CinemaName = _context.cinemas.Where(c => c.Id == g.Key).Select(c => c.NameOfCinema).FirstOrDefault(), // Lấy tên rạp
                    TotalTicketsSold = g.Count(), // Tính tổng số vé đã bán từ trước đến nay
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket) // Tính tổng doanh thu từ trước đến nay
                });

            return result;
        }

        public IQueryable<MovieRevenueDTO> ThongKeDoanhThuTheoPhim()
        {
            // Truy vấn hóa đơn đã thanh toán và còn hoạt động
            IQueryable<Bill> query = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true);

            // Nhóm theo phim và tính toán tổng doanh thu và tổng vé đã bán
            var result = query
                .SelectMany(b => b.BillTickets) // Lấy tất cả vé từ hóa đơn
                .GroupBy(bt => bt.Ticket.Schedule.MovieId) // Nhóm theo phim
                .Select(g => new MovieRevenueDTO
                {
                    MovieId = g.Key,
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket), // Tổng doanh thu từ vé
                    TotalTicketsSold = g.Count(), // Tổng số vé đã bán
                    MovieName = _context.movies.Where(m => m.Id == g.Key).Select(m => m.Name).FirstOrDefault() // Lấy tên phim
                });

            return result;
        }

        public IQueryable<FoodRDTO> Thongkedoan()
        {
            var day = DateTime.UtcNow.AddDays(-7);
            var result = _context.bills
                       .Where(b => b.CreateTime >= day)
                       .Include(b => b.BillFoods.OrderByDescending(bf => bf.Quantity))
                       .SelectMany(b => b.BillFoods)
                       .GroupBy(bf => bf.FoodId)
                        .Select(g => new FoodRDTO
                        {
                            FoodId = g.Key,
                            TotalQuantitySold = g.Sum(bf => bf.Quantity),
                            FoodName = g.FirstOrDefault().Food.NameOfFood
                        })
                         .OrderByDescending(dto => dto.TotalQuantitySold)
                         .AsQueryable();

            return result;
        }


        public int ThongKeGheConTrong(int scheduleId)
        {
            // Lấy thông tin lịch chiếu
            var schedule = _context.schedules
                .Include(s => s.Room)
                .ThenInclude(r => r.Seats) // Bao gồm ghế trong phòng
                .FirstOrDefault(s => s.Id == scheduleId);

            if (schedule == null)
            {
                throw new ArgumentException("Lịch chiếu không tồn tại.");
            }

            // Đếm số ghế còn trống
            var emptySeatsCount = schedule.Room.Seats
                .Count(s => s.SeatStatusId == 1); // 1 có thể là trạng thái ghế trống

            return emptySeatsCount;
        }
        // Thống kê theo ngày
        public IQueryable<CinemaStatisticsDTO> ThongKeTheoNgay(DateTime ngayThongKe)
        {
            var result = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true && b.CreateTime.Date == ngayThongKe.Date)
                .SelectMany(b => b.BillTickets)
                .GroupBy(bt => new { bt.Ticket.Schedule.Room.CinemaId })
                .Select(g => new CinemaStatisticsDTO
                {
                    CinemaId = g.Key.CinemaId,
                    CinemaName = _context.cinemas.Where(c => c.Id == g.Key.CinemaId).Select(c => c.NameOfCinema).FirstOrDefault(),
                    TotalTicketsSold = g.Count(),
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket)
                });

            return result;
        }



        // Thống kê theo tháng
        public IQueryable<CinemaStatisticsDTO> ThongKeTheoThang(int month, int year)
        {

            var result = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true && b.CreateTime.Month == month && b.CreateTime.Year == year)
                .SelectMany(b => b.BillTickets)
                .GroupBy(bt => new { bt.Ticket.Schedule.Room.CinemaId })
                .Select(g => new CinemaStatisticsDTO
                {
                    CinemaId = g.Key.CinemaId,
                    CinemaName = _context.cinemas.Where(c => c.Id == g.Key.CinemaId).Select(c => c.NameOfCinema).FirstOrDefault(),
                    TotalTicketsSold = g.Count(),
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket)
                });

            return result;
        }


        // Thống kê theo năm
        public IQueryable<CinemaStatisticsDTO> ThongKeTheoNam(int year)
        {
            var result = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true && b.CreateTime.Year == year)
                .SelectMany(b => b.BillTickets)
                .GroupBy(bt => new { bt.Ticket.Schedule.Room.CinemaId })
                .Select(g => new CinemaStatisticsDTO
                {
                    CinemaId = g.Key.CinemaId,
                    CinemaName = _context.cinemas.Where(c => c.Id == g.Key.CinemaId).Select(c => c.NameOfCinema).FirstOrDefault(),
                    TotalTicketsSold = g.Count(),
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket)
                });

            return result;
        }
        public IQueryable<dynamic> ThongKeDoanhThuTheoThang()
        {
            // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;

            IQueryable<Bill> query = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true); // Chỉ chọn hóa đơn đã thanh toán

            // Lọc hóa đơn theo năm hiện tại
            query = query.Where(b => b.CreateTime.Year == currentYear);

            // Nhóm theo tháng và rạp, sau đó tính tổng doanh thu
            var result = query
                .SelectMany(b => b.BillTickets) // Lấy danh sách vé từ hóa đơn
                .GroupBy(bt => new
                {
                    Month = bt.Bill.CreateTime.Month, 
                    CinemaId = bt.Ticket.Schedule.Room.CinemaId 
                })
                .Select(g => new
                {
                    Year = currentYear,
                    Month = g.Key.Month, 
                    CinemaName = _context.cinemas.FirstOrDefault(c => c.Id == g.Key.CinemaId).NameOfCinema, 
                    TotalRevenue = g.Sum(bt => bt.Ticket.PriceTicket) 
                })
                .ToList()
                .GroupBy(x => new { x.Year, x.Month }) 
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenues = g.ToDictionary(
                        x => x.CinemaName,
                        x => x.TotalRevenue 
                    )
                });

            return result.AsQueryable();
        }
        public IQueryable<RevenueByMonthDTO> DoanhThuTheoThang()
        {
          
            int currentYear = DateTime.Now.Year;

            var billsQuery = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true && b.CreateTime.Year == currentYear);

          
            var result = from bill in billsQuery
                         group bill by bill.CreateTime.Month into g
                         select new RevenueByMonthDTO
                         {
                             Year = currentYear, 
                             Month = g.Key, 
                             TotalRevenue = Convert.ToDecimal(g.Sum(b => b.TotalMoney)), 
                             TotalTicketsSold = g.SelectMany(b => _context.billTickets.Where(dt => dt.BillId == b.Id))
                                                 .Sum(dt => dt.Quantity) 
                         };

            return result;
        }
        public IQueryable<BillDTO> GetBillsByUserId(int userId)
        {
            var bills = _context.bills
                .Where(b => b.UserId == userId)
                .Select(b => billConverter.EntityToDTO(b)); 

            return bills;
        }
        public IQueryable<BillDTO> GetBillsByDay()
        {
            
            DateTime currentDate = DateTime.Now.Date; 
            var bills = _context.bills
                .Where(b => b.BillStatusId == 2 && b.IsActive == true && b.CreateTime.Date == currentDate) 
                .Select(b => billConverter.EntityToDTO(b)); 

            return bills;
        }


    }

}

