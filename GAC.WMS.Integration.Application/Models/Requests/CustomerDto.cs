using System.ComponentModel.DataAnnotations;

namespace GAC.WMS.Integration.Application.Models.Requests
{
    public class CustomerDto
    {
        public int Id { get; set; }
        [Required] public string CustomerIdentifier { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        [Required] public string Address { get; set; } = null!;
    }
}
