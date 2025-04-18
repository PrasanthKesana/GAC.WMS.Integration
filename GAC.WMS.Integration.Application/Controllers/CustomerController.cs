using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Integration.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("GetCustomer/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [HttpPost("CreateCustomer")]
        public async Task<IActionResult> Create([FromBody] CustomerDto customer)
        {
            var createdCustomer = await _customerService.CreateCustomerAsync(customer);
            return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
        }

        [HttpPut("UpdateCustomer/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerDto customerDto)
        {
            if (id != customerDto.Id)
            {
                return BadRequest();
            }

            await _customerService.UpdateCustomerAsync(customerDto);
            return NoContent();
        }

        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _customerService.DeleteCustomerAsync(id);
            return NoContent();
        }
    }
}
