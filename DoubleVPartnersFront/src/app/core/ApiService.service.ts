import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  listFind: T[];
  totalRecords: number;
  pageNumber: number;
  pageSize: number;
}

export interface ApiError {
  message: string;
  status: number;
  details?: any;
}

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly baseUrl = 'http://localhost:5000/api';
  private readonly defaultHeaders = new HttpHeaders({
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  });

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: any): Observable<PagedResult<T>> {
    const url = `${this.baseUrl}/${endpoint}`;
    
    return this.http.get<ApiResponse<PagedResult<T>>>(url, {
      headers: this.defaultHeaders,
      params: params
    }).pipe(
      map(response => {
        if (!response.success) {
          throw new Error(response.message || 'Error en la respuesta del servidor');
        }
        return response.data;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  getOne<T>(endpoint: string, params?: any): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    return this.http.get<ApiResponse<T>>(url, {
      headers: this.defaultHeaders,
      params: params
    }).pipe(
      map(response => {
        return response.data;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    const url = `${this.baseUrl}/${endpoint}`;
    
    return this.http.post<ApiResponse<T>>(url, body, {
      headers: this.defaultHeaders
    }).pipe(
      map(response => {
        if (!response.success) {
          throw new Error(response.message || 'Error en la respuesta del servidor');
        }
        return response.data;
      }),
      catchError(this.handleError.bind(this))
    );
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'Ha ocurrido un error inesperado';
    let status = 0;

    if (error.error instanceof ErrorEvent) {
      // Error del lado del cliente
      errorMessage = `Error del cliente: ${error.error.message}`;
    } else {
      // Error del lado del servidor
      status = error.status;
      
      if (error.error && typeof error.error === 'object') {
        // Si el servidor devuelve un objeto de error estructurado
        if (error.error.message) {
          errorMessage = error.error.message;
        } else if (error.error.error) {
          errorMessage = error.error.error;
        }
      } else if (typeof error.error === 'string') {
        // Si el servidor devuelve un mensaje de error como string
        errorMessage = error.error;
      }

      // Mensajes específicos por código de estado
      switch (error.status) {
        case 400:
          errorMessage = errorMessage || 'Solicitud incorrecta. Verifique los datos enviados.';
          break;
        case 401:
          errorMessage = 'No autorizado. Debe iniciar sesión.';
          break;
        case 403:
          errorMessage = 'Acceso denegado. No tiene permisos para realizar esta acción.';
          break;
        case 404:
          errorMessage = 'Recurso no encontrado.';
          break;
        case 409:
          errorMessage = 'Conflicto. El recurso ya existe o está en uso.';
          break;
        case 422:
          errorMessage = errorMessage || 'Datos de validación incorrectos.';
          break;
        case 500:
          errorMessage = 'Error interno del servidor. Intente nuevamente más tarde.';
          break;
        case 503:
          errorMessage = 'Servicio no disponible temporalmente.';
          break;
        default:
          errorMessage = errorMessage || `Error del servidor (${error.status})`;
      }
    }

    const apiError: ApiError = {
      message: errorMessage,
      status: status,
      details: error.error
    };

    return throwError(() => apiError);
  }
}
