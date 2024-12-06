using DATN.Payload.DTO;
using DATN.Payload.Request.BillFoodRequest;
using DATN.Payload.Request.BillRequest;
using DATN.Payload.Response;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DATN.Service.Interface
{
    public interface IBillService
    {
       ResponseObject<BillDTO> Create(int userid,CreateBillRequest bookingRequest);

        IQueryable<CinemaStatisticsDTO> ThongKeDoanhSo();
        IQueryable<FoodRDTO> Thongkedoan();
        IQueryable<MovieRevenueDTO> ThongKeDoanhThuTheoPhim();
        IQueryable<CinemaStatisticsDTO> ThongKeTheoNgay(DateTime ngayThongKe);
        public int  ThongKeGheConTrong(int scheduleId);
        IQueryable<CinemaStatisticsDTO> ThongKeTheoThang(int month, int year);
        IQueryable<CinemaStatisticsDTO> ThongKeTheoNam(int year);
        IQueryable<dynamic> ThongKeDoanhThuTheoThang();
        IQueryable<RevenueByMonthDTO> DoanhThuTheoThang();
        IQueryable<BillDTO> GetBillsByUserId(int userId);
        IQueryable<BillDTO> GetBillsByDay();
    }
}
