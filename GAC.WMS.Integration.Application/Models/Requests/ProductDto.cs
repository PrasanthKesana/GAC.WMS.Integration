using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class ProductDto
    {
        public string ProductCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public bool IsActive { get; set; }
    }
}
