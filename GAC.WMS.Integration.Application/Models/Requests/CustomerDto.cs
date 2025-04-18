using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string CustomerIdentifier { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
