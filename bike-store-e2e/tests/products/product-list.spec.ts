import { test, expect } from '@playwright/test';
import { ProductListPage } from '../../pages/product-list.page';

test.describe('Product List', () => {
  test('user can see product list', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    await expect(productList.pageHeading).toContainText('Explore Our Collection');
    await expect(productList.pageSubtitle).toContainText('Find your perfect ride from our premium selection of bikes');
    await expect(productList.productCards.first()).toBeVisible();
    await expect(productList.productNames.first()).toBeVisible();
  });

  test('user can filter products by brand', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const randomBrand = await productList.getRandomBrand();
    if (!randomBrand) return;

    await productList.filterByBrand(randomBrand);
    await expect(productList.productCards.first()).toBeVisible();
    await productList.expectAllCardsHaveBrand(randomBrand);
  });

  test('user can filter products by type', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const randomType = await productList.getRandomType();
    if (!randomType) return;

    await productList.filterByType(randomType );
    await expect(productList.productCards.first()).toBeVisible();
    await productList.expectAllCardsHaveType(randomType);
  });

  test('user can sort products by price', async ({ page }) => {
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

  test('user can navigate through product pagination', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    const paginationVisible = await productList.paginationNav.isVisible().catch(() => false);
    if (!paginationVisible) {
      // Skip test if pagination is not available
      return;
    }

    const firstPageNames = await productList.getProductNames();
    expect(firstPageNames.length).toBeGreaterThan(0);

    // Navigate to next page
    await productList.nextPage.click();
    await expect(productList.productCards.first()).toBeVisible();

    const secondPageNames = await productList.getProductNames();
    // Pages should show different products (unless very few products)
    if (secondPageNames.length > 0) {
      expect(secondPageNames).not.toEqual(firstPageNames);
    }

    // Navigate to specific page number
    const pageButton = productList.paginationPageButton(1);
    await pageButton.click();
    await expect(productList.productCards.first()).toBeVisible();

    const backToFirstPageNames = await productList.getProductNames();
    expect(backToFirstPageNames).toEqual(firstPageNames);
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

