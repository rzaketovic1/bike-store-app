import { createProduct, deleteProduct } from '../helpers/api.helper';

const TEST_PRODUCT = {
  name: 'E2E Test Bike',
  description: 'A bike created for E2E testing',
  price: 499.99,
  pictureUrl: '/images/products/test-image.jpg',
  brand: 'Trek',
  type: 'Mountain',
  quantityInStock: 10,
};

/**
 * Creates a test product via API. Returns the product (includes id).
 */
export async function seedProduct(token: string, overrides?: Partial<typeof TEST_PRODUCT>) {
  const product = { ...TEST_PRODUCT, ...overrides };
  return createProduct(token, product);
}

/**
 * Deletes a test product via API. Silently ignores 404.
 */
export async function cleanupProduct(token: string, productId: number) {
  await deleteProduct(token, productId);
}
