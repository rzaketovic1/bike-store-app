import { test, expect } from '../../fixtures/auth.fixture';
import { ProductListPage } from '../../pages/product-list.page';
import { cleanupProduct } from '../../fixtures/test-data.fixture';
import path from 'path';

test.describe('Product Create', () => {
  let createdProductId: number | undefined;

  test.afterEach(async ({ authToken }) => {
    if (createdProductId) {
      await cleanupProduct(authToken, createdProductId);
      createdProductId = undefined;
    }
  });

  test('should create a product with all fields', async ({ authenticatedPage }) => {
    const productList = new ProductListPage(authenticatedPage);
    await productList.goto();

    await productList.addProductButton.click();
    await expect(productList.createModal).toBeVisible();

    const productName = `Playwright Created Bike ${Date.now()}`;
    const productDescription = `Created via Playwright E2E test ${Date.now()}`;

    // Fill form
    await productList.createNameInput.fill(productName);
    await productList.createDescriptionInput.fill(productDescription);
    await productList.createPriceInput.fill('299.99');
    await productList.createQuantityInput.fill('5');

    // Select brand and type from the dropdowns
    const [selectedBrand] = await productList.createBrandSelect.selectOption({ index: 1 });
    const [selectedType] = await productList.createTypeSelect.selectOption({ index: 1 });

    // Upload a test image
    const testImagePath = path.resolve(__dirname, '..', '..', 'fixtures', 'test-image.png');
    await productList.createImageInput.setInputFiles(testImagePath);

    // Submit
    const createResponsePromise = authenticatedPage.waitForResponse((response) =>
      response.url().includes('/api/Products') &&
      response.request().method() === 'POST' &&
      response.ok()
    );
    await productList.createSubmitButton.click();

    const createResponse = await createResponsePromise;
    const createdProduct = await createResponse.json();
    createdProductId = createdProduct.id;

    // Expect success toastr and modal closes
    await expect(authenticatedPage.locator('.toast-success')).toBeVisible();
    await expect(productList.createModal).not.toBeVisible();

    // Filter by selected brand and type, then verify the created product is visible
    await productList.filterByBrand(selectedBrand);
    await productList.filterByType(selectedType);

    const createdProductCard = productList.createdProductCard(productName, productDescription);

    await expect(createdProductCard).toBeVisible();
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
