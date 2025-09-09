using DoubleVPartnersApi.Interfaces;
using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Services
{
    public class ProductoService : IProducto
    {
        private readonly IDataService _dataService;

        public ProductoService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<PagedResult<Producto_Model?>> GetAsync()
        {
            var result = await _dataService.EjecutarProcedimientoAsync<Producto_Model>("SP_Productos_Get");
            return PagedResult<Producto_Model?>.PaginatedResponse(result);
        }

        public async Task<Producto_Model> CreateAsync(Producto_Model producto)
        {
            await _dataService.EjecutarProcedimientoAsync<Producto_Model>("SP_Productos_Create", producto);
            return producto;
        }
    }
}
