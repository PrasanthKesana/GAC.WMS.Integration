using GAC.WMS.Integration.Application.Interfaces;
using GAC.WMS.Integration.Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GAC.WMS.Integration.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("GetProduct/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var product = await _productService.GetProductByCodeAsync(code);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("CreateProduct")]
        public async Task<IActionResult> Create([FromBody] ProductDto product)
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetByCode), new { code = createdProduct.Code }, createdProduct);
        }

        [HttpPut("UpdateProduct/{code}")]
        public async Task<IActionResult> Update(string code, [FromBody] ProductDto productDto)
        {
            if (code != productDto.ProductCode)
            {
                return BadRequest();
            }

            await _productService.UpdateProductAsync(productDto);
            return NoContent();
        }

        [HttpDelete("DeleteProduct/{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            await _productService.DeleteProductAsync(code);
            return NoContent();
        }
    }
}
