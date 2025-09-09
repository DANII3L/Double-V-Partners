import { Injectable, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ApiService, PagedResult } from './ApiService.service';
import { Cliente } from '../types/Cliente';

@Injectable({ providedIn: 'root' })
export class ClientesService {
  constructor(private apiService: ApiService) { }

  get(): Observable<Cliente[]> {
    return this.apiService.get<Cliente>('Clientes/get').pipe(
      map((result: PagedResult<Cliente>) => {
        return result.listFind || [];
      }),
      tap({
        error: (error) => {
          console.error('Error al cargar clientes:', error);
        }
      })
    );
  }
}


