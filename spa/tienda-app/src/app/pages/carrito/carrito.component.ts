import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { CarritoService } from '../../core/carrito';

@Component({
  standalone: true,
  selector: 'app-carrito',
  imports: [CommonModule, CurrencyPipe, MatCardModule, MatButtonModule, MatDividerModule, MatSnackBarModule],
  templateUrl: "./carrito.html",
  styleUrl: './carrito.css'
})
export class CarritoComponent implements OnInit {
  items:any[]=[]; total=0;
  constructor(private cart: CarritoService, private snack: MatSnackBar){}
  ngOnInit(){ this.refresh(); }
  refresh(){
    this.cart.items().subscribe(r => {
      this.items = r;
      this.total = r.reduce((s,x)=> s + (x.articulo.precio * x.cantidad), 0);
    });
  }
  clear(){ this.cart.clear().subscribe(()=>{ this.snack.open('Carrito limpio','OK',{duration:1500}); this.refresh(); }); }
  remove(a:number,t:number){ this.cart.remove(a,t).subscribe(()=>{ this.snack.open('Artículo eliminado','OK',{duration:1500}); this.refresh(); }); }
  checkout(){ this.cart.checkout().subscribe(()=>{ this.snack.open('Compra realizada','OK',{duration:1500}); this.refresh(); }); }
}