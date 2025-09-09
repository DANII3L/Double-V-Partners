using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Interfaces
{
    public interface IProducto
    {
        Task<PagedResult<Producto_Model?>> GetAsync();
        Task<Producto_Model> CreateAsync(Producto_Model producto);
    }
}
