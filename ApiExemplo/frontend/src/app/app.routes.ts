import { Routes } from '@angular/router';
import { ComponenteCascaApp } from './casca-app/casca-app.component';
import { PaginaCheckoutClienteComponent } from './pagina-checkout-cliente/pagina-checkout-cliente.component';

export const routes: Routes = [
  { path: '', redirectTo: 'escaner', pathMatch: 'full' },
  { path: 'escaner', component: ComponenteCascaApp },
  { path: 'checkout', component: PaginaCheckoutClienteComponent },
];
