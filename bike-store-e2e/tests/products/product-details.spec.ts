import { test, expect } from '../../fixtures/auth.fixture';
import { ProductDetailsPage } from '../../pages/product-details.page';
import { seedProduct, cleanupProduct } from '../../fixtures/test-data.fixture';

test.describe('Product Details', () => {
  let productId: number;

  test.beforeEach(async ({ authToken }) => {
    const product = await seedProduct(authToken);
    productId = product.id;
  });

  test.afterEach(async ({ authToken }) => {
    await cleanupProduct(authToken, productId);
  });

  test('should display product details', async ({ authenticatedPage }) => {
    const detailsPage = new ProductDetailsPage(authenticatedPage);
    await detailsPage.goto(productId);

    await expect(detailsPage.productName).toContainText('E2E Test Bike');
    await expect(detailsPage.productPrice).toBeVisible();
    await expect(detailsPage.productBrand).toContainText('Trek');
    await expect(detailsPage.productType).toContainText('Mountain');
    await expect(detailsPage.productStock).toContainText('10');
    await expect(detailsPage.productImage).toBeVisible();
    await expect(detailsPage.editButton).toBeVisible();
    await expect(detailsPage.deleteButton).toBeVisible();
  });

  test('should edit product via modal', async ({ authenticatedPage }) => {
    const detailsPage = new ProductDetailsPage(authenticatedPage);
    await detailsPage.goto(productId);

    await detailsPage.openEditModal();
    await detailsPage.editProduct({
      name: 'Updated E2E Bike',
      price: 599.99,
    });

    // Toastr success
    await expect(authenticatedPage.locator('.toast-success')).toBeVisible();
    // Verify updated name
    await expect(detailsPage.productName).toContainText('Updated E2E Bike');
  });

  test('should delete product', async ({ authenticatedPage, authToken }) => {
    const detailsPage = new ProductDetailsPage(authenticatedPage);
    await detailsPage.goto(productId);

    await detailsPage.deleteProduct();

    // Should redirect to home after delete
    await expect(authenticatedPage).toHaveURL('/');

    // Mark as cleaned up so afterEach doesn't fail
    productId = -1;
  });
});
