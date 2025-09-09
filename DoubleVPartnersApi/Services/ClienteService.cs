using DoubleVPartnersApi.Interfaces;
using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Services
{
    public class ClienteService : ICliente
    {
        private readonly IDataService _dataService;

        public ClienteService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<PagedResult<Cliente_Model?>> GetAsync()
        {
            var result = await _dataService.EjecutarProcedimientoAsync<Cliente_Model>("SP_Clientes_Get");
            return PagedResult<Cliente_Model?>.PaginatedResponse(result);
        }
    }
}
