import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CartItem } from 'src/app/models/cart-item';
import { CartService } from 'src/app/services/cart.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent {
  cartItems$: Observable<CartItem[]> = this.cartService.cartItems$;
  cartTotal$: Observable<number> = this.cartService.getCartTotal();
  cartCount$: Observable<number> = this.cartService.getCartCount();

  constructor(private cartService: CartService, private router: Router) {}

  increment(item: CartItem): void {
    this.cartService.updateQuantity(item.product.id, item.quantity + 1);
  }

  decrement(item: CartItem): void {
    this.cartService.updateQuantity(item.product.id, item.quantity - 1);
  }

  remove(productId: number): void {
    this.cartService.removeItem(productId);
  }

  getItemTotal(item: CartItem): number {
    return this.cartService.getItemTotal(item);
  }

  getImageUrl(pictureUrl: string): string {
    if (!pictureUrl) return '';
    if (/^https?:\/\//i.test(pictureUrl)) return pictureUrl;
    const origin = environment.apiUrl.replace(/\/api\/?$/i, '');
    const path = pictureUrl.startsWith('/') ? pictureUrl : `/${pictureUrl}`;
    return `${origin}${path}`;
  }

  continueShopping(): void {
    this.router.navigateByUrl('/');
  }

  checkout(): void {
    this.router.navigateByUrl('/checkout');
  }
}
