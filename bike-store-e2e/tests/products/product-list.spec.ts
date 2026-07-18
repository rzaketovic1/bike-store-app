import { test, expect } from '@playwright/test';
import { ProductListPage } from '../../pages/product-list.page';

test.describe('Product List', () => {
  test('should display product list', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    await expect(productList.pageHeading).toContainText('Explore Our Collection');
    await expect(productList.pageSubtitle).toContainText('Find your perfect ride from our premium selection of bikes');
    await expect(productList.productCards.first()).toBeVisible();
    await expect(productList.productNames.first()).toBeVisible();
  });

  test('should filter products by brand', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const randomBrand = await productList.getRandomBrand();
    if (!randomBrand) return;

    await productList.filterByBrand(randomBrand);
    await expect(productList.productCards.first()).toBeVisible();
    await productList.expectAllCardsHaveBrand(randomBrand);
  });

  test('should filter products by type', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const randomType = await productList.getRandomType();
    if (!randomType) return;

    await productList.filterByType(randomType );
    await expect(productList.productCards.first()).toBeVisible();
    await productList.expectAllCardsHaveType(randomType);
  });

  test('should sort products by price', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    await productList.sortBy('priceAsc');
    const ascPrices = await productList.getProductPrices();
    expect(ascPrices.length).toBeGreaterThan(1);
    expect(isAscending(ascPrices)).toBe(true);

    await productList.sortBy('priceDesc');
    const descPrices = await productList.getProductPrices();
    expect(descPrices.length).toBeGreaterThan(1);
    expect(isDescending(descPrices)).toBe(true);
  });

  test('should navigate through product pagination', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const paginationVisible = await productList.paginationNav.isVisible().catch(() => false);
    if (!paginationVisible) {
      // Skip test if pagination is not available
      return;
    }

    const pageTwoButton = productList.paginationPageButton(2);
    const hasSecondPage = await pageTwoButton.isVisible().catch(() => false);
    if (!hasSecondPage) {
      // Skip when there is only one page of products.
      return;
    }

    const firstPageNames = await productList.getProductNames();
    expect(firstPageNames.length).toBeGreaterThan(0);

    const activePage = productList.paginationNav.locator('li.page-item.active > a.page-link');
    await expect(activePage).toHaveText('1');

    // Navigate directly to page 2 to avoid flaky next-button behavior.
    await pageTwoButton.click();
    await expect(activePage).toHaveText('2');
    await expect(productList.productCards.first()).toBeVisible();

    const secondPageNames = await productList.getProductNames();
    expect(secondPageNames.length).toBeGreaterThan(0);

    // Navigate to specific page number
    const pageButton = productList.paginationPageButton(1);
    await pageButton.click();
    await expect(activePage).toHaveText('1');
    await expect(productList.productCards.first()).toBeVisible();

    const backToFirstPageNames = await productList.getProductNames();
    expect(backToFirstPageNames.length).toBeGreaterThan(0);
  });

  function isAscending(values: number[]): boolean {
    for (let i = 1; i < values.length; i++) {
      if (values[i] < values[i - 1]) return false;
    }
    return true;
  }

  function isDescending(values: number[]): boolean {
    for (let i = 1; i < values.length; i++) {
      if (values[i] > values[i - 1]) return false;
    }
    return true;
  }
});

