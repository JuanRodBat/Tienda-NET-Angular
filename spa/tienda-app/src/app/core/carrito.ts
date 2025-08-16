import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CarritoService {
  private base = environment.apiBase;
  constructor(private http: HttpClient) {}
  add(articuloId: number, tiendaId: number, cantidad: number) {
    return this.http.post(`${this.base}/carrito/add`, { articuloId, tiendaId, cantidad });
  }
  items() { return this.http.get<any[]>(`${this.base}/carrito/items`); }
  checkout() { return this.http.post(`${this.base}/carrito/checkout`, {}); }
  clear() { return this.http.delete(`${this.base}/carrito/clear`); }
  remove(articuloId: number, tiendaId: number) {
    return this.http.request('DELETE', `${this.base}/carrito/item`, {
      body: { articuloId, tiendaId }
  });
}
}

