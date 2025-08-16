import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ArticulosService } from '../../core/articulos';
import { CarritoService } from '../../core/carrito';

@Component({
  standalone: true,
  selector: 'app-catalogo',
  imports: [CommonModule, CurrencyPipe, MatCardModule, MatButtonModule, MatSnackBarModule],
  templateUrl: "./catalogo.html",
  styleUrl: './catalogo.css'
})
export class CatalogoComponent implements OnInit {
  tiendaId!: number; items:any[]=[];
  constructor(private route: ActivatedRoute, private art: ArticulosService, private cart: CarritoService, private snack: MatSnackBar) {}
  ngOnInit(){
    this.tiendaId = Number(this.route.snapshot.paramMap.get('id'));
    this.art.getCatalogoByTienda(this.tiendaId).subscribe(r=> this.items = r);
  }
  add(articuloId:number){
    this.cart.add(articuloId, this.tiendaId, 1).subscribe({
      next: ()=> this.snack.open('Agregado al carrito','OK',{duration:2000}),
      error: e => this.snack.open(e?.error ?? 'Error al agregar','Cerrar',{duration:3000})
    });
  }
}
