import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import { CartService } from './services/cart.service';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'client';
  userDisplayName: string | null = null;
  cartCount$: Observable<number>;

  constructor(public authService: AuthService, private router: Router, private cartService: CartService) {
    this.cartCount$ = this.cartService.getCartCount();
  }

  ngOnInit() {
    this.refreshUserDisplayName();
  }

  refreshUserDisplayName() {
    this.userDisplayName = this.authService.getDisplayName();
  }

  logout() {
    this.authService.logout();
    this.refreshUserDisplayName();
    this.router.navigateByUrl('/');
  }
}