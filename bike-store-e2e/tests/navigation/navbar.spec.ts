import { test, expect } from '@playwright/test';
import { BasePage } from '../../pages/base.page';

test.describe('Navbar - Logged Out', () => {
  test('brand, home, login and register links verifications', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await expect(basePage.brandLink).toContainText('Bike Store');
    await expect(basePage.homeLink).toBeVisible();
    await expect(basePage.loginButton).toBeVisible();
    await expect(basePage.registerButton).toBeVisible();
    await expect(basePage.logoutButton).toBeHidden();
  });

  test('navigate to login page', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await basePage.goToLogin();
    await expect(page).toHaveURL('/login');
  });

  test('navigate to register page', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/');

    await basePage.goToRegister();
    await expect(page).toHaveURL('/register');
  });

  test('navigate home via brand logo', async ({ page }) => {
    const basePage = new BasePage(page);
    await page.goto('/login');

    await basePage.brandLink.click();
    await expect(page).toHaveURL('/');
  });
});
