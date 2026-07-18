import { Page, Locator, expect } from '@playwright/test';
import { BasePage } from './base.page';

export class ProductListPage extends BasePage {
  readonly pageHeading: Locator;
  readonly pageSubtitle: Locator;
  readonly productCards: Locator;
  readonly productNames: Locator;
  readonly productPrices: Locator;
  readonly addProductButton: Locator;
  readonly sortDropdown: Locator;
  readonly emptyState: Locator;

  // Create product modal
  readonly createModal: Locator;
  readonly createNameInput: Locator;
  readonly createDescriptionInput: Locator;
  readonly createPriceInput: Locator;
  readonly createBrandSelect: Locator;
  readonly createTypeSelect: Locator;
  readonly createQuantityInput: Locator;
  readonly createImageInput: Locator;
  readonly createSubmitButton: Locator;
  readonly createCloseButton: Locator;

  // Filters
  readonly brandFilterSection: Locator;
  readonly typeFilterSection: Locator;

  // Pagination
  readonly paginationNav: Locator;
  readonly previousPage: Locator;
  readonly nextPage: Locator;

  constructor(page: Page) {
    super(page);
    this.pageHeading = page.getByTestId('products-heading');
    this.pageSubtitle = page.getByTestId('products-subtitle');
    this.productCards = page.getByTestId('product-card');
    this.productNames = page.getByTestId('product-name');
    this.productPrices = page.getByTestId('product-price');
    this.addProductButton = page.getByTestId('add-product-button');
    this.sortDropdown = page.getByLabel('Sort by:');
    this.emptyState = page.getByTestId('products-empty-state');

    // Create product modal
    this.createModal = page.locator('#createProductModal');
    this.createNameInput = this.createModal.getByLabel('Name');
    this.createDescriptionInput = this.createModal.getByLabel('Description');
    this.createPriceInput = this.createModal.getByLabel('Price');
    this.createBrandSelect = this.createModal.getByLabel('Brand');
    this.createTypeSelect = this.createModal.getByLabel('Type');
    this.createQuantityInput = this.createModal.getByLabel('Quantity In Stock');
    this.createImageInput = this.createModal.getByLabel('Select Image');
    this.createSubmitButton = this.createModal.getByTestId('create-modal-submit');
    this.createCloseButton = this.createModal.getByTestId('create-modal-close');

    // Filters
    this.brandFilterSection = page.getByTestId('brand-filter-section');
    this.typeFilterSection = page.getByTestId('type-filter-section');

    // Pagination
    this.paginationNav = page.getByTestId('products-pagination');
    this.previousPage = this.paginationNav.getByText('Previous', { exact: true });
    this.nextPage = this.paginationNav.getByText('Next', { exact: true });
  }

  async goto() {
    await this.page.goto('/');
  }

  brandFilterButton(brand: string): Locator {
    return this.brandFilterSection.getByRole('button', { name: brand });
  }

  typeFilterButton(type: string): Locator {
    return this.typeFilterSection.getByRole('button', { name: type });
  }

  async filterByBrand(brand: string) {
    await this.brandFilterButton(brand).click();
    // Wait for first brand badge to appear and be stable
    const badges = this.page.getByTestId('product-brand-badge');
    await badges.first().waitFor({ state: 'visible' });
    // Give the UI a moment to render all filtered results
    await this.page.waitForTimeout(500);
  }

  async filterByType(type: string) {
    await this.typeFilterButton(type).click();
    // Wait for first type badge to appear and be stable
    const badges = this.page.getByTestId('product-type-badge');
    await badges.first().waitFor({ state: 'visible' });
    // Give the UI a moment to render all filtered results
    await this.page.waitForTimeout(500);
  }

  async getRandomBrand(): Promise<string | undefined> {
    const buttons = this.brandFilterSection.getByRole('button');
    await buttons.first().waitFor();
    const labels = await buttons.allTextContents();
    const specificBrands = labels.map((b) => b.trim()).filter((b) => b !== 'All');
    return specificBrands.length > 0
      ? specificBrands[Math.floor(Math.random() * specificBrands.length)]
      : undefined;
  }

  async getRandomType(): Promise<string | undefined> {
    const buttons = this.typeFilterSection.getByRole('button');
    await buttons.first().waitFor();
    const labels = await buttons.allTextContents();
    const specificTypes = labels.map((t) => t.trim()).filter((t) => t !== 'All');
    return specificTypes.length > 0
      ? specificTypes[Math.floor(Math.random() * specificTypes.length)]
      : undefined;
  }

  async expectAllCardsHaveBrand(brand: string) {
    // Get all brand badges (which are within product cards)
    const badges = this.page.getByTestId('product-brand-badge');
    await badges.first().waitFor({ state: 'visible' });
    
    const count = await badges.count();
    for (let i = 0; i < count; i++) {
      await expect(badges.nth(i)).toContainText(brand);
    }
  }

  async expectAllCardsHaveType(type: string) {
    // Get all type badges (which are within product cards)
    const badges = this.page.getByTestId('product-type-badge');
    await badges.first().waitFor({ state: 'visible' });
    
    const count = await badges.count();
    for (let i = 0; i < count; i++) {
      await expect(badges.nth(i)).toContainText(type);
    }
  }

  async sortBy(value: string) {
    await this.sortDropdown.selectOption(value);
    await expect(this.sortDropdown).toHaveValue(value);
    // Wait for product cards to update after sort
    await this.productCards.first().waitFor({ state: 'visible' });
    await this.page.waitForLoadState('networkidle');

    if (value === 'priceAsc' || value === 'priceDesc') {
      await this.page.waitForFunction(
        (sortValue) => {
          const priceElements = Array.from(document.querySelectorAll('[data-testid="product-price"]'));
          const prices = priceElements
            .map((el) => {
              const text = (el.textContent || '').replace(/[^0-9.]/g, '');
              return Number.parseFloat(text);
            })
            .filter((price) => Number.isFinite(price));

          if (prices.length < 2) return false;

          for (let i = 1; i < prices.length; i++) {
            if (sortValue === 'priceAsc' && prices[i] < prices[i - 1]) return false;
            if (sortValue === 'priceDesc' && prices[i] > prices[i - 1]) return false;
          }

          return true;
        },
        value
      );
    }
  }

  async goToPage(pageNumber: number) {
    await this.paginationPageButton(pageNumber).click();
  }

  async getProductNames(): Promise<string[]> {
    const names = await this.productNames.allTextContents();
    return names.map((name) => name.trim()).filter(Boolean);
  }

  async getProductPrices(): Promise<number[]> {
    await this.productPrices.first().waitFor({ state: 'visible' });
    const texts = await this.productPrices.allTextContents();
    return texts
      .map((t) => parseFloat(t.replace(/[^0-9.]/g, '')))
      .filter((price) => Number.isFinite(price));
  }

  async getProductCount(): Promise<number> {
    return this.productCards.count();
  }

  async clickProduct(index: number) {
    await this.productCards.nth(index).click();
  }

  createdProductCard(name: string, description: string): Locator {
    return this.page
      .getByTestId('product-card')
      .filter({ hasText: name })
      .filter({ hasText: description })
      .first();
  }

  paginationPageButton(page: number): Locator {
    return this.paginationNav.getByText(String(page), { exact: true });
  }
}
