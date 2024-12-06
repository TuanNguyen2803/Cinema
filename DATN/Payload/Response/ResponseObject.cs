namespace DATN.Payload.Response
{
    public class ResponseObject<T>
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
        public DateTime ResponseDate { get; set; }
        public T Data { get; set; }

        // Constructor mặc định
        public ResponseObject()
        {
        }

        // Constructor đầy đủ
        public ResponseObject(int status, string message, int code, DateTime responseDate, T data)
        {
            Status = status;
            Message = message;
            Code = code;
            ResponseDate = responseDate;
            Data = data;
        }

        // Phương thức trả về thành công với đối tượng đơn
        public ResponseObject<T> ResponseSuccess(string message, T data)
        {
            return new ResponseObject<T>(0, message, StatusCodes.Status200OK, DateTime.Now, data);
        }

        // Phương thức trả về lỗi với đối tượng đơn
        public ResponseObject<T> ResponseError(int statusCode, string message, T data)
        {
            return new ResponseObject<T>(0, message, statusCode, DateTime.Now, data);
        }

        // --- Thêm các phương thức cho danh sách đối tượng ---

        // Phương thức trả về thành công với danh sách đối tượng
        public ResponseObject<List<T>> ResponseSuccessList(string message, List<T> dataList)
        {
            return new ResponseObject<List<T>>(0, message, StatusCodes.Status200OK, DateTime.Now, dataList);
        }

        // Phương thức trả về lỗi với danh sách đối tượng
        public ResponseObject<List<T>> ResponseErrorList(int statusCode, string message, List<T> dataList)
        {
            return new ResponseObject<List<T>>(0, message, statusCode, DateTime.Now, dataList);
        }

        // --- Thêm các phương thức trả về với dữ liệu là null ---

        // Thành công nhưng không có dữ liệu
        public ResponseObject<T> ResponseSuccessNoData(string message)
        {
            return new ResponseObject<T>(0, message, StatusCodes.Status200OK, DateTime.Now, default(T));
        }

        // Lỗi nhưng không có dữ liệu
        public ResponseObject<T> ResponseErrorNoData(int statusCode, string message)
        {
            return new ResponseObject<T>(0, message, statusCode, DateTime.Now, default(T));
        }
    }
}
