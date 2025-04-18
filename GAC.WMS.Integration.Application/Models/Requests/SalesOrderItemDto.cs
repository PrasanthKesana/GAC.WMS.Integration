using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class SalesOrderItemDto
    {
        public string ProductCode { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

    }
}
