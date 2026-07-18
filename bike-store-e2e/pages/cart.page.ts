import { Page, Locator, expect } from '@playwright/test';
import { BasePage } from './base.page';

export class CartPage extends BasePage {
  readonly heading: Locator;
  readonly cartTable: Locator;
  readonly cartItemRows: Locator;
  readonly cartSummary: Locator;
  readonly cartSubtotal: Locator;
  readonly cartTotalItems: Locator;
  readonly checkoutButton: Locator;
  readonly emptyCartMessage: Locator;

  constructor(page: Page) {
    super(page);
    this.heading = page.getByTestId('cart-heading');
    this.cartTable = page.getByTestId('cart-table');
    this.cartItemRows = page.getByTestId('cart-item-row');
    this.cartSummary = page.getByTestId('cart-summary');
    this.cartSubtotal = page.getByTestId('cart-subtotal');
    this.cartTotalItems = page.getByTestId('cart-total-items');
    this.checkoutButton = page.getByTestId('checkout-btn');
    this.emptyCartMessage = page.getByText('Your cart is empty', { exact: false });
  }

  async goto() {
    await this.page.goto('/cart');
  }

  cartItemName(index: number): Locator {
    return this.cartItemRows.nth(index).getByTestId('cart-item-name');
  }

  cartRemoveButton(index: number): Locator {
    return this.cartItemRows.nth(index).getByTestId('cart-remove-btn');
  }

  async getSubtotal(): Promise<number> {
    const text = await this.cartSubtotal.textContent();
    return parseFloat((text ?? '').replace(/[^0-9.]/g, ''));
  }

  async proceedToCheckout() {
    await this.checkoutButton.click();
  }
}
