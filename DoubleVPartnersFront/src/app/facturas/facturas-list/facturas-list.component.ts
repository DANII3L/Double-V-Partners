import { Component, computed, signal, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { map } from 'rxjs/operators';
import { FacturasService } from '../../core/facturas.service';
import { ClientesService } from '../../core/clientes.service';
import { type Factura } from '../../types/Factura';
import { DetalleFactura, DetalleFacturaList } from '../../types/DetalleFactura';
import { Cliente } from '../../types/Cliente';
import { PagedResult } from '../../core/ApiService.service';

@Component({
  selector: 'app-facturas-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './facturas-list.component.html',
  styleUrls: ['./facturas-list.component.scss']
})
export class FacturasListComponent implements OnInit {
  protected readonly titulo = 'Facturas';
  protected readonly showFilters = signal<boolean>(false);
  protected readonly filterType = signal<'numero' | 'cliente'>('numero');
  protected readonly numeroQuery = signal<string>('');
  protected readonly clienteQuery = signal<string>('');
  protected readonly selectedClienteId = signal<string | null>(null);
  protected readonly selectedFactura = signal<Factura | null>(null);
  protected readonly selectedDetalles = signal<DetalleFacturaList[] | null>(null);
  protected readonly showModal = signal<boolean>(false);
  
  // Clientes
  private readonly clientesService = inject(ClientesService);
  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly cargandoClientes = signal<boolean>(false);
  
  // Paginación del servidor
  protected readonly pageSize = signal<number>(10);
  protected readonly currentPage = signal<number>(1);
  protected readonly totalRecords = signal<number>(0);
  protected readonly facturas = signal<Factura[]>([]);
  protected readonly loading = signal<boolean>(false);
  protected readonly error = signal<string>('');

  protected readonly totalPages = computed(() => Math.max(1, Math.ceil(this.totalRecords() / this.pageSize())));
  
  protected readonly isSpecificSearch = computed(() => {
    const type = this.filterType();
    const qNum = this.numeroQuery().trim();
    const selectedClienteId = this.selectedClienteId();
    return (type === 'numero' && qNum) || (type === 'cliente' && selectedClienteId);
  });

  ngOnInit(): void {
    this.cargarFacturas();
    this.cargarClientes();
  }

  protected goToPage(page: number): void {
    const clamped = Math.min(Math.max(1, page), this.totalPages());
    this.currentPage.set(clamped);
    
    const type = this.filterType();
    const qNum = this.numeroQuery().trim();
    const selectedClienteId = this.selectedClienteId();

    if (type === 'numero' && qNum) {
      this.buscarPorNumero(qNum);
    } else if (type === 'cliente' && selectedClienteId) {
      this.buscarPorCliente(selectedClienteId);
    } else {
      this.cargarFacturas();
    }
  }

  protected toggleFilters(): void {
    this.showFilters.set(!this.showFilters());
  }

  protected onFilterTypeChange(type: 'numero' | 'cliente'): void {
    this.filterType.set(type);
    this.numeroQuery.set('');
    this.clienteQuery.set('');
    this.selectedClienteId.set(null);
    this.currentPage.set(1);
    this.cargarFacturas();
  }

  protected buscar(): void {
    this.currentPage.set(1);
    const type = this.filterType();
    const qNum = this.numeroQuery().trim();
    const selectedClienteId = this.selectedClienteId();

    if (type === 'numero' && qNum) {
      this.buscarPorNumero(qNum);
    } else if (type === 'cliente' && selectedClienteId) {
      this.buscarPorCliente(selectedClienteId);
    } else {
      this.cargarFacturas();
    }
  }

  protected limpiar(): void {
    this.numeroQuery.set('');
    this.clienteQuery.set('');
    this.selectedClienteId.set(null);
    this.currentPage.set(1);
    this.cargarFacturas();
  }

  protected reintentar(): void {
    this.cargarFacturas();
  }

  private cargarFacturas(): void {
    this.loading.set(true);
    this.error.set('');
    
    const params: any = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize()
    };

    // Agregar filtros si existen
    const type = this.filterType();
    const qNum = this.numeroQuery().trim();
    const selectedClienteId = this.selectedClienteId();

    if (type === 'numero' && qNum) {
      params.numeroFactura = qNum;
    } else if (type === 'cliente' && selectedClienteId) {
      params.clienteId = selectedClienteId;
    }

    this.facturasSvc.get(params).subscribe({
      next: (result: PagedResult<Factura>) => {
        this.facturas.set(result.listFind || []);
        this.totalRecords.set(result.totalRecords || 0);
        this.loading.set(false);
        this.error.set('');
      },
      error: (error) => {
        this.error.set(error.message || 'Error al cargar las facturas');
        this.facturas.set([]);
        this.totalRecords.set(0);
        this.loading.set(false);
      }
    });
  }

  constructor(private facturasSvc: FacturasService) {}

  protected openDetalleModal(f: Factura): void {
    this.selectedFactura.set(f);
    this.facturasSvc.getDetalles({ id: f.id }).subscribe({
      next: (detalles) => {
        this.selectedDetalles.set(detalles);
        this.showModal.set(true);
      },
      error: (error) => {
        console.error('Error al cargar detalles:', error);
      }
    });
  }

  protected closeDetalleModal(): void {
    this.showModal.set(false);
    this.selectedFactura.set(null);
    this.selectedDetalles.set(null);
  }

  private cargarClientes(): void {
    this.cargandoClientes.set(true);
    this.clientesService.get().subscribe({
      next: (clientes) => {
        this.clientes.set(clientes);
        this.cargandoClientes.set(false);
      },
      error: (error) => {
        console.error('Error al cargar clientes:', error);
        this.cargandoClientes.set(false);
      }
    });
  }

  protected onClienteChange(clienteId: string | null): void {
    this.selectedClienteId.set(clienteId);
    this.currentPage.set(1);
    if (clienteId) {
      this.buscarPorCliente(clienteId);
    } else {
      this.cargarFacturas();
    }
  }

  private buscarPorNumero(numero: string): void {
    this.loading.set(true);
    this.error.set('');
    
    this.facturasSvc.getByNumeroFactura(numero).subscribe({
      next: (factura) => {
        if (factura) {
          this.facturas.set(factura.listFind || []);
          this.totalRecords.set(factura.totalRecords || 0);
        } else {
          this.facturas.set([]);
          this.totalRecords.set(0);
        }
        this.loading.set(false);
        this.error.set('');
      },
      error: (error) => {
        console.error('Error al buscar factura por número:', error);
        this.error.set(error.message || 'Error al buscar la factura');
        this.facturas.set([]);
        this.totalRecords.set(0);
        this.loading.set(false);
      }
    });
  }

  private buscarPorCliente(clienteId: string): void {
    this.loading.set(true);
    this.error.set('');
    
    this.facturasSvc.getByClienteId(clienteId).subscribe({
      next: (factura) => {
        if (factura) {
          this.facturas.set(factura.listFind || []);
          this.totalRecords.set(1);
        } else {
          this.facturas.set([]);
          this.totalRecords.set(0);
        }
        this.loading.set(false);
        this.error.set('');
      },
      error: (error) => {
        console.error('Error al buscar facturas por cliente:', error);
        this.error.set(error.message || 'Error al buscar las facturas del cliente');
        this.facturas.set([]);
        this.totalRecords.set(0);
        this.loading.set(false);
      }
    });
  }
}


