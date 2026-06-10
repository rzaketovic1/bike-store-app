import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CartItem } from '../models/cart-item';
import { Product } from '../models/product';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly storageKey = 'cart';

  private cartItemsSubject = new BehaviorSubject<CartItem[]>(this.loadFromStorage());

  cartItems$: Observable<CartItem[]> = this.cartItemsSubject.asObservable();

  // --- Derived observables for reactive UI ---

  getCartCount(): Observable<number> {
    return this.cartItems$.pipe(
      map(items => items.reduce((sum, item) => sum + item.quantity, 0))
    );
  }

  getCartTotal(): Observable<number> {
    return this.cartItems$.pipe(
      map(items => items.reduce((sum, item) => sum + item.product.price * item.quantity, 0))
    );
  }

  // --- Synchronous snapshot ---

  getItems(): CartItem[] {
    return this.cartItemsSubject.getValue();
  }

  isInCart(productId: number): boolean {
    return this.getItems().some(item => item.product.id === productId);
  }

  getItemTotal(item: CartItem): number {
    return item.product.price * item.quantity;
  }

  // --- Mutations ---

  addItem(product: Product, quantity = 1): void {
    const items = this.getItems().map(i => ({ ...i }));
    const existing = items.find(i => i.product.id === product.id);

    if (existing) {
      existing.quantity = Math.min(existing.quantity + quantity, product.quantityInStock);
    } else {
      items.push({ product, quantity: Math.min(quantity, product.quantityInStock) });
    }

    this.persist(items);
  }

  removeItem(productId: number): void {
    this.persist(this.getItems().filter(i => i.product.id !== productId));
  }

  updateQuantity(productId: number, quantity: number): void {
    const items = this.getItems().map(item => {
      if (item.product.id !== productId) return item;
      return { ...item, quantity: Math.max(1, Math.min(quantity, item.product.quantityInStock)) };
    });
    this.persist(items);
  }

  clearCart(): void {
    this.persist([]);
  }

  // --- Private helpers ---

  private loadFromStorage(): CartItem[] {
    try {
      const data = localStorage.getItem(this.storageKey);
      return data ? JSON.parse(data) : [];
    } catch {
      return [];
    }
  }

  private persist(items: CartItem[]): void {
    localStorage.setItem(this.storageKey, JSON.stringify(items));
    this.cartItemsSubject.next(items);
  }
}
