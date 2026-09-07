import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../services/auth';
import { Router,RouterLink } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule,RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {

  name = '';
  email = '';
  password = '';

  message = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  register() {

    this.message = '';
    this.errorMessage = '';

    this.authService
      .register(this.name, this.email, this.password)
      .subscribe({

        next: (response) => {

          console.log(response);

          this.message = 'Registration successful!';

          // Go to Login page
          this.router.navigate(['/login']);

        },

        error: (error) => {

          console.error(error);

          this.errorMessage = 'Registration failed.';

        }

      });
  }
}