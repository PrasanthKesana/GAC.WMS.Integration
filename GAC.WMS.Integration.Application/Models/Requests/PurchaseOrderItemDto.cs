using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class PurchaseOrderItemDto
    {
        [Required] public string ProductCode { get; set; } = null!;
        [Range(1, int.MaxValue)] public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
