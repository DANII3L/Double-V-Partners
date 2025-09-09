import { Injectable } from '@angular/core';
import { Factura } from '../types/Factura';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ApiService, PagedResult } from './ApiService.service';
import { DetalleFactura, DetalleFacturaList } from '../types/DetalleFactura';

@Injectable({ providedIn: 'root' })
export class FacturasService {
  constructor(private apiService: ApiService) { }

  get(params?: any): Observable<PagedResult<Factura>> {
    return this.apiService.get<Factura>('Facturas/get', params).pipe(
      map((result: PagedResult<Factura>) => {
        return result;
      }),
      tap({
        error: (error) => {
          console.error('Error al cargar productos:', error);
        }
      })
    );
  }

  getDetalles(params?: any): Observable<DetalleFacturaList[]> {
    return this.apiService.getOne<DetalleFacturaList>('Facturas/GetDetallesByIdFactura/' + params.id).pipe(
      map((result: any) => {
        return result || [];
      }),
      tap({
        error: (error) => {
          console.error('Error al cargar productos:', error);
        }
      })
    );
  }

  create(factura: Factura, detalles: any): Observable<Factura> {
    var facturaPayload = {
      ...factura,
      detalles: detalles
    };

    return this.apiService.post<Factura>('Facturas/create', facturaPayload).pipe(
      map((result: Factura) => {
        return result;
      }),
      tap({
        error: (error) => {
          console.error('Error al crear factura:', error.message);
        }
      })
    );
  }

  getByNumeroFactura(numeroFactura: string): Observable<PagedResult<Factura>> {
    return this.apiService.get<Factura>(`Facturas/GetByNumeroFactura/${ numeroFactura }`).pipe(
      map((result: PagedResult<Factura>) => {
        return result;
      }),
      tap({
        error: (error) => { 
          console.error('Error al obtener factura por número:', error.message);
        }
      })
    );
  }

  getByClienteId(clienteId: string): Observable<PagedResult<Factura>> {
    return this.apiService.get<Factura>(`Facturas/GetByClienteId/${ clienteId }`).pipe(
      map((result: PagedResult<Factura>) => {
        return result;
      }),
      tap({
        error: (error) => {
          console.error('Error al obtener factura por número:', error.message);
        }
      })
    );
  }
}


