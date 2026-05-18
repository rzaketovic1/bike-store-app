import { Page, Locator } from '@playwright/test';
import { BasePage } from './base.page';

export class ProductDetailsPage extends BasePage {
  readonly productName: Locator;
  readonly productPrice: Locator;
  readonly productDescription: Locator;
  readonly productBrand: Locator;
  readonly productType: Locator;
  readonly productStock: Locator;
  readonly productImage: Locator;
  readonly editButton: Locator;
  readonly deleteButton: Locator;

  // Edit modal
  readonly editModal: Locator;
  readonly editNameInput: Locator;
  readonly editDescriptionInput: Locator;
  readonly editPriceInput: Locator;
  readonly editBrandSelect: Locator;
  readonly editTypeSelect: Locator;
  readonly editQuantityInput: Locator;
  readonly editImageInput: Locator;
  readonly editSaveButton: Locator;
  readonly editCancelButton: Locator;

  constructor(page: Page) {
    super(page);
    this.productName = page.getByTestId('product-name');
    this.productPrice = this.productPrice = page.getByTestId('product-price');
    this.productDescription = page.getByTestId('product-description');
    this.productBrand = page.getByTestId('product-brand');
    this.productType = page.getByTestId('product-type');
    this.productStock = page.getByTestId('product-stock');
    this.productImage = this.productImage = page.getByAltText('product image');
    this.editButton = page.getByRole('button', { name: /edit/i });
    this.deleteButton = page.getByRole('button', { name: /delete/i });

    // Edit modal
    this.editModal = page.locator('#editProductModal');
    this.editNameInput        = this.editModal.getByLabel('Name');
    this.editDescriptionInput = this.editModal.getByLabel('Description');
    this.editPriceInput       = this.editModal.getByLabel('Price');
    this.editBrandSelect      = this.editModal.getByLabel('Brand');
    this.editTypeSelect       = this.editModal.getByLabel('Type');
    this.editQuantityInput    = this.editModal.getByLabel('Quantity In Stock');
    this.editImageInput = this.editModal.getByLabel('Change Image (optional)');
    this.editSaveButton   = this.editModal.getByRole('button', { name: /save changes/i });
    this.editCancelButton = this.editModal.getByRole('button', { name: /cancel/i });
  }

  async goto(productId: number) {
    await this.page.goto(`/product/${productId}`);
  }

  async openEditModal() {
    await this.editButton.click();
    await this.editModal.waitFor({ state: 'visible' });
  }

  async editProduct(fields: {
    name?: string;
    description?: string;
    price?: number;
    brand?: string;
    type?: string;
    quantity?: number;
  }) {
    if (fields.name !== undefined) {
      await this.editNameInput.clear();
      await this.editNameInput.fill(fields.name);
    }
    if (fields.description !== undefined) {
      await this.editDescriptionInput.clear();
      await this.editDescriptionInput.fill(fields.description);
    }
    if (fields.price !== undefined) {
      await this.editPriceInput.clear();
      await this.editPriceInput.fill(String(fields.price));
    }
    if (fields.brand !== undefined) {
      await this.editBrandSelect.selectOption(fields.brand);
    }
    if (fields.type !== undefined) {
      await this.editTypeSelect.selectOption(fields.type);
    }
    if (fields.quantity !== undefined) {
      await this.editQuantityInput.clear();
      await this.editQuantityInput.fill(String(fields.quantity));
    }
    await this.editSaveButton.click();
  }

  async deleteProduct() {
    await this.deleteButton.click();
  }
}
