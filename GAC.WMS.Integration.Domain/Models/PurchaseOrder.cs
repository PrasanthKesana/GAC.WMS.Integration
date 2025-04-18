namespace GAC.WMS.Integration.Domain.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public DateTime ProcessingDate { get; set; } = DateTime.UtcNow;
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    }

    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }
        public string ProductCode { get; set; } = null!;
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
