using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Interfaces
{
    public interface ICliente
    {
        Task<PagedResult<Cliente_Model?>> GetAsync();
    }
}
