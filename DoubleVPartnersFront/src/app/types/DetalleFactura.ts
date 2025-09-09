export type DetalleFactura = {
    Id: number | null;
    FacturaId?: string;
    ProductoId?: string;
    precioUnitario?: number;
    Subtotal?: number;
  };

  export type DetalleFacturaList = {
    facturaId?: string; 
    cantidad?: number;
    productoNombre?: string;
    precioUnitario?: number;
    total?: number;
    imagen?: string;
  };