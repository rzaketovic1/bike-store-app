Feature: Product API

  Scenario: Successfully creating a new product
    Given I have a product with name "Test Product" and price 100
    When I send a POST request to /api/products
    Then the response status should be 201
    And the product should be returned in the response