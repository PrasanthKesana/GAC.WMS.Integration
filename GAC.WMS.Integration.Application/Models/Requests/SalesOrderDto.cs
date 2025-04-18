using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        [Required] public string OrderId { get; set; }
        public DateTime ProcessingDate { get; set; }
        [Required] public int CustomerId { get; set; }
        [Required] public string ShipmentAddress { get; set; }
        [MinLength(1)] public List<SalesOrderItemDto> Items { get; set; }
    }
}
