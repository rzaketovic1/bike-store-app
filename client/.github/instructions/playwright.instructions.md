---
applyTo: "tests/**/*.ts,e2e/**/*.ts,playwright.config.ts,playwright.config.js"
description: "Use when writing or debugging Playwright E2E tests for the Bike Store app."
---

# Playwright E2E Instructions

## Project Context

Playwright tests verify the Bike Store app end-to-end. The Angular frontend runs at `http://localhost:4200` and communicates with the .NET API at `http://localhost:5000/api/`. Tests should reflect real user behavior — not implementation details.

## Mentor Approach

Before writing any Playwright test:
1. Explain what E2E testing is verifying and why it matters for this feature
2. Identify the user flow being tested (actions, assertions, edge cases)
3. Explain any Playwright concept used (locators, waiting strategies, fixtures, page objects)

After writing a test:
- Explain what the test covers and what it would catch if broken
- Explain how to run it and interpret the results
- Point out common E2E testing pitfalls for that scenario
- Ask one review question

## Core Principles

- **Test behavior, not implementation**: interact the way a user would (click buttons, fill forms, read text)
- **Avoid brittle selectors**: prefer `getByRole`, `getByLabel`, `getByText` over CSS selectors or XPath
- **One concern per test**: each `test()` should verify one user-facing outcome
- **Tests must be independent**: no test should depend on state left by another

## Preferred Locator Strategy (in priority order)

1. `getByRole('button', { name: 'Add to Cart' })` — most resilient
2. `getByLabel('Email')` — for form fields
3. `getByText('Login')` — for visible text
4. `getByTestId('product-card')` — when semantic locators aren't available; add `data-testid` attributes to the Angular template
5. CSS selectors (`.product-card`) — last resort only

## Test Structure

```typescript
import { test, expect } from '@playwright/test';

test.describe('Feature: Product List', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:4200');
  });

  test('should display products on the home page', async ({ page }) => {
    // Arrange — already done in beforeEach

    // Act — perform user actions
    // (none needed for this test)

    // Assert — verify visible outcome
    await expect(page.getByRole('heading', { name: 'Products' })).toBeVisible();
  });
});
```

## Waiting Strategy

- **Do not use `page.waitForTimeout()`** — it makes tests slow and flaky
- Use auto-waiting locators: `await expect(locator).toBeVisible()` waits automatically
- Wait for network: `await page.waitForResponse('**/api/Products**')` when needed
- Wait for navigation: `await page.waitForURL('**/product/**')` after a route change

## Authentication in Tests

For tests requiring a logged-in user, use `storageState` to reuse a session rather than logging in every test:

```typescript
// In a global setup file
const page = await browser.newPage();
await page.goto('http://localhost:4200/login');
await page.getByLabel('Email').fill('test@example.com');
await page.getByLabel('Password').fill('Password123!');
await page.getByRole('button', { name: 'Login' }).click();
await page.context().storageState({ path: 'tests/.auth/user.json' });
```

```typescript
// In playwright.config.ts
use: { storageState: 'tests/.auth/user.json' }
```

## Page Object Model

For complex flows, extract reusable actions into a Page Object:

```typescript
// tests/pages/LoginPage.ts
export class LoginPage {
  constructor(private page: Page) {}

  async login(email: string, password: string) {
    await this.page.goto('http://localhost:4200/login');
    await this.page.getByLabel('Email').fill(email);
    await this.page.getByLabel('Password').fill(password);
    await this.page.getByRole('button', { name: 'Login' }).click();
  }
}
```

Only introduce Page Objects when a flow is used in **multiple** test files.

## Running Tests

```bash
# Run all tests
npx playwright test

# Run a specific file
npx playwright test tests/product-list.spec.ts

# Run with UI mode (great for debugging)
npx playwright test --ui

# Show the HTML report
npx playwright show-report
```

Ensure the Angular dev server (`npm start`) and the .NET API are running before executing tests.

## Common Mistakes to Watch For

- Using `page.waitForTimeout()` instead of auto-waiting assertions
- Selecting by CSS class that changes frequently (`.btn-primary`) instead of role/label
- Testing internal state (component variables) instead of what the user sees
- Making tests dependent on each other's data (use `beforeEach` to reset)
- Not handling async properly — always `await` Playwright actions and assertions
- Hardcoding URLs — consider a `baseURL` in `playwright.config.ts`
