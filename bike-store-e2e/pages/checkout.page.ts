import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class CheckoutPage extends BasePage {
  readonly heading: Locator;
  readonly orderSummary: Locator;
  readonly checkoutItemRows: Locator;
  readonly firstNameInput: Locator;
  readonly lastNameInput: Locator;
  readonly emailInput: Locator;
  readonly addressInput: Locator;
  readonly finishOrderButton: Locator;
  readonly total: Locator;

  constructor(page: Page) {
    super(page);
    this.heading = page.getByTestId('checkout-heading');
    this.orderSummary = page.getByTestId('checkout-order-summary');
    this.checkoutItemRows = page.getByTestId('checkout-item-row');
    this.firstNameInput = page.getByTestId('checkout-first-name');
    this.lastNameInput = page.getByTestId('checkout-last-name');
    this.emailInput = page.getByTestId('checkout-email');
    this.addressInput = page.getByTestId('checkout-address');
    this.finishOrderButton = page.getByTestId('finish-order-btn');
    this.total = page.getByTestId('checkout-total');
  }

  async fillCustomerInfo(data: {
    firstName: string;
    lastName: string;
    email: string;
    address: string;
  }) {
    await this.firstNameInput.fill(data.firstName);
    await this.lastNameInput.fill(data.lastName);
    await this.emailInput.fill(data.email);
    await this.addressInput.fill(data.address);
  }

  async getTotal(): Promise<number> {
    const text = await this.total.textContent();
    return parseFloat((text ?? '').replace(/[^0-9.]/g, ''));
  }

  async finishOrder() {
    await this.finishOrderButton.click();
  }
}
