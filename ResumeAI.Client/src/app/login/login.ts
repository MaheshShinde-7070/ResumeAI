import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule,CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {

  email = '';
  password = '';

  message = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  login() {

  this.message = '';
  this.errorMessage = '';

  this.authService
    .login(this.email, this.password)
    .subscribe({

      next: (response) => {

        console.log(response);

        localStorage.setItem('token', response.token);

        this.message = 'Login successful!';

        this.router.navigate(['/resume-analyzer']);
      },

      error: (error) => {

        console.error(error);

        this.errorMessage = 'Invalid email or password.';

      }

    });
}
}