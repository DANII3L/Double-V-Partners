using Microsoft.AspNetCore.Mvc;
using DoubleVPartnersApi.Interfaces;
using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacturasController : BaseApiController
    {
        private readonly IFactura _facturaService;

        public FacturasController(IFactura facturaService)
        {
            _facturaService = facturaService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _facturaService.GetAsync();
                return OkResponse(result, "Facturas obtenidas correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FacturasController: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpGet("GetDetallesByIdFactura/{id}")]
        public async Task<IActionResult> GetDetallesByIdFactura(string id)
        {
            try
            {
                var result = await _facturaService.GetByIdFacturaAsync(id);
                if (result == null)
                    return NotFoundResponse("Factura no encontrada");

                return OkResponse(result, "Factura obtenida correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FacturasController GetById: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Factura_Model factura)
        {
            try
            {
                if (factura is null)
                    return BadRequestResponse("model is null");

                var result = await _facturaService.CreateAsync(factura);
                return OkResponse(result, "Factura creada correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FacturasController Create: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpGet("GetByNumeroFactura/{numeroFactura}")]
        public async Task<IActionResult> GetByNumeroFactura(string numeroFactura)
        {
            try
            {
                if (string.IsNullOrEmpty(numeroFactura))
                    return BadRequestResponse("El número de factura es requerido");

                var result = await _facturaService.GetByNumeroFacturaAsync(numeroFactura);
                if (result.ListFind.Count() == 0)
                    return NotFoundResponse("Factura no encontrada");
                    
                return OkResponse(result, "Factura obtenida correctamente");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }

        [HttpGet("GetByClienteId/{clienteId}")]
        public async Task<IActionResult> GetByClienteId(string clienteId)
        {
            try
            {
                var result = await _facturaService.GetByClienteIdAsync(clienteId);
                if (result.ListFind.Count() == 0)
                    return NotFoundResponse("Factura no encontrada");

                return OkResponse(result, "Factura obtenida correctamente");
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }
    }
}
