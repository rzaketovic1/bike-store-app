import { test, expect } from '../../fixtures/auth.fixture';
import { BasePage } from '../../pages/base.page';

test.describe('Logout', () => {
  test('should logout, clear auth state, and show login/register buttons', async ({ authenticatedPage }) => {
    const basePage = new BasePage(authenticatedPage);

    await authenticatedPage.goto('/');
    await expect(basePage.welcomeText).toBeVisible();

    await basePage.logout();

    await expect(authenticatedPage).toHaveURL('/');
    await expect(basePage.loginButton).toBeVisible();
    await expect(basePage.registerButton).toBeVisible();
    await expect(basePage.logoutButton).toBeHidden();

    const user = await authenticatedPage.evaluate(() => localStorage.getItem('user'));
    expect(user).toBeNull();
  });
});
