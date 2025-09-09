using DoubleVPartnersApi.Interfaces;
using DoubleVPartnersApi.Models;

namespace DoubleVPartnersApi.Services
{
    public class FacturaService : IFactura
    {
        private readonly IDataService _dataService;

        public FacturaService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<PagedResult<Factura_Model?>> GetAsync()
        {
            var result = await _dataService.EjecutarProcedimientoAsync<Factura_Model>("SP_Facturas_Get");
            return PagedResult<Factura_Model?>.PaginatedResponse(result);
        }

        public async Task<PagedResult<Factura_Model?>> GetByNumeroFacturaAsync(string NumeroFactura)
        {
            var result = await _dataService.EjecutarProcedimientoAsync<Factura_Model>("SP_Facturas_Get", new { NumeroFactura = NumeroFactura }  );
            if (!result.IsSuccess)
                throw new Exception(result.Exception?.Message);
                
            return PagedResult<Factura_Model?>.PaginatedResponse(result);
        }

        public async Task<PagedResult<Factura_Model?>> GetByClienteIdAsync(string ClienteId)
        {
            var result = await _dataService.EjecutarProcedimientoAsync<Factura_Model>("SP_Facturas_Get", new { ClienteId = ClienteId }  );
            if (!result.IsSuccess)
                throw new Exception(result.Exception?.Message);
                
            return PagedResult<Factura_Model?>.PaginatedResponse(result);
        }

        public async Task<Factura_Dto> CreateAsync(Factura_Model factura)
        {
            if (factura.Detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto a la factura");

            var facturaDto = new Factura_Dto
            {
                NumeroFactura = factura.NumeroFactura,
                ClienteId = factura.ClienteId,
                Subtotal = factura.Subtotal,
                Impuestos = factura.Impuestos,
                Total = factura.Total,
                FechaEmision = factura.FechaEmision
            };
            var result = await _dataService.EjecutarProcedimientoAsync<Factura_Dto>("SP_Facturas_Create", facturaDto);
            if (!result.IsSuccess)
                throw new Exception(result.Exception?.Message);

            await CreateDetalleAsync(factura.Detalles, result.Data.FirstOrDefault()?.Id);
            return result.Data.FirstOrDefault();
        }

        public async Task<IEnumerable<DetalleFacturas_Dto>> GetByIdFacturaAsync(string FacturaId)
        {
            var result = await _dataService.EjecutarProcedimientoAsync<DetalleFacturas_Dto>("SP_Facturas_GetDetalles", new { FacturaId = FacturaId });
            if (!result.IsSuccess)
                throw new Exception(result.Exception?.Message);

            return result.Data;
        }

        public async Task<bool> CreateDetalleAsync(IEnumerable<DetalleFacturas_Model> detalles, string FacturaId)
        {
            foreach (var detalle in detalles)
            {
                var detalleFactura = new DetalleFacturas_Model
                {
                    FacturaId = FacturaId,
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad
                };
                await _dataService.EjecutarProcedimientoAsync<DetalleFacturas_Model>("SP_Facturas_CreateDetalle", detalleFactura);
            }
            return true;
        }
    }
}
