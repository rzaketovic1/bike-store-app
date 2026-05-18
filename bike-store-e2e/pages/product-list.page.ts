import { Page, Locator, expect } from '@playwright/test';
import { BasePage } from './base.page';

export class ProductListPage extends BasePage {
  readonly pageHeading: Locator;
  readonly pageSubtitle: Locator;
  readonly productCards: Locator;
  readonly addProductButton: Locator;
  readonly sortDropdown: Locator;
  readonly emptyState: Locator;
  readonly loadingSpinner: Locator;

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
    this.addProductButton = page.getByTestId('add-product-button');
    this.sortDropdown = page.getByLabel('Sort by:');
    this.emptyState = page.getByTestId('products-empty-state');
    this.loadingSpinner = page.locator('.spinner-border');

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
    this.previousPage = this.paginationNav.getByText('Previous');
    this.nextPage = this.paginationNav.getByText('Next');
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
  }

  async filterByType(type: string) {
    await this.typeFilterButton(type).click();
  }

  async getFirstSpecificBrand(): Promise<string | undefined> {
    const buttons = this.brandFilterSection.getByRole('button');
    await buttons.first().waitFor();
    const labels = await buttons.allTextContents();
    return labels.map((b) => b.trim()).find((b) => b !== 'All');
  }

  async getFirstSpecificType(): Promise<string | undefined> {
    const buttons = this.typeFilterSection.getByRole('button');
    await buttons.first().waitFor();
    const labels = await buttons.allTextContents();
    return labels.map((t) => t.trim()).find((t) => t !== 'All');
  }

  async expectAllCardsHaveBrand(brand: string) {
    const count = await this.productCards.count();
    for (let i = 0; i < count; i++) {
      await expect(this.productCards.nth(i).getByTestId('product-brand-badge')).toContainText(brand);
    }
  }

  async expectAllCardsHaveType(type: string) {
    const count = await this.productCards.count();
    for (let i = 0; i < count; i++) {
      await expect(this.productCards.nth(i).getByTestId('product-type-badge')).toContainText(type);
    }
  }

  async sortBy(value: string) {
    await this.sortDropdown.selectOption(value);
  }

  async goToPage(pageNumber: number) {
    await this.paginationPageButton(pageNumber).click();
  }

  async getProductNames(): Promise<string[]> {
    return this.productCards.locator('.card-title').allTextContents();
  }

  async getProductPrices(): Promise<number[]> {
    const texts = await this.productCards.locator('.text-success').allTextContents();
    return texts.map((t) => parseFloat(t.replace(/[^0-9.]/g, '')));
  }

  async getProductCount(): Promise<number> {
    return this.productCards.count();
  }

  async clickProduct(index: number) {
    await this.productCards.nth(index).click();
  }

  paginationPageButton(page: number): Locator {
    return this.paginationNav.getByText(String(page), { exact: true });
  }
}
