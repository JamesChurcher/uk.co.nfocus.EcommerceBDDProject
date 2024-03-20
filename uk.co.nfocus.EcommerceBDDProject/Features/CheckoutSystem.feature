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
	 When a discount code '<code>' is applied
	 Then <discount> is subtracted from the total

	 Examples: 
	 | item       | quantity | code      | discount |
	 | Beanie     | 1        | edgewords | 15%      |
	 | Belt       | 3        | edgewords | 15%      |
	 | Cap,Hoodie | 1        | nfocus    | 25%      |
	 | Hoodie     | 8        | edgewords | 15%      |

@TestCase2
Scenario: Checkout cart and create an order
Newly created order number should be visible when listing the
orders on the account.

	Given we add 3 randomly chosen items to the cart
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
