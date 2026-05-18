import { test, expect } from '../../fixtures/auth.fixture';
import { ProductListPage } from '../../pages/product-list.page';
import path from 'path';

test.describe('Product Create', () => {
  test('should open and close create product modal', async ({ authenticatedPage }) => {
    const productList = new ProductListPage(authenticatedPage);
    await productList.goto();

    await productList.addProductButton.click();
    await expect(productList.createModal).toBeVisible();

    await productList.createCloseButton.click();
    await expect(productList.createModal).not.toBeVisible();
  });

  test('should create a product with all fields', async ({ authenticatedPage }) => {
    const productList = new ProductListPage(authenticatedPage);
    await productList.goto();

    await productList.addProductButton.click();
    await expect(productList.createModal).toBeVisible();

    // Fill form
    await productList.createNameInput.fill('Playwright Created Bike');
    await productList.createDescriptionInput.fill('Created via Playwright E2E test');
    await productList.createPriceInput.fill('299.99');
    await productList.createQuantityInput.fill('5');

    // Select brand and type from the dropdowns
    await productList.createBrandSelect.selectOption({ index: 1 });
    await productList.createTypeSelect.selectOption({ index: 1 });

    // Upload a test image
    const testImagePath = path.join(__dirname, '..', '..', 'fixtures', 'test-image.png');
    await productList.createImageInput.setInputFiles(testImagePath);

    // Submit
    await productList.createSubmitButton.click();

    // Expect success toastr and modal closes
    await expect(authenticatedPage.locator('.toast-success')).toBeVisible();
    await expect(productList.createModal).not.toBeVisible();
  });

  test('should show alert when no image is selected', async ({ authenticatedPage }) => {
    const productList = new ProductListPage(authenticatedPage);
    await productList.goto();

    await productList.addProductButton.click();

    await productList.createNameInput.fill('No Image Bike');
    await productList.createDescriptionInput.fill('Missing image');
    await productList.createPriceInput.fill('100');
    await productList.createQuantityInput.fill('1');
    await productList.createBrandSelect.selectOption({ index: 1 });
    await productList.createTypeSelect.selectOption({ index: 1 });

    // Listen for dialog (alert)
    authenticatedPage.once('dialog', async (dialog) => {
      expect(dialog.message()).toContain('Please select an image');
      await dialog.accept();
    });

    await productList.createSubmitButton.click();
  });
});
