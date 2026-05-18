import { test, expect } from '@playwright/test';
import { ProductListPage } from '../../pages/product-list.page';

test.describe('Product List', () => {
  let productList: ProductListPage;

  test.beforeEach(async ({ page }) => {
    productList = new ProductListPage(page);
    await productList.goto();
  });

  test('should display the page heading and product cards', async () => {
    await expect(productList.pageHeading).toContainText('Explore Our Collection');
    await expect(productList.pageSubtitle).toContainText('Find your perfect ride from our premium selection of bikes');
  });

  test('should filter products by brand', async () => {
    const specificBrand = await productList.getFirstSpecificBrand();
    if (!specificBrand) return;

    await productList.filterByBrand(specificBrand);

    await productList.expectAllCardsHaveBrand(specificBrand);
  });

  test('should filter products by type', async () => {
    const specificType = await productList.getFirstSpecificType();
    if (!specificType) return;

    await productList.filterByType(specificType);

    await productList.expectAllCardsHaveType(specificType);
  });

  test('should reset filters when "All" is clicked', async () => {
    // Apply a brand filter first
    const brandButtons = productList.brandFilterSection.locator('button');
    await brandButtons.first().waitFor();
    const brands = await brandButtons.allTextContents();
    const specificBrand = brands.find((b) => b.trim() !== 'All');
    if (!specificBrand) return;

    await productList.filterByBrand(specificBrand.trim());
    const filteredCount = await productList.getProductCount();

    await productList.filterByBrand('All');
    const allCount = await productList.getProductCount();

    expect(allCount).toBeGreaterThanOrEqual(filteredCount);
  });

  test('should sort by price ascending', async () => {
    await productList.sortBy('priceAsc');

    const prices = await productList.getProductPrices();
    for (let i = 1; i < prices.length; i++) {
      expect(prices[i]).toBeGreaterThanOrEqual(prices[i - 1]);
    }
  });

  test('should sort by price descending', async () => {
    await productList.sortBy('priceDesc');

    const prices = await productList.getProductPrices();
    for (let i = 1; i < prices.length; i++) {
      expect(prices[i]).toBeLessThanOrEqual(prices[i - 1]);
    }
  });

  test('should navigate between pages', async () => {
    // Only run if pagination exists
    const paginationVisible = await productList.paginationNav.isVisible();
    if (!paginationVisible) return;

    const firstPageNames = await productList.getProductNames();

    await productList.nextPage.click();

    const secondPageNames = await productList.getProductNames();
    // Pages should show different products (unless very few products)
    if (secondPageNames.length > 0) {
      expect(secondPageNames).not.toEqual(firstPageNames);
    }
  });

  test('should navigate to product details on card click', async ({ page }) => {
    await productList.productCards.first().waitFor();
    await productList.clickProduct(0);

    await expect(page).toHaveURL(/\/product\/\d+/);
  });
});
