export type Factura = {
    id: string;
    numeroFactura: string;
    fechaEmision: string; // ISO
    clienteId: string | null;
    subtotal: number;
    impuestos: number;
    total: number;
  };