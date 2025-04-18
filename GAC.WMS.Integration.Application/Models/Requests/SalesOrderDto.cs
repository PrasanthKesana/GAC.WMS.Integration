using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public string ShipmentAddress { get; set; }
        public List<SalesOrderItemDto> Items { get; set; }
    }
}
