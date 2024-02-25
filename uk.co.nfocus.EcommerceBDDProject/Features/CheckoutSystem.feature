Feature: CheckoutSystem
To see if the user can purchase from the website
we want to see if features of the cart and checkout system
function as intended.

Background: 
Making sure each scenario is tested while logged into an account.
	Given we are logged in
	  And we are on the shop page

Scenario Outline: Apply discount to the cart
Add some provided items to the cart and apply a 15% discount.
	Given we add '<quantity>' of '<item>' to the cart
	  And we are viewing the cart page
	 When a 15% discount code 'edgewords' is applied
	 Then the correct amount is subtracted from the total

	 Examples: 
	 | item       | quantity |
	 | beanie     | 1        |
	 | belt       | 3        |
	 | cap,hoodie | 1        |

Scenario: Checkout cart and create an order
Checkout and create an order and that the order appears under the
accounts list of orders.
	Given we have items in the cart
	  And we are viewing the checkout page
	 When a purchase is completed
	 Then a new order is created
	  And our account records this new order
