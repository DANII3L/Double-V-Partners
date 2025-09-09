namespace DoubleVPartnersApi.Models
{
    public class DetalleFacturas_Dto 
    {
        public string ProductoNombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }
        public decimal Cantidad { get; set; }
        public string FacturaId { get; set; }
        public string Imagen { get; set; }
    }
}
