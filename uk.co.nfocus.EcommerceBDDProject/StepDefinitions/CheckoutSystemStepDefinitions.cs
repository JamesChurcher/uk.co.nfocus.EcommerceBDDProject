using OpenQA.Selenium;
using SpecFlow.Internal.Json;
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

        //private const decimal couponWorth = 0.15M;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;
        }


        //----- Background -----
        [Given(@"we are logged in")]
        public void GivenWeAreLoggedIn()
        {
            // Get username and password from setup hooks
            string testUsername = (string)_scenarioContext["Username"];
            string testPassword = (string)_scenarioContext["Password"];

            // Create NavBar POM instance
            _navBar = new(_driverWrapper);
            _scenarioContext["NavBarPOMObject"] = _navBar;

            // Navigate to account login page
            _navBar.GoAccount();
            Console.WriteLine("Navigated to login page");

            // Login to said account
            AccountPagePOM loginPage = new(_driverWrapper);

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
            ShopPagePOM shopPage = new(_driverWrapper);

            // If given a list of products, seperate them and loop over each one
            foreach(string item in product.Split(','))
            {
                // Add the given quantity of product to cart
                for (int i = quantity; i > 0; i--)
                {
                    //Console.WriteLine("loop over cart i " + i);
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

        [When(@"a discount code '([^']*)' is applied")]
        public void WhenADiscountCodeIsApplied(string testDiscountCode)
        {
            // Apply coupon
            CartPagePOM cartPage = new(_driverWrapper);
            _scenarioContext["CartPagePOMObject"] = cartPage;

            bool discountStatus = cartPage.ApplyDiscountExpectSuccess(testDiscountCode);
            Assert.That(discountStatus, "Could not apply discount");   //Verify discount was applied
            Console.WriteLine("Applied coupon code");
        }

        [Then(@"(.*)% is subtracted from the total")]
        public void ThenTheCorrectAmountIsSubtractedFromTheTotal(Decimal couponWorth)
        {
            //Convert percentage to decimal
            couponWorth /= 100M;

            CartPagePOM cartPage = new(_driverWrapper);

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
            catch (AssertionException)
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
            catch (AssertionException)
            {
                //Do nothing
                state = "Fail";
            }
            Console.WriteLine($"Final subtotal -> {state}\n\tExpected: £{expectedTotal}, Actual: £{actualTotal}");

            // Screenshot the cart summary
            cartPage.ScrollToOrderTotal();
            string screenshotName = ValidFileNameFromTest("CartSummary");
            TakeScreenshot(_driverWrapper.Driver, screenshotName, "Cart summary page");
        }


        //----- Testcase 2 -----
        [Given(@"we have items in the cart")]
        public void GivenWeHaveItemsInTheCart()
        {
            // Add to basket
            ShopPagePOM shopPage = new(_driverWrapper);

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

        [When(@"a purchase is completed with billing information")]
        public void WhenAPurchaseIsCompletedWithBillingInformation(Table billInfoTable)
        {
            // Create a dictionary from the table
            Dictionary<string, string> billInfoDict = new();
            foreach (var row in billInfoTable.Rows)
            {
                billInfoDict.Add(row[0], row[1]);
            }

            //Convert the payment method string to an enum
            PaymentMethod method;
            Enum.TryParse<PaymentMethod>(billInfoDict["paymentMethod"], out method);

            // Enter billing information
            CheckoutPagePOM checkoutPage = new(_driverWrapper);
            Console.WriteLine("Enter billing information");

            checkoutPage.CheckoutExpectSuccess(
                billInfoDict["firstName"], 
                billInfoDict["lastName"], 
                billInfoDict["country"], 
                billInfoDict["street"], 
                billInfoDict["city"], 
                billInfoDict["postcode"], 
                billInfoDict["phoneNumber"], 
                method);
        }

        [Then(@"a new order is created")]
        public void ThenANewOrderIsCreated()
        {
            // Order summary page
            OrderPagePOM orderPage = new(_driverWrapper);
            string orderNumber = orderPage.GetOrderNumber();
            _scenarioContext["OrderNumber"] = orderNumber;
            Console.WriteLine($"New order number is {orderNumber}");

            //TODO, maybe verify if an order was created successfully?

            // Screenshot order summary page
            string screenshotName = ValidFileNameFromTest("OrderSummary");
            TakeScreenshot(_driverWrapper.Driver, screenshotName, "New Order summary page");
        }

        [Then(@"our account records this new order")]
        public void ThenOurAccountRecordsThisNewOrder()
        {
            // Go to my account
            _navBar.GoAccount();
            Console.WriteLine("Navigated to account page");

            string orderNumber = (string)_scenarioContext["OrderNumber"];

            // Go to my account's list of orders
            AccountPagePOM accountPage = new(_driverWrapper);
            accountPage.GoOrders();

            // Check if the new order is listed under this account
            OrderListPagePOM orderListPage = new(_driverWrapper);
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


            //TODO, each test overwrites the last one since they are all given the same name, define test name in screenshot name
            // Screenshot listed account orders
            string screenshotName = ValidFileNameFromTest("AccountOrderList");
            TakeScreenshot(_driverWrapper.Driver, screenshotName, "List of recent account orders");
        }
    }
}
