using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Domain.Models
{
    public class Product
    {
        [Key]
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    }
}
