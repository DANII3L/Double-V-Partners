import { Injectable, signal } from '@angular/core';
import { Producto } from '../types/Producto';
import { ApiService, PagedResult } from './ApiService.service';
import { map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductosService {

  constructor(private apiService: ApiService) { }
  
  get(): Observable<Producto[]> {
    return this.apiService.get<Producto>('Productos/get').pipe(
      map((result: PagedResult<Producto>) => {
        return result.listFind || [];
      }),
      tap({
        error: (error) => {
          console.error('Error al cargar productos:', error);
        }
      })
    );
  }
}


