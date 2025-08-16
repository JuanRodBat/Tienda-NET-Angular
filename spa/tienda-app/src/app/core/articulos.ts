import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ArticulosService {
  private base = environment.apiBase;
  constructor(private http: HttpClient) {}
  getCatalogoByTienda(tiendaId: number) {
    return this.http.get<any[]>(`${this.base}/tiendas/${tiendaId}/articulos`);
  }
}
