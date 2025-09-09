using Microsoft.AspNetCore.Mvc;
using DoubleVPartnersApi.Interfaces;

namespace DoubleVPartnersApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : BaseApiController
    {
        private readonly ICliente _clienteService;

        public ClientesController(ICliente clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _clienteService.GetAsync();
                return OkResponse(result, "Clientes obtenidos correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ClientesController: {ex.Message}");
                return BadRequestResponse(ex.Message);
            }
        }
    }
}
