import { test, expect } from '@playwright/test';
import { LoginPage } from '../../pages/login.page';
import { createValidLoginCredentials, createUserWithInvalidPassword } from '../../data/factory';

test.describe('Login', () => {
  let loginPage: LoginPage;

  test.beforeEach(async ({ page }) => {
    loginPage = new LoginPage(page);
    await loginPage.goto();
  });

  test('positive: valid user can log in', async ({ page }) => {
    const user = createValidLoginCredentials();
    const baseUrl = String(test.info().project.use.baseURL);

    await loginPage.login(user.email, user.password);
    
    await expect(page).toHaveURL(new URL('/', baseUrl).toString());
    await expect(loginPage.logoutButton).toBeVisible();
    await expect(loginPage.loginButton).toBeHidden();
    await expect(loginPage.welcomeText).toHaveText(`Welcome, ${user.displayName}`);
  });

  test('negative: invalid password shows error and stays on login page', async ({ page }) => {
    const user = createUserWithInvalidPassword();

    await loginPage.login(user.email, user.password);

    await expect(loginPage.errorAlert).toBeVisible();
    await expect(loginPage.errorAlert).toHaveText(/invalid credentials/i);
    await expect(loginPage.notLoggedInToast).toBeVisible();
    await expect(page).toHaveURL(/\/login/);
    await expect(loginPage.loginButton).toBeVisible();
    await expect(loginPage.logoutButton).toBeHidden();
  });
});
