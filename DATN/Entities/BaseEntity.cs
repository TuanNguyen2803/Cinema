using System.ComponentModel.DataAnnotations;

namespace DATN.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
