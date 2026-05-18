import { test as base, Page } from '@playwright/test';
import { registerUser } from '../helpers/api.helper';
import { injectAuthState } from '../helpers/storage.helper';

const TEST_EMAIL = process.env.TEST_USER_EMAIL || 'e2e-test@bikestore.com';
const TEST_PASSWORD = process.env.TEST_USER_PASSWORD || 'Test1234!';
const TEST_DISPLAY_NAME = process.env.TEST_USER_DISPLAY_NAME || 'E2E Tester';

type AuthFixtures = {
  authenticatedPage: Page;
  authToken: string;
};

export const test = base.extend<AuthFixtures>({
  authToken: async ({}, use) => {
    const user = await registerUser({
      email: TEST_EMAIL,
      password: TEST_PASSWORD,
      displayName: TEST_DISPLAY_NAME,
    });
    await use(user.token);
  },

  authenticatedPage: async ({ page, authToken }, use) => {
    await injectAuthState(page, {
      displayName: TEST_DISPLAY_NAME,
      token: authToken,
    });
    await use(page);
  },
});

export { expect } from '@playwright/test';
