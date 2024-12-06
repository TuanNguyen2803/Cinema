using DATN.DataContext;
using DATN.Entities;

public class BillEmailTemplate
{
    private readonly AppDbContext _context;

    // Constructor để inject AppDbContext
    public BillEmailTemplate(AppDbContext context)
    {
        _context = context;
    }

    // Phương thức tạo nội dung email thông báo hóa đơn
    public string GenerateNotificationBillEmail(Bill bill, string message = "")
    {
        try
        {
            // Sử dụng ngữ cảnh cơ sở dữ liệu để truy vấn
            var ticketIds = _context.billTickets
                                    .Where(bt => bt.BillId == bill.Id)
                                    .Select(bt => bt.TicketId)
                                    .ToList();

            // Lấy danh sách ScheduleId và SeatId từ các TicketId
            var schedules = _context.tickets
                                    .Where(t => ticketIds.Contains(t.Id))
                                    .Select(t => new { t.ScheduleId, t.SeatId }) // Vẫn giữ kiểu ẩn danh
                                    .ToList(); // Sử dụng List<dynamic>

            // Lấy thông tin từ ScheduleId
            var scheduleInfo = _context.schedules
                                       .Where(s => schedules.Select(sc => sc.ScheduleId).Contains(s.Id))
                                       .Select(s => new { s.RoomId, s.MovieId, s.StartAt })
                                       .ToList();

            // Lấy thông tin phòng từ RoomId
            var roomInfo = _context.rooms
                                  .Where(r => scheduleInfo.Select(s => s.RoomId).Contains(r.Id))
                                  .Select(r => new { r.CinemaId, r.Name })
                                  .Distinct()
                                  .ToList();

            // Lấy thông tin rạp từ CinemaId
            var cinemaInfo = _context.cinemas
                                    .Where(c => roomInfo.Select(r => r.CinemaId).Contains(c.Id))
                                    .Select(c => new { c.NameOfCinema })
                                    .FirstOrDefault();

            // Lấy thông tin phim từ MovieId
            var movieInfo = _context.movies
                                   .Where(m => scheduleInfo.Select(s => s.MovieId).Contains(m.Id))
                                   .Select(m => new { m.Name })
                                   .FirstOrDefault();

            // Lấy số ghế từ SeatId
            var seatNumbers = _context.seats
                            .Where(s => schedules.Select(sc => sc.SeatId).Contains(s.Id))
                            .Select(s => new { s.Line, s.Number })
                            .ToList();

            // Lấy thông tin đồ ăn từ BillFood
            var foodInfo = _context.billFoods
                                  .Where(bf => bf.BillId == bill.Id)
                                  .Select(bf => new { bf.Food.NameOfFood, bf.Quantity })
                                  .ToList();

            // Định dạng số ghế và đồ ăn thành chuỗi HTML
            var seatNumbersHtml = string.Join(", ", seatNumbers.Select(sn => $"{sn.Line}{sn.Number}"));
            var foodHtml = foodInfo.Any() ? string.Join(", ", foodInfo.Select(f => $"{f.NameOfFood} (Số lượng: {f.Quantity})")) : "Không có đồ ăn";

            // Lấy thông tin thời gian chiếu
            var showDateTime = scheduleInfo.FirstOrDefault()?.StartAt;
            var showDate = showDateTime?.ToString("dd/MM/yyyy") ?? "N/A";
            var showTime = showDateTime?.ToString("HH:mm") ?? "N/A";

            // Tạo chuỗi thông tin hóa đơn cho mã QR
            var qrData = $@"
                Tên hóa đơn: {bill.Name}
                Tổng tiền: {bill.TotalMoney.ToString("N0")} VND
                Trạng thái: {_context.billStatuses.SingleOrDefault(x => x.Id == bill.BillStatusId)?.Name ?? "Trạng thái không xác định"}
                Tên khách hàng: {_context.users.SingleOrDefault(x => x.Id == bill.UserId)?.Name ?? "Khách hàng không xác định"}
                Tên rạp: {cinemaInfo?.NameOfCinema}
                Tên phòng: {roomInfo.FirstOrDefault()?.Name}
                Tên phim: {movieInfo?.Name}
                Ngày chiếu: {showDate}
                Giờ chiếu: {showTime}
                Số ghế: {seatNumbersHtml}
                Đồ ăn: {foodHtml}";

            // Tạo URL mã QR
            string qrCodeUrl = $"https://api.qrserver.com/v1/create-qr-code/?size=300x300&data={Uri.EscapeDataString(qrData)}";

            // Nội dung HTML của email
            string htmlContent = $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; }}
                            img {{ width: 300px; height: auto; }}
                            h1 {{ color: #333; }}
                            .footer {{ margin-top: 20px; font-size: 14px; }}
                        </style>
                    </head>
                    <body>
                        <h1>Thông tin hóa đơn đặt vé</h1>
                        <h2 style=""color: red; font-size: 20px; font-weight: bold;"">{(string.IsNullOrEmpty(message) ? "" : message)}</h2>

                        <p><strong>Tên hóa đơn:</strong> {bill.Name}</p>
                        <p><strong>Tổng tiền:</strong> {bill.TotalMoney.ToString("N0")} VND</p>
                        <p><strong>Trạng thái:</strong> {_context.billStatuses.SingleOrDefault(x => x.Id == bill.BillStatusId)?.Name ?? "Trạng thái không xác định"}</p>
                        <p><strong>Tên khách hàng:</strong> {_context.users.SingleOrDefault(x => x.Id == bill.UserId)?.Name ?? "Khách hàng không xác định"}</p>
                        <p><strong>Tên rạp:</strong> {cinemaInfo?.NameOfCinema}</p>
                        <p><strong>Tên phòng:</strong> {roomInfo.FirstOrDefault()?.Name}</p>
                        <p><strong>Tên phim:</strong> {movieInfo?.Name}</p>
                        <p><strong>Ngày chiếu:</strong> {showDate}</p>
                        <p><strong>Giờ chiếu:</strong> {showTime}</p>
                        <p><strong>Số ghế:</strong> {seatNumbersHtml}</p>
                        <p><strong>Đồ ăn:</strong> {foodHtml}</p>

                        <h2>Mã QR cho thông tin đặt vé:</h2>
                        <img src='{qrCodeUrl}' alt='Mã QR Thanh Toán'  />

                        <div class=""footer"">
                            <p>Trân trọng,</p>
                            <p>MyBugs Cinema</p>
                        </div>
                    </body>
                </html>";

            return htmlContent; // Trả về nội dung HTML
        }
        catch (Exception ex)
        {
            return $"Đã xảy ra lỗi khi tạo email: {ex.Message}"; // Trả về thông báo lỗi nếu có
        }
    }
}
