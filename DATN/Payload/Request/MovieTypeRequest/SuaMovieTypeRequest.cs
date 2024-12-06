namespace DATN.Payload.Request.MovieTypeRequest
{
    public class SuaMovieTypeRequest
    {
        public int Id
        {
            get; set;
        }
        public string MovieTypeName { get; set; }
        public bool IsActive { get; set; }
    }
}
