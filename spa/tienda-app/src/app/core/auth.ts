import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private base = environment.apiBase;
  token: string | null = localStorage.getItem('jwt');

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string) {
    return this.http.post<any>(`${this.base}/auth/login`, { email, password })
      .pipe(tap(res => {
        this.token = res.token;
        localStorage.setItem('jwt', res.token);
      }));
  }

  logout() {
    this.token = null;
    localStorage.removeItem('jwt');
    this.router.navigateByUrl('/login');
  }

  isLoggedIn() { return !!this.token; }
}
