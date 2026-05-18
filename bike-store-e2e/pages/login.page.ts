import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class LoginPage extends BasePage {
  readonly loginForm: Locator;
  readonly emailInput: Locator;
  readonly passwordInput: Locator;
  readonly submitButton: Locator;
  readonly errorAlert: Locator;
  readonly notLoggedInToast: Locator;
  readonly registerLink: Locator;
  readonly heading: Locator;

  constructor(page: Page) {
    super(page);
    this.loginForm = page.locator('form').first();
    this.emailInput = page.getByLabel(/email address/i);
    this.passwordInput = page.getByLabel(/password/i);
    this.submitButton = this.loginForm.getByRole('button', { name: /sign in|signing in/i });
    // Scope alert to the login form to avoid matching global toast alerts.
    this.errorAlert = this.loginForm.getByRole('alert');
    this.notLoggedInToast = page.getByRole('alert', { name: /you are not logged in/i });
    this.registerLink = page.getByRole('link', { name: /register here/i });
    this.heading = page.getByRole('heading', { name: /welcome back/i });
  }

  async goto() {
    await this.page.goto('/login');
  }

  async login(email: string, password: string) {
    await this.emailInput.fill(email);
    await this.passwordInput.fill(password);
    await this.submitButton.click();
  }
}
