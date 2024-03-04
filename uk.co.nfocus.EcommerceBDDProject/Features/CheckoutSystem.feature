Feature: CheckoutSystem
To see if the user can purchase from the website
we want to see if features of the cart and checkout system
function as intended.

Background: 
Making sure each scenario is tested while logged into an account.
	Given we are logged in
	  And we are on the shop page

@TestCase1
Scenario Outline: Apply discount to the cart
Add some provided items to the cart and apply a 15% discount.

	Given we add '<quantity>' of '<item>' to the cart
	  And we are viewing the cart page
	 When a discount code 'edgewords' is applied
	 Then 10% is subtracted from the total

	 Examples: 
	 | item       | quantity |
	 | Beanie     | 1        |
	 | Belt       | 3        |
	 | Cap,Hoodie | 1        |
	 | Hoodie     | 8        |

@TestCase2
Scenario: Checkout cart and create an order
Checkout and create an order and that the order appears under the
accounts list of orders.

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
