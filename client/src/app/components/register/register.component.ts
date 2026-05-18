import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AppComponent } from 'src/app/app.component';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  email = '';
  password = '';
  displayName = '';
  error = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router, private appComponent: AppComponent) {}

  onSubmit(form: NgForm) {
    this.error = '';

    if (form.invalid) {
      if (form.controls['email']?.errors?.['email']) {
        this.error = 'Invalid email format';
      }
      return;
    }

    this.isLoading = true;
    
    this.authService.register(this.email, this.password, this.displayName).subscribe({
      next: (response) => {
        this.authService.saveUser({ displayName: response.displayName, token: response.token });
        this.appComponent.refreshUserDisplayName();
        this.isLoading = false;
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        if (err.error?.errors) {
          const messages = Object.values(err.error.errors).flat();
          this.error = messages.join(', ');
        } else if (err.error?.message) {
          this.error = err.error.message;
        } else if (typeof err.error === 'string') {
          this.error = err.error;
        } else {
          this.error = 'Registration failed. Please try again.';
        }
        this.isLoading = false;
      }
    });
  }
}