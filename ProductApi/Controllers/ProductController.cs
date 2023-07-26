using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        public readonly PRODUCTAPIContext _productApiContext;

        public ProductController(PRODUCTAPIContext productApiContext)
        {
            _productApiContext = productApiContext;
        }

        [HttpGet("list", Name = "GetAllProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts() {
            List<Product> products = new List<Product>();

            try
            {
                products = await _productApiContext.Products.Include(c => c.oCategory).ToListAsync();

                return Ok(products);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productApiContext.Products.Include(c => c.oCategory).FirstOrDefaultAsync(p => p.Id.Equals(id));

                if (product is null) throw new Exception("Product not exist");


                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("", Name = "NewProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> NewProduct([FromBody] Product product)
        {
            try
            {
                await _productApiContext.Products.AddAsync(product);

                await  _productApiContext.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("", Name = "UpdateProduct")]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] Product product)
        {
            try
            {
                var productStored = await _productApiContext.Products.Include(c => c.oCategory).FirstOrDefaultAsync(p => p.Id.Equals(product.Id));

                if (productStored is null) throw new Exception("Product not exist");


                productStored.Mark = product.Mark is null ? productStored.Mark : product.Mark;
                productStored.Description = product.Description is null ? productStored.Description : product.Description;
                productStored.Price = product.Price is null ? productStored.Price : product.Price;
                productStored.Code = product.Code is null ? productStored.Code : product.Code;
                productStored.IdCategory = product.IdCategory is null ? productStored.IdCategory : product.IdCategory;


                _productApiContext.Update(productStored);

                await _productApiContext.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("/{id:int}", Name = "DeleteProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteProduct([FromRoute] int id)
        {
            try
            {
                var productStored = await _productApiContext.Products.Include(c => c.oCategory).FirstOrDefaultAsync(p => p.Id.Equals(id));

                if (productStored is null) throw new Exception("Product not exist");


                _productApiContext.Remove(productStored);

                await _productApiContext.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
