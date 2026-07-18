import { test, expect } from '@playwright/test';
import { BasePage } from '../../pages/base.page';

test.describe('Navbar - Logged Out', () => {
  test('should display brand, home, login and register links', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await expect(basePage.brandLink).toContainText('Bike Store');
    await expect(basePage.homeLink).toBeVisible();
    await expect(basePage.loginButton).toBeVisible();
    await expect(basePage.registerButton).toBeVisible();
    await expect(basePage.logoutButton).toBeHidden();
  });

  test('should navigate to login page', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await basePage.goToLogin();
    await expect(page).toHaveURL('/login');
  });

  test('should navigate to register page', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await basePage.goToRegister();
    await expect(page).toHaveURL('/register');
  });

  test('should navigate to home via brand logo', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/login');

    await basePage.brandLink.click();
    await expect(page).toHaveURL('/');
  });
});
