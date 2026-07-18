import { faker } from '@faker-js/faker';

export interface LoginCredentials {
  email: string;
  password: string;
  displayName?: string;
}

export function createValidLoginCredentials(): LoginCredentials {
  return {
    email: process.env.TEST_USER_EMAIL || 'e2e-test@bikestore.com',
    password: process.env.TEST_USER_PASSWORD || 'Test1234!',
    displayName: process.env.TEST_USER_DISPLAY_NAME || 'E2E Tester',
  };
}

export function createUserWithInvalidPassword(): LoginCredentials {
  const validEmail = process.env.TEST_USER_EMAIL || 'e2e-test@bikestore.com';

  return {
    // Keep valid email but generate a random wrong password to exercise auth failure.
    email: validEmail,
    password: `${faker.string.alphanumeric(8)}!Aa1`,
  };
}
