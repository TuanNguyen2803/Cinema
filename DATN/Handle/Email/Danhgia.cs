using DATN.Entities;

namespace DATN.Handle.Email
{
    public class Danhgia
    {
        public string GenerateRatingEmail(Bill bill)
        {
            try
            {
                // URL cơ bản để xử lý đánh giá, thêm `BillId` để nhận diện hóa đơn
                string ratingUrlBase = "https://yourwebsite.com/rating?billId=" + bill.Id;

                // Nội dung HTML của email với hình ngôi sao có thể nhấp
                string htmlContent = $@"
            <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        h1 {{ color: #333; }}
                        .star-rating {{ text-align: center; margin-top: 20px; }}
                        .star-rating a {{
                            text-decoration: none;
                            color: #FFD700;
                            font-size: 30px;
                        }}
                    </style>
                </head>
                <body>
                    <h1>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</h1>
                    <p>Chúng tôi rất mong nhận được đánh giá từ bạn:</p>

                    <div class='star-rating'>
                        <a href='{ratingUrlBase}&rating=1'>★</a>
                        <a href='{ratingUrlBase}&rating=2'>★</a>
                        <a href='{ratingUrlBase}&rating=3'>★</a>
                        <a href='{ratingUrlBase}&rating=4'>★</a>
                        <a href='{ratingUrlBase}&rating=5'>★</a>
                    </div>

                    <p>Chọn số lượng sao bạn muốn đánh giá. Mỗi sao đại diện cho một mức độ hài lòng.</p>
                    <p>1 sao là rất không hài lòng, và 5 sao là rất hài lòng.</p>

                    <div class='footer'>
                        <p>Trân trọng,</p>
                        <p>MyBugs Cinema</p>
                    </div>
                </body>
            </html>";

                return htmlContent;
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi khi tạo email đánh giá: {ex.Message}"; // Trả về thông báo lỗi nếu có
            }
        }
    }
}
