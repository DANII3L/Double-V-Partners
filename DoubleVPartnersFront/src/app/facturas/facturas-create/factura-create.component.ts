import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators, FormGroup, FormArray } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ClientesService } from '../../core/clientes.service';
import { Cliente } from '../../types/Cliente';
import { ProductosService } from '../../core/productos.service';
import { FacturasService } from '../../core/facturas.service';
import { Producto } from '../../types/Producto';
import { DetalleFactura } from '../../types/DetalleFactura';

@Component({
  selector: 'app-factura-create',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink],
  templateUrl: './factura-create.component.html',
  styleUrls: ['./factura-create.component.scss']
})
export class FacturaCreateComponent implements OnInit {
  protected readonly titulo = 'Crear factura';

  protected form!: FormGroup;
  protected detalles!: FormArray;

  private readonly clientesService = inject(ClientesService);
  private readonly productosService = inject(ProductosService);

  protected readonly clientes = signal<Cliente[]>([]);
  protected readonly productos = signal<Producto[]>([]);
  protected readonly cargandoClientes = signal<boolean>(false);
  protected readonly cargandoProductos = signal<boolean>(false);

  // Estados de carga y éxito
  protected readonly loading = signal<boolean>(false);
  protected readonly success = signal<boolean>(false);
  protected readonly error = signal<string>('');
  protected readonly numeroError = signal<string>('');
  protected readonly validandoNumero = signal<boolean>(false);

  // Disparador para reactividad de computed sobre valores de formularios
  private readonly totalsVersion = signal(0);

  protected readonly subtotal = computed(() => {
    // Dependencia explícita para reactividad
    this.totalsVersion();
    return this.detalles.controls
      .reduce((acc, ctrl: any) => acc + (Number(ctrl.get('total')?.value) || 0), 0);
  });
  protected readonly impuestos = computed(() => Math.round(this.subtotal() * 0.19 * 100) / 100);
  protected readonly totalCalc = computed(() => Math.round((this.subtotal() + this.impuestos()) * 100) / 100);

  constructor(private fb: FormBuilder, private router: Router, private facturasSvc: FacturasService) {
    this.detalles = this.fb.array([]);
    this.form = this.fb.group({
      numero: ['', [Validators.required, Validators.minLength(3)]],
      fecha: ['', [Validators.required]],
      clienteId: [null as string | null, [Validators.required]],
      detalles: this.detalles
    });

    // Cálculos automáticos por cambio de cantidad o selección de producto
    this.detalles.valueChanges.subscribe(() => {
      this.recomputeTotals();
    });
  }

  ngOnInit(): void {
    // Cargar clientes desde la API
    this.cargandoClientes.set(true);
    this.clientesService.get().subscribe({
      next: (clientes) => {
        console.log('Clientes cargados:', clientes);
        this.clientes.set(clientes);
        this.cargandoClientes.set(false);
      },
      error: (error) => {
        console.error('Error al cargar clientes:', error);
        this.cargandoClientes.set(false);
      }
    });
    
    // Cargar productos desde la API
    this.cargandoProductos.set(true);
    this.productosService.get().subscribe({
      next: (productos) => {
        console.log('Productos cargados:', productos);
        this.productos.set(productos);
        this.cargandoProductos.set(false);
      },
      error: (error) => {
        console.error('Error al cargar productos:', error);
        this.cargandoProductos.set(false);
      }
    });
  }

  protected nuevo(): void {
    this.form.reset();
    while (this.detalles.length) this.detalles.removeAt(0);
    this.limpiarEstados();
  }

  private limpiarEstados(): void {
    this.loading.set(false);
    this.success.set(false);
    this.error.set('');
    this.numeroError.set('');
    this.validandoNumero.set(false);
  }

  protected validarNumeroFactura(): void {
    const numero = this.form.get('numero')?.value?.trim();
    
    if (!numero || numero.length < 3) {
      this.numeroError.set('');
      this.validandoNumero.set(false);
      return;
    }

    this.validandoNumero.set(true);
    this.numeroError.set('');

    // Verificar si el número ya existe
    this.facturasSvc.getByNumeroFactura(numero).subscribe({
      next: (facturaExistente) => {
        this.validandoNumero.set(false);
        if (facturaExistente.listFind.length > 0) {
          this.numeroError.set('Este número de factura ya existe');
        } else {
          this.numeroError.set('');
        }
      },
      error: (error) => {
        this.validandoNumero.set(false);
        if (error.status === 404) {
          // No existe, está disponible
          this.numeroError.set('');
        } else {
          this.numeroError.set('Error al verificar el número de factura');
        }
      }
    });
  }

  protected agregarProducto(): void {
    const grupo = this.fb.group({
      productoId: [null as string | null, [Validators.required]],
      nombreProducto: [{ value: '', disabled: true }],
      precioUnitario: [{ value: 0, disabled: true }],
      imagenUrl: [{ value: '', disabled: true }],
      cantidad: [1, [Validators.required, Validators.min(1)]],
      total: [{ value: 0, disabled: true }]
    });


    grupo.get('productoId')!.valueChanges.subscribe((id: string | null) => {
      const prod = this.productos().find(p => p.id === id);
      grupo.get('nombreProducto')!.setValue(prod?.nombre ?? '');
      grupo.get('precioUnitario')!.setValue(prod?.precio ?? 0);
      grupo.get('imagenUrl')!.setValue(prod?.imagen ?? '');
      this.recalcularLinea(grupo);
    });

    grupo.get('cantidad')!.valueChanges.subscribe(() => this.recalcularLinea(grupo));

    this.detalles.push(grupo);
    this.recomputeTotals();
  }

  private recalcularLinea(grupo: FormGroup): void {
    const precio = Number(grupo.get('precioUnitario')!.value) || 0;
    const cantidad = Number(grupo.get('cantidad')!.value) || 0;
    const total = Math.round(precio * cantidad * 100) / 100;
    grupo.get('total')!.setValue(total, { emitEvent: false });
    this.recomputeTotals();
  }

  private recomputeTotals(): void {
    this.totalsVersion.set(this.totalsVersion() + 1);
  }

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.loading()) {
      return; // Prevenir múltiples envíos
    }

    // Verificar si hay error en el número de factura
    if (this.numeroError()) {
      this.error.set('Por favor corrija el número de factura antes de continuar');
      return;
    }

    this.loading.set(true);
    this.error.set('');
    this.success.set(false);

    const numero = String(this.form.get('numero')!.value || '').trim();
    this.crearFactura(numero);
  }

  private crearFactura(numero: string): void {
    const payload = {
      numeroFactura: numero,
      fechaEmision: this.form.get('fecha')!.value,
      clienteId: this.form.get('clienteId')!.value,
      subtotal: this.subtotal(),
      impuestos: this.impuestos(),
      total: this.totalCalc()
    };

    const detalles = this.detalles.getRawValue().map((detalle: any) => ({
      FacturaId: '',
      ProductoId: detalle.productoId,
      Cantidad: detalle.cantidad
    }));
    
    this.facturasSvc.create(payload as any, detalles).subscribe({
      next: (facturaCreada) => {
        this.loading.set(false);
        this.success.set(true);
        this.error.set('');
        
        // Mostrar mensaje de éxito y navegar después de 2 segundos
        setTimeout(() => {
          this.router.navigateByUrl('/facturas');
        }, 2000);
      },
      error: (error) => {
        this.loading.set(false);
        this.error.set('Error al crear la factura: ' + (error.message || 'Error desconocido'));
        this.success.set(false);
      }
    });
  }

  protected get numero() {
    return this.form.get('numero');
  }

  protected get fecha() { return this.form.get('fecha'); }

  protected get clienteId() { return this.form.get('clienteId'); }

  protected get total() { return this.totalCalc(); }
}


