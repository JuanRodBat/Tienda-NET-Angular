import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth';
import { Router } from '@angular/router';

// Angular Material
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [CommonModule, FormsModule, MatSnackBarModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatCardModule],
  templateUrl: "./login.html",
  styleUrl: './login.css'
})
export class LoginComponent {
  email = 'demo@example.com'; 
  password = 'P@ssw0rd!';

  constructor(private auth: AuthService, private router: Router, private snack: MatSnackBar) {}
  submit() {
    this.auth.login(this.email, this.password).subscribe({
      next: () => this.router.navigateByUrl('/tiendas'),
      error: (err) => {
        const msg = err?.status === 401
          ? 'Usuario y/o contraseña incorrectos'
          : 'Error al iniciar sesión';
        this.snack.open(msg, 'Cerrar', { duration: 3000 });
      }
    });
  }
}