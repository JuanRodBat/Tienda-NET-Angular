import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { TiendasComponent } from './pages/tiendas/tiendas.component';
import { CatalogoComponent } from './pages/catalogo/catalogo.component';
import { CarritoComponent } from './pages/carrito/carrito.component';
import { authGuard } from './core/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'tiendas', component: TiendasComponent, canActivate: [authGuard] },
  { path: 'tiendas/:id', component: CatalogoComponent, canActivate: [authGuard] },
  { path: 'carrito', component: CarritoComponent, canActivate: [authGuard] },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
