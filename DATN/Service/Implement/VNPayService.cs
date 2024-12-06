using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATN.Service.Interface;
using DATN.ConfigModels.VnPayPayment;
using DATN.DataContext;
using DATN.Entities;
using DATN.HandleVNPayPayment;
using Utils = DATN.ConfigModels.VnPayPayment.Utils;
using DATN.Service.Implement;
using DATN.Handle.Email;
using System.ServiceModel.Channels;

namespace DATN.Implements
{
    public class VNPayService : IVNPayService
    {
        
        private readonly IConfiguration _configuration;
        private readonly VNPayLibrary vnpay;
        private readonly UserSevice userService;
        public readonly AppDbContext dbContext;
        public VNPayService(IConfiguration configuration, UserSevice _userService, AppDbContext context)
        {
            vnpay = new VNPayLibrary();
            _configuration = configuration;
            dbContext = context;
            userService = _userService;
        }



        public string TaoDuongDanThanhToan(int hoaDonId, HttpContext httpContext, int id)
        {
            var hoaDon = dbContext.bills.SingleOrDefault(x => x.Id == hoaDonId);
            var nguoiDung = dbContext.users.SingleOrDefault(x => x.Id == id);
            if (nguoiDung.Id == hoaDon.UserId)
            {

                if (hoaDon.BillStatusId == 2)
                {
                    return "Hóa đơn đã được thanh toán trước đó";
                }
                if (hoaDon.BillStatusId == 1 && hoaDon.TotalMoney != 0 && hoaDon.TotalMoney != null)
                {
                    VnPayLibrary vnpay = new VnPayLibrary();

                    vnpay.AddRequestData("vnp_Version", "2.1.0");
                    vnpay.AddRequestData("vnp_Command", "pay");
                    vnpay.AddRequestData("vnp_TmnCode", "UXCI740Y");
                    vnpay.AddRequestData("vnp_Amount", (hoaDon.TotalMoney * 100).ToString());
                    vnpay.AddRequestData("vnp_CreateDate", hoaDon.CreateTime.ToString("yyyyMMddHHmmss"));
                    vnpay.AddRequestData("vnp_CurrCode", "VND");
                    vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(httpContext));
                    vnpay.AddRequestData("vnp_Locale", "vn");
                    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán hóa đơn: " + hoaDon.Id);
                    vnpay.AddRequestData("vnp_OrderType", "other");
                    vnpay.AddRequestData("vnp_ReturnUrl", "https://localhost:7237/Return");
                    vnpay.AddRequestData("vnp_TxnRef", hoaDon.Id.ToString());
                    string ma = _configuration.GetSection("VnPay:HashSecret").Value;
                    string duongDanThanhToan = vnpay.CreateRequestUrl(_configuration.GetSection("VnPay:BaseUrl").Value, "IE4W0LTRHAD0FCWRRWRA5VS0H0B4NDN4");
                    return duongDanThanhToan;
                }

                return "Vui lòng kiểm tra lại hóa đơn";
            }
            return "Vui lòng kiểm tra lại thông tin người thanh toán";
        }


        public string VNPayReturn(IQueryCollection vnpayData)
        {
            string vnp_TmnCode = _configuration.GetSection("VnPay:TmnCode").Value;
            string vnp_HashSecret = _configuration.GetSection("VnPay:HashSecret").Value;

            var vnPayLibrary = new VnPayLibrary();
            foreach (var (key, value) in vnpayData)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPayLibrary.AddResponseData(key, value);
                }
            }

            string hoaDonId = vnPayLibrary.GetResponseData("vnp_TxnRef");
            string vnp_ResponseCode = vnPayLibrary.GetResponseData("vnp_ResponseCode");
            string vnp_TransactionStatus = vnPayLibrary.GetResponseData("vnp_TransactionStatus");
            string vnp_SecureHash = vnPayLibrary.GetResponseData("vnp_SecureHash");
            double vnp_Amount = Convert.ToDouble(vnPayLibrary.GetResponseData("vnp_Amount"));
            bool check = vnPayLibrary.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            var billEmailTemplate = new BillEmailTemplate(dbContext);
            if (check)
            {
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var hoaDon = dbContext.bills.FirstOrDefault(x => x.Id == Convert.ToInt32(hoaDonId));

                    if (hoaDon == null)
                    {
                        return "Không tìm thấy hóa đơn";
                    }

                    hoaDon.BillStatusId = 2;
                    hoaDon.CreateTime = DateTime.Now;

                    var user = dbContext.users.FirstOrDefault(x => x.Id == hoaDon.UserId);
                    if (user != null)
                    {
                        string email = user.Email;
                        string mss = userService.SendEmail(new EmailTo
                        {
                            Mail = email,
                            Subject = $"Thanh Toán đơn hàng: {hoaDon.Id}",
                            Content = billEmailTemplate.GenerateNotificationBillEmail(hoaDon, "đã thanh toán")
                        });
                    }
                    user.Point += 50;
                    if (user.Point >= 1000)
                    {
                        user.RankCustomerId = 4;
                    }
                    else if (user.Point >= 500)
                    {
                        user.RankCustomerId = 2;
                    }
                    else if (user.Point >= 100)
                    {
                        user.RankCustomerId = 1;
                    }
                    else
                    {
                        user.RankCustomerId = 5;
                    }

                    dbContext.users.Update(user);
                    dbContext.SaveChanges();

                  
                        var billticket = dbContext.billTickets.Where(bt => bt.BillId == hoaDon.Id).ToList();
                        if (billticket.Any())
                        {
                            var ticketIds = billticket.Select(bt => bt.TicketId).ToList();
                            var tickets = dbContext.tickets.Where(t => ticketIds.Contains(t.Id)).ToList();
                            if (tickets.Any())
                            {
                                var seatIds = tickets.Select(t => t.SeatId).ToList();
                                var seats = dbContext.seats.Where(s => seatIds.Contains(s.Id)).ToList();
                                foreach (var seat in seats)
                                {
                                    seat.SeatStatusId = 2;
                                    dbContext.seats.Update(seat);
                                }
                                dbContext.SaveChanges();
                            }
                        }


                    return $"Thanh toán thành công hóa đơn {hoaDon.Name}. Vui lòng kiểm tra email";
                }
                else
                {
                    return $"Lỗi trong khi thực hiện giao dịch. Mã lỗi: {vnp_ResponseCode}";
                }
            }
            else
            {
                return "Có lỗi trong quá trình xử lý";
            }
        }
    }
}
