using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Integration.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrdersController : ControllerBase
    {
        private readonly ISalesOrderService _salesOrderService;

        public SalesOrdersController(ISalesOrderService salesOrderService)
        {
            _salesOrderService = salesOrderService;
        }

        [HttpGet("GetAllSalesOrders")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _salesOrderService.GetAllSalesOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("GetBySalesOrder/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _salesOrderService.GetSalesOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("CreateSalesOrder")]
        public async Task<IActionResult> Create([FromBody] SalesOrderDto salesOrderDto)
        {
            var createdOrder = await _salesOrderService.CreateSalesOrderAsync(salesOrderDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("UpdateSalesOrder/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SalesOrderDto salesOrderDto)
        {
            if (id != salesOrderDto.Id)
            {
                return BadRequest();
            }

            await _salesOrderService.UpdateSalesOrderAsync(salesOrderDto);
            return NoContent();
        }

    }
}
