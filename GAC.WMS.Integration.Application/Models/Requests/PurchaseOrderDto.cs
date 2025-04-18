using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        [Required] public string OrderId { get; set; }
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; }
    }
}
