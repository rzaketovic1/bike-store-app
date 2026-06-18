import { test, expect } from '@playwright/test';
import { ProductListPage } from '../../pages/product-list.page';
import { CartPage } from '../../pages/cart.page';

test.describe('Cart', () => {
  test('user can add a product to cart and verify it appears in cart', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    // Wait for first product card to load
    await expect(productList.productCards.first()).toBeVisible();

    // Capture first product name for later assertion
    const firstProductName = await productList.productNames.first().textContent();

    // Add first product to cart
    await productList.productCards.first().getByTestId('add-to-cart-btn').click();

    // Verify cart count badge appears and shows 1
    await expect(productList.cartCountBadge).toBeVisible();
    await expect(productList.cartCountBadge).toHaveText('1');

    // Navigate to cart page
    await productList.cartNavLink.click();
    const cartPage = new CartPage(page);

    await expect(cartPage.heading).toContainText('Shopping Cart');

    // Verify the product exists in cart
    await expect(cartPage.cartItemRows.first()).toBeVisible();
    await expect(cartPage.cartItemName(0)).toContainText(firstProductName!.trim());

    // Verify subtotal is greater than zero
    const subtotal = await cartPage.getSubtotal();
    expect(subtotal).toBeGreaterThan(0);
  });

  test('user can remove item from cart and cart becomes empty', async ({ page }) => {
    const productList = new ProductListPage(page);
    await productList.goto();

    await expect(productList.productCards.first()).toBeVisible();

    // Add first product to cart
    await productList.productCards.first().getByTestId('add-to-cart-btn').click();
    await expect(productList.cartCountBadge).toBeVisible();

    // Navigate to cart page
    await productList.cartNavLink.click();
    const cartPage = new CartPage(page);

    await expect(cartPage.cartItemRows.first()).toBeVisible();

    // Remove the item
    await cartPage.cartRemoveButton(0).click();

    // Verify cart is now empty
    await expect(cartPage.emptyCartMessage).toBeVisible();
    await expect(cartPage.cartCountBadge).not.toBeVisible();
  });
});
