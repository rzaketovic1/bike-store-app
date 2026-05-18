import { test as setup } from '@playwright/test';
import { registerUser } from '../helpers/api.helper';

const TEST_EMAIL = process.env.TEST_USER_EMAIL || 'e2e-test@bikestore.com';
const TEST_PASSWORD = process.env.TEST_USER_PASSWORD || 'Test1234!';
const TEST_DISPLAY_NAME = process.env.TEST_USER_DISPLAY_NAME || 'E2E Tester';

setup('ensure test user exists', async () => {
  await registerUser({
    email: TEST_EMAIL,
    password: TEST_PASSWORD,
    displayName: TEST_DISPLAY_NAME,
  });
});
