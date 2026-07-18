import { test, expect } from '@playwright/test';
import { ProductListPage } from '../../pages/product-list.page';
import { CartPage } from '../../pages/cart.page';
import { CheckoutPage } from '../../pages/checkout.page';

test.describe('Checkout', () => {
  test('should complete checkout successfully with valid customer data', async ({ page }) => {
    const productList = new ProductListPage(page);
    const cartPage = new CartPage(page);
    const checkoutPage = new CheckoutPage(page);

    // 1) Open product list and add first product to cart
    await productList.goto();
    await expect(productList.productCards.first()).toBeVisible();
    await productList.productCards.first().getByTestId('add-to-cart-btn').click();

    // 2) Verify cart icon count increased
    await expect(productList.cartCountBadge).toBeVisible();
    await expect(productList.cartCountBadge).toHaveText('1');

    // 3) Open cart page and verify item + subtotal
    await productList.cartNavLink.click();
    await expect(cartPage.heading).toContainText('Shopping Cart');
    await expect(cartPage.cartItemRows.first()).toBeVisible();
    await expect(cartPage.cartSubtotal).toBeVisible();
    expect(await cartPage.getSubtotal()).toBeGreaterThan(0);

    // 4) Proceed to checkout and verify summary
    await cartPage.proceedToCheckout();
    await expect(checkoutPage.heading).toContainText('Checkout');
    await expect(checkoutPage.orderSummary).toBeVisible();
    await expect(checkoutPage.checkoutItemRows.first()).toBeVisible();
    expect(await checkoutPage.getTotal()).toBeGreaterThan(0);

    // 5) Fill customer info and finish order
    await checkoutPage.fillCustomerInfo({
      firstName: 'Jane',
      lastName: 'Doe',
      email: 'jane.doe@example.com',
      address: '123 Main St, Zagreb',
    });
    await checkoutPage.finishOrder();

    // 6) Verify completion page and cart cleared
    await expect(page).toHaveURL('/checkout/complete');
    await expect(page.getByTestId('checkout-complete-heading')).toContainText('Order Placed Successfully!');
    await expect(productList.cartCountBadge).not.toBeVisible();
  });
});
