Feature: CheckoutSystem
As a potential customer
I want to be able to apply discounts, checkout, and view my orders
So that I can make and keep track of my purchases.

Background:
Assure each scenario is run while logged into an account.
	Given we are logged in
	  And we are on the shop page

@TestCase1
Scenario Outline: Apply discount to the cart
The correct discount should be applied to the cart total when a
coupon code is accepted.

	Given we add '<quantity>' of '<item>' to the cart
	  And we are viewing the cart page
	 When a discount code 'edgewords' is applied
	 Then 15% is subtracted from the total

	 Examples: 
	 | item       | quantity |
	 | Beanie     | 1        |
	 | Belt       | 3        |
	 | Cap,Hoodie | 1        |
	 | Hoodie     | 8        |

@TestCase2
Scenario: Checkout cart and create an order
Newly created order number should be visible when listing the
orders on the account.

	Given we have items in the cart
	  And we are viewing the checkout page
	 When a purchase is completed with billing information
	 | field         | values              |
	 | firstName     | Jeff                |
	 | lastName      | Bezos               |
	 | country       | United Kingdom (UK) |
	 | street        | Amazon lane         |
	 | city          | New York            |
	 | postcode      | W1J 7NT             |
	 | phoneNumber   | 07946 123400        |
	 | paymentMethod | cheque              |
	 Then a new order is created
	  And our account records this new order
