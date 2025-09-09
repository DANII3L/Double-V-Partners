namespace DoubleVPartnersApi.Models
{
    public class Factura_Model
    {    
        public string? Id { get; set; }
        public string? NumeroFactura { get; set; }
        public string? ClienteId { get; set; }
        public List<DetalleFacturas_Model> Detalles { get; set; } = new List<DetalleFacturas_Model>();
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string FechaEmision { get; set; }
    }
}
