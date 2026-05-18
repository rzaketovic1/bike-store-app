import { Page } from '@playwright/test';

/**
 * Injects auth state into localStorage so the app treats the user as logged in.
 * Must be called before navigating to a page.
 */
export async function injectAuthState(
  page: Page,
  user: { displayName: string; token: string }
): Promise<void> {
  await page.addInitScript((userData) => {
    localStorage.setItem('user', JSON.stringify(userData));
  }, user);
}

/**
 * Clears auth state from localStorage.
 */
export async function clearAuthState(page: Page): Promise<void> {
  await page.addInitScript(() => {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  });
}
