import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { CartItem } from 'src/app/models/cart-item';
import { CartService } from 'src/app/services/cart.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit {
  checkoutForm!: FormGroup;
  cartItems$: Observable<CartItem[]> = this.cartService.cartItems$;
  cartTotal$: Observable<number> = this.cartService.getCartTotal();
  cartCount$: Observable<number> = this.cartService.getCartCount();

  readonly taxRate = 0.1;
  readonly shipping = 0;

  constructor(
    private fb: FormBuilder,
    private cartService: CartService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.checkoutForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      address: ['', [Validators.required]]
    });

    // If cart is empty, redirect back to /cart
    const items = this.cartService.getItems();
    if (items.length === 0) {
      this.router.navigateByUrl('/cart');
    }
  }

  get firstName() { return this.checkoutForm.get('firstName'); }
  get lastName()  { return this.checkoutForm.get('lastName'); }
  get email()     { return this.checkoutForm.get('email'); }
  get address()   { return this.checkoutForm.get('address'); }

  isInvalid(field: ReturnType<FormGroup['get']>): boolean {
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getTax(subtotal: number): number {
    return subtotal * this.taxRate;
  }

  getTotal(subtotal: number): number {
    return subtotal + this.getTax(subtotal) + this.shipping;
  }

  getItemTotal(item: CartItem): number {
    return this.cartService.getItemTotal(item);
  }

  getImageUrl(pictureUrl: string): string {
    if (!pictureUrl) return '';
    if (pictureUrl.startsWith('http')) return pictureUrl;
    return `${environment.apiUrl.replace('/api/', '')}${pictureUrl}`;
  }

  onCancel(): void {
    this.router.navigateByUrl('/cart');
  }

  onSubmit(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.cartService.clearCart();
    this.router.navigateByUrl('/checkout/complete');
  }
}
