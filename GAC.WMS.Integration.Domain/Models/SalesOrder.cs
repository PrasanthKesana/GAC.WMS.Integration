using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Domain.Models
{
    public class SalesOrder 
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public string ShipmentAddress { get; set; }
        public ICollection<SalesOrderItem> Items { get; set; } = new List<SalesOrderItem>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    }

    public class SalesOrderItem
    {
        public int Id { get; set; }
        public int SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public string ProductCode { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
