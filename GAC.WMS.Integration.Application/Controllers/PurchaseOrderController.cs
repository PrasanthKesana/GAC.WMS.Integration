using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Integration.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet("GetAllPurchaseOrders")]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("GetPurchaseOrder/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPost("CreatePurchaseOrder")]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderDto purchaseOrderDto)
        {

            var createdOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrderDto);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }

        [HttpPut("UpdatePurchaseOrder/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrderDto purchaseOrderDto)
        {
            if (id != purchaseOrderDto.Id)
            {
                return BadRequest();
            }

            await _purchaseOrderService.UpdatePurchaseOrderAsync(purchaseOrderDto);
            return NoContent();
        }

    }
}
