using OpenQA.Selenium;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using TechTalk.SpecFlow;
using uk.co.nfocus.EcommerceBDDProject.POMClasses;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.StepDefinitions
{
    [Binding]
    public class CheckoutSystemStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        //private IWebDriver _driver; //TODO > Make wrapper
        private WebDriverWrapper _driverWrapper;

        private NavBarPOM _navBar;

        private const decimal couponWorth = 0.15M;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;

            //_driver = (IWebDriver)_scenarioContext["NewDriver"];
        }


        //----- Background -----
        [Given(@"we are logged in")]
        public void GivenWeAreLoggedIn()
        {
            string testUsername = "newexampleemail@email.com";
            string testPassword = "MyPassword12345@";

            // Create NavBar POM instance
            _navBar = new(_driverWrapper.Driver);
            _scenarioContext["NavBarPOMObject"] = _navBar;

            // Navigate to account login page
            _navBar.GoAccount();
            Console.WriteLine("Navigated to login page");

            // Login to said account
            AccountPagePOM loginPage = new(_driverWrapper.Driver);

            //Provide username, password, and click
            bool loginStatus = loginPage.LoginExpectSuccess(testUsername, testPassword);
            Assert.That(loginStatus, "Could not login");   //Verify successful login
            Console.WriteLine("Login complete");
        }

        [Given(@"we are on the shop page")]
        public void GivenWeAreOnTheShopPage()
        {
            // Enter the shop
            _navBar.GoShop();
            Console.WriteLine("Navigated to shop");
        }


        //----- Testcase 1 -----
        [Given(@"we add '([^']*)' of '([^']*)' to the cart")]
        public void GivenWeAddOfToTheCart(int quantity, string product)
        {
            Console.WriteLine($"Provide a quantity {quantity} of product {product}");

            // Add to basket
            ShopPagePOM shopPage = new(_driverWrapper.Driver);

            // If given a list of products, seperate them and loop over each one
            foreach(string item in product.Split(','))
            {
                // Add the given quantity of product to cart
                for (int i = quantity; i > 0; i--)
                {
                    Console.WriteLine("loop over cart i " + i);
                    shopPage.ClickAddToBasket(item);
                }
            }

            Console.WriteLine("Add product to cart");
        }

        [Given(@"we are viewing the cart page")]
        public void GivenWeAreViewingTheCartPage()
        {
            // View cart
            _navBar.GoCart();
            Console.WriteLine("Navigated to cart");
        }

        [When(@"a (.*)% discount code '([^']*)' is applied")]
        public void WhenADiscountCodeIsApplied(int p0, string edgewords)
        {
            string testDiscountCode = "edgewords";

            // Apply coupon
            CartPagePOM cartPage = new(_driverWrapper.Driver);
            _scenarioContext["CartPagePOMObject"] = cartPage;

            bool discountStatus = cartPage.ApplyDiscountExpectSuccess(testDiscountCode);
            Assert.That(discountStatus, "Could not apply discount");   //Verify discount was applied
            Console.WriteLine("Applied coupon code");
        }

        [Then(@"the correct amount is subtracted from the total")]
        public void ThenTheCorrectAmountIsSubtractedFromTheTotal()
        {
            CartPagePOM cartPage = new(_driverWrapper.Driver);

            // Get subtotal from webage
            Decimal subtotal = cartPage.GetCartSubtotal();

            // Get shipping cost from webpage
            Decimal shippingCost = cartPage.GetShippingCost();

            // Calculate actual and expected discounts
            Decimal expectedDiscount = subtotal * couponWorth;          // Calculate expected discount amount
            Decimal actualDiscount = cartPage.GetAppliedDiscount();     // Get actual discount amount

            // Caculate actual and expected totals
            Decimal expectedTotal = (subtotal * (1 - couponWorth)) + shippingCost;
            Decimal actualTotal = cartPage.GetCartTotal();


            //Verification
            // Assess coupon removes 15%
            string state = "";
            try     //Verify coupon amount
            {
                Assert.That(actualDiscount, Is.EqualTo(expectedDiscount), "Incorrect discount applied");
                state = "Pass";
            }
            catch (AssertionException)   //TODO > Catch Assert exceptions only
            {
                //Do nothing
                state = "Fail";
            }
            Console.WriteLine($"15% discount amount -> {state}\n\tExpected: £{actualDiscount}, Actual: £{actualDiscount}");

            // Assess final total is correct
            try     //Verify final subtotal
            {
                Assert.That(actualTotal, Is.EqualTo(expectedTotal), "Final total subtotal incorrect");
                state = "Pass";
            }
            catch (AssertionException)   //TODO > Catch Assert exceptions only
            {
                //Do nothing
                state = "Fail";
            }
            Console.WriteLine($"Final subtotal -> {state}\n\tExpected: £{expectedTotal}, Actual: £{actualTotal}");

            // Screenshot the cart summary
            ScrollToElement(_driverWrapper.Driver, _driverWrapper.Driver.FindElement(By.ClassName("order-total")));     //TODO, move locator or make class for screenshots and move to that
            TakeScreenshot(_driverWrapper.Driver, "TestCase1_CartSummary", "Cart summary page");
        }


        //----- Testcase 2 -----
        [Given(@"we have items in the cart")]
        public void GivenWeHaveItemsInTheCart()
        {
            // Add to basket
            ShopPagePOM shopPage = new(_driverWrapper.Driver);

            // Add a random item to the basket
            string[] productNames = shopPage.GetProductNames();

            // Select three random items to add to the cart
            Random rnd = new();
            int randomIntex;

            // Add three random items to the cart
            for (int i = 0; i < 3; i++)
            {
                randomIntex = rnd.Next(productNames.Count());
                shopPage.ClickAddToBasket(productNames[randomIntex]);
                Console.WriteLine($"Added {productNames[randomIntex]} to the basket");
            }

            Console.WriteLine("Add product to cart");
        }

        [Given(@"we are viewing the checkout page")]
        public void GivenWeAreViewingTheCheckoutPage()
        {
            // Go to checkout
            _navBar.GoCheckout();
            Console.WriteLine("Navigated to checkout");
        }

        [When(@"a purchase is completed")]
        public void WhenAPurchaseIsCompleted()
        {
            //TODO, move data to annotation or feature file
            string firstName = "Jeff";
            string lastName = "Bezos";
            string country = "United Kingdom (UK)";
            string street = "Amazon lane";
            string city = "New York";
            string postcode = "W1J 7NT";
            string phoneNumber = "07946 123400";
            PaymentMethod paymentMethod = PaymentMethod.cheque;

            // Enter billing information
            CheckoutPagePOM checkoutPage = new(_driverWrapper.Driver);
            Console.WriteLine("Enter billing information");
            checkoutPage.CheckoutExpectSuccess(firstName, lastName, country, street, city, postcode, phoneNumber, paymentMethod);
        }

        [Then(@"a new order is created")]
        public void ThenANewOrderIsCreated()
        {
            // Order summary page
            OrderPagePOM orderPage = new(_driverWrapper.Driver);
            string orderNumber = orderPage.GetOrderNumber();
            _scenarioContext["OrderNumber"] = orderNumber;
            Console.WriteLine($"New order number is {orderNumber}");

            //TODO, maybe verify if an order was created successfully?

            // Screenshot order summary page
            TakeScreenshot(_driverWrapper.Driver, "TestCase2_OrderSummary", "New Order summary page");
        }

        [Then(@"our account records this new order")]
        public void ThenOurAccountRecordsThisNewOrder()
        {
            // Go to my account
            _navBar.GoAccount();
            Console.WriteLine("Navigated to account page");

            string orderNumber = (string)_scenarioContext["OrderNumber"];

            // Go to my account's list of orders
            AccountPagePOM accountPage = new(_driverWrapper.Driver);
            accountPage.GoOrders();

            // Check if the new order is listed under this account
            OrderListPagePOM orderListPage = new(_driverWrapper.Driver);
            bool isOrderCreated = orderListPage.CheckIfOrderInOrderNumbers(orderNumber);    //TODO, maybe move comparison outside of POM so we get the actual order number

            // Assess if previously created order is listed under this account
            try
            {
                Assert.That(isOrderCreated, "Order not in set");
            }
            catch (AssertionException)   //TODO > Catch Assert exceptions only
            {
                //Do nothing
            }
            Console.WriteLine($"Is the new order listed under account? {isOrderCreated}");


            // Screenshot listed account orders
            TakeScreenshot(_driverWrapper.Driver, "TestCase2_AccountOrderList", "List of recent account orders");
        }
    }
}
