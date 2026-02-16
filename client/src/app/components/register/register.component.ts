import { Component } from '@angular/core';
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

  onSubmit() {
    this.isLoading = true;
    this.error = '';
    
    this.authService.register(this.email, this.password, this.displayName).subscribe({
      next: (response) => {
        this.authService.saveUser({ displayName: response.displayName, token: response.token });
        this.appComponent.refreshUserDisplayName();
        this.isLoading = false;
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        this.error = err.error || 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}