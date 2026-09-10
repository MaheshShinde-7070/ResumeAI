import { ChangeDetectorRef, Component } from '@angular/core';
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
    private router: Router,
    private cdr: ChangeDetectorRef,
  ) {}

  login() {

  this.message = '';
  this.errorMessage = '';

  if (!this.email.trim()) {
    this.errorMessage = 'Email is required.';
    return;
  }

  if (!this.password.trim()) {
    this.errorMessage = 'Password is required.';
    return;
  }

  this.authService
    .login(this.email, this.password)
    .subscribe({

      next: (response) => {

        localStorage.setItem('token', response.token);

        this.message = 'Login successful!';

        this.router.navigate(['/resume-analyzer']);
      },

      error: (error) => {

        console.error(error);

        // alert('ERROR CALLBACK WORKED');

        this.errorMessage = 'Invalid email or password.';
        this.cdr.detectChanges();
      }

    });
}
}