import { Page, Locator } from '@playwright/test';

export class BasePage {
  readonly navbar: Locator;
  readonly brandLink: Locator;
  readonly homeLink: Locator;
  readonly loginButton: Locator;
  readonly registerButton: Locator;
  readonly logoutButton: Locator;
  readonly welcomeText: Locator;

  constructor(protected page: Page) {
    this.navbar = page.getByTestId('main-navbar');
    this.brandLink = this.navbar.getByRole('link', { name: /bike store/i });
    this.homeLink = this.navbar.getByRole('link', { name: /home/i });
    this.loginButton = this.navbar.getByRole('button', { name: /login/i });
    this.registerButton = this.navbar.getByRole('button', { name: /register/i });
    this.logoutButton = this.navbar.getByRole('button', { name: /logout/i });
    this.welcomeText = this.navbar.getByText(/welcome,/i);
  }

  async navigateHome() {
    await this.homeLink.click();
  }

  async goToLogin() {
    await this.loginButton.click();
  }

  async goToRegister() {
    await this.registerButton.click();
  }

  async logout() {
    await this.logoutButton.click();
  }

  async getWelcomeDisplayName(): Promise<string> {
    const text = await this.welcomeText.textContent();
    // "Welcome, John" → "John"
    return text?.replace(/Welcome,\s*/i, '').trim() || '';
  }
}
