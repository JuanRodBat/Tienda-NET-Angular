import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { TiendasService } from '../../core/tiendas';

@Component({
  standalone: true,
  selector: 'app-tiendas',
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule],
  templateUrl: "./tiendas.html",
  styleUrl: './tiendas.css'
})
export class TiendasComponent implements OnInit {
  tiendas:any[]=[];
  constructor(private svc: TiendasService, private router: Router) {}
  ngOnInit(){ this.svc.getAll().subscribe(r => this.tiendas = r); }
  open(id:number){ this.router.navigate(['/tiendas', id]); }
}