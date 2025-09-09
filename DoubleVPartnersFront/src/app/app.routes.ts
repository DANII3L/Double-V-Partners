import { Routes } from '@angular/router';
import { FacturasListComponent } from './facturas/facturas-list/facturas-list.component';
import { FacturaCreateComponent } from './facturas/facturas-create/factura-create.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'facturas' },
  { path: 'facturas', component: FacturasListComponent },
  { path: 'facturas/crear', component: FacturaCreateComponent },
  { path: '**', redirectTo: 'facturas' }
];
