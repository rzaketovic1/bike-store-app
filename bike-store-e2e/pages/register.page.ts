import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class RegisterPage extends BasePage {
  readonly displayNameInput: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly submitButton: Locator;
  readonly errorAlert: Locator;
  readonly displayNameRequiredError: Locator;
  readonly invalidEmailError: Locator;
  readonly passwordMinLengthError: Locator;
  readonly loginLink: Locator;
  readonly heading: Locator;

  constructor(page: Page) {
    super(page);
    this.displayNameInput = page.getByLabel('Display Name');
    this.emailInput = page.getByLabel('Email Address');
    this.passwordInput = page.getByLabel('Password');
    this.submitButton = page.getByRole('button', { name: /create account/i });
    this.errorAlert = page.getByTestId('register-form-error');
    this.displayNameRequiredError = page.getByTestId('register-display-name-error');
    this.invalidEmailError = page.getByTestId('register-email-error');
    this.passwordMinLengthError = page.getByTestId('register-password-error');
    this.loginLink = page.getByRole('link', { name: /sign in here/i });
    this.heading = page.getByRole('heading', { level: 2, name: /create account/i });
  }

  async goto() {
    await this.page.goto('/register');
  }

  async register(displayName: string, email: string, password: string) {
    await this.displayNameInput.fill(displayName);
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.submitButton.click();
  }
}
