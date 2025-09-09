using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Interfaces
{
    public interface IFactura
    {
        Task<PagedResult<Factura_Model?>> GetAsync();
        Task<IEnumerable<DetalleFacturas_Dto>> GetByIdFacturaAsync(string FacturaId);
        Task<Factura_Dto> CreateAsync(Factura_Model factura);
        Task<PagedResult<Factura_Model?>> GetByNumeroFacturaAsync(string NumeroFactura);
        Task<PagedResult<Factura_Model?>> GetByClienteIdAsync(string ClienteId);
    }
}
