import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'client';
  userDisplayName: string | null = null;

  constructor(public authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.refreshUserDisplayName();
    console.log('Ime korisnika:', this.userDisplayName);
  }

  refreshUserDisplayName() {
    this.userDisplayName = this.authService.getDisplayName();
  }

  logout() {
    this.authService.logout();
    this.refreshUserDisplayName();
    this.router.navigateByUrl('/');
  }

  onPost() {
  // Ovdje implementiraj POST logiku ili navigaciju
  console.log('POST clicked');
}

onPut() {
  // Ovdje implementiraj PUT logiku ili navigaciju
  console.log('PUT clicked');
}

onDelete() {
  // Ovdje implementiraj DELETE logiku ili navigaciju
  console.log('DELETE clicked');
}
}