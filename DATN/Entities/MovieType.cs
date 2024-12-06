namespace DATN.Entities
{
    public class MovieType : BaseEntity
    {
        public string MovieTypeName { get; set; }
        public bool? IsActive { get; set; } = true;
        public IEnumerable<Movie>? Movies { get; set; }
    }
}
