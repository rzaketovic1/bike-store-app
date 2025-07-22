import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { Router } from '@angular/router';
import { AppComponent } from 'src/app/app.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

    constructor(
    private authService: AuthService, 
    private router: Router, 
    private appComponent: AppComponent // dodaj ovo
  ) {}

  onSubmit() {
    this.authService.login(this.email, this.password).subscribe({
      next: (response) => {
        this.authService.saveUser({ displayName: response.displayName, token: response.token });
        this.appComponent.refreshUserDisplayName(); // OVO JE KLJUČ!
        this.router.navigateByUrl('/');
      },
      error: (err) => {
        this.error = err.error || 'Login failed';
      }
    });
  }
}