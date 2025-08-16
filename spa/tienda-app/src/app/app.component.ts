import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet, RouterLink, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule }  from '@angular/material/button';
import { MatIconModule }    from '@angular/material/icon';
import { MatBadgeModule }   from '@angular/material/badge';
import { AuthService } from './core/auth';
import { CarritoService } from './core/carrito';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule, MatBadgeModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent implements OnInit {
  cartCount = 0;
  constructor(private router: Router, private auth: AuthService, private cart: CarritoService) {}
  ngOnInit() {
    const refresh = () => this.cart.items().subscribe(items => this.cartCount = items.length);
    refresh();
    this.router.events.pipe(filter(e => e instanceof NavigationEnd)).subscribe(() => refresh());
  }
  logout(){ this.auth.logout(); }
}
