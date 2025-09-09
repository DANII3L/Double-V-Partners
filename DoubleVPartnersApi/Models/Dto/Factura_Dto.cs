namespace DoubleVPartnersApi.Models
{
    public class Factura_Dto
    {    
        public string? Id { get; set; }
        public string? NumeroFactura { get; set; }
        public string? ClienteId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Impuestos { get; set; }
        public decimal Total { get; set; }
        public string FechaEmision { get; set; }
        public string? ClienteNombre { get; set; }
    }
}
