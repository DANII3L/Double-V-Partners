using Microsoft.AspNetCore.Mvc;
using DoubleVPartnersApi.Interfaces;
using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : BaseApiController
    {
        private readonly IProducto _productoService;

        public ProductosController(IProducto productoService)
        {
            _productoService = productoService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _productoService.GetAsync();
                return OkResponse(result, "Productos obtenidos correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProductosController: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Producto_Model producto)
        {
            try
            {
                var result = await _productoService.CreateAsync(producto);
                return OkResponse(result, "Producto creado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ProductosController Create: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }
    }
}
