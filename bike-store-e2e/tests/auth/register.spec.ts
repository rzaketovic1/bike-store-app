import { test, expect } from '@playwright/test';
import { RegisterPage } from '../../pages/register.page';

test.describe('Register', () => {
  let registerPage: RegisterPage;

  test.beforeEach(async ({ page }) => {
    registerPage = new RegisterPage(page);
    await registerPage.goto();
  });

  test('user can register successfully', async ({ page }) => {
    await expect(registerPage.heading).toBeVisible();
    await expect(registerPage.displayNameInput).toBeVisible();
    await expect(registerPage.emailInput).toBeVisible();
    await expect(registerPage.passwordInput).toBeVisible();
    await expect(registerPage.submitButton).toBeVisible();

    const uniqueEmail = `e2e-${Date.now()}@bikestore.com`;

    await registerPage.register('New User', uniqueEmail, 'Test1234!');

    await expect(page).toHaveURL('/');
    await expect(registerPage.welcomeText).toContainText('New User');
    await expect(registerPage.logoutButton).toBeVisible();
  });

    test('verify link to login page', async ({ page }) => {
    await registerPage.loginLink.click();
    await expect(page).toHaveURL('/login');
  });

  test('submit button disabled when form is empty', async () => {
    await expect(registerPage.submitButton).toBeDisabled();
  });

  test('error for duplicate email', async () => {
    const TEST_EMAIL = process.env.TEST_USER_EMAIL || 'e2e-test@bikestore.com';

    await registerPage.register('Duplicate', TEST_EMAIL, 'Test1234!');

    await expect(registerPage.errorAlert).toBeVisible();
    await expect(registerPage.errorAlert).toContainText(/Email already in use/i);
  });

  test('validation error for short password', async () => {
    await registerPage.displayNameInput.fill('Test');
    await registerPage.emailInput.fill('test@test.com');
    await registerPage.passwordInput.fill('12');
    await registerPage.passwordInput.blur();

    await expect(registerPage.passwordMinLengthError).toBeVisible();
  });

  test('validation error when display name is missing', async () => {
    await registerPage.displayNameInput.fill('');
    await registerPage.emailInput.fill('valid@email.com');
    await registerPage.passwordInput.fill('Test1234!');
    await registerPage.displayNameInput.blur();

    await expect(registerPage.displayNameRequiredError).toBeVisible();
    await expect(registerPage.submitButton).toBeDisabled();
  });

  test('validation error for invalid email format', async () => {
    await registerPage.displayNameInput.fill('Test User');
    await registerPage.emailInput.fill('invalid-email');
    await registerPage.passwordInput.fill('Test1234!');
    await expect(registerPage.submitButton).toBeEnabled();
    await registerPage.submitButton.click();

    await expect(registerPage.errorAlert).toBeVisible();
    await expect(registerPage.errorAlert).toContainText(/Invalid email format/i);
  });
});
