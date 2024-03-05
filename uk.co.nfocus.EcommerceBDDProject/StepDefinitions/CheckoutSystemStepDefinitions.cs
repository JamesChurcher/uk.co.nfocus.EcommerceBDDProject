using OpenQA.Selenium;
using SpecFlow.Internal.Json;
using System;
using System.Diagnostics.Metrics;
using System.IO;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Infrastructure;
using uk.co.nfocus.EcommerceBDDProject.POMClasses;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.StepDefinitions
{
    [Binding]
    public class CheckoutSystemStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private WebDriverWrapper _driverWrapper;
        private readonly ISpecFlowOutputHelper _outputHelper;

        private NavBarPOM _navBar;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper, ISpecFlowOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;
            _outputHelper = outputHelper;
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

            // Dismiss popup
            _navBar.DismissPopup();

            // Navigate to account login page
            _navBar.GoAccount();
            _outputHelper.WriteLine("Navigated to login page");

            // Login to said account
            AccountPagePOM loginPage = new(_driverWrapper);

            //Provide username, password, and click
            bool loginStatus = loginPage.LoginExpectSuccess(testUsername, testPassword);
            Assert.That(loginStatus, "Could not login");   //Verify successful login
            _outputHelper.WriteLine("Login complete");

            // Clear cart
            _navBar.GoCart();
            CartPagePOM cartPage = new(_driverWrapper);
            cartPage.MakeCartEmpty();
        }

        [Given(@"we are on the shop page")]
        public void GivenWeAreOnTheShopPage()
        {
            // Enter the shop
            _navBar.GoShop();
            _outputHelper.WriteLine("Navigated to shop");
        }


        //----- Testcase 1 -----
        [Given(@"we add '([^']*)' of '([^']*)' to the cart")]
        public void GivenWeAddOfToTheCart(int quantity, string product)
        {
            _outputHelper.WriteLine($"Provide a quantity {quantity} of product {product}");

            // Add to basket
            ShopPagePOM shopPage = new(_driverWrapper);

            // If given a list of products, seperate them and loop over each one
            foreach (string item in product.Split(','))
            {
                // Add the given quantity of product to cart
                for (int i = quantity; i > 0; i--)
                {
                    //Console.WriteLine("loop over cart i " + i);
                    shopPage.ClickAddToBasket(item);

                    _outputHelper.WriteLine($"Added {item} to the basket");
                }
            }

            _outputHelper.WriteLine("Products added to cart");
        }

        [Given(@"we are viewing the cart page")]
        public void GivenWeAreViewingTheCartPage()
        {
            // View cart
            _navBar.GoCart();
            _outputHelper.WriteLine("Navigated to cart");
        }

        [When(@"a discount code '([^']*)' is applied")]
        public void WhenADiscountCodeIsApplied(string testDiscountCode)
        {
            _outputHelper.WriteLine($"Attempt to apply code: {testDiscountCode}");

            // Apply coupon
            CartPagePOM cartPage = new(_driverWrapper);

            bool discountStatus = cartPage.ApplyDiscountExpectSuccess(testDiscountCode);
            //Assert.That(discountStatus, "Could not apply discount");   //Verify discount was applied

            if (discountStatus)
                _outputHelper.WriteLine("Applied discount code");
            else
                _outputHelper.WriteLine("Could not apply discount code");
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
            //Decimal expectedDiscount = subtotal * couponWorth;          // Calculate expected discount amount
            Decimal actualDiscount = cartPage.GetAppliedDiscount() / subtotal;     // Get actual discount amount

            // Caculate actual and expected totals
            Decimal expectedTotal = (subtotal * (1 - couponWorth)) + shippingCost;
            Decimal actualTotal = cartPage.GetCartTotal();


            // Screenshot the cart summary
            cartPage.ScrollToOrderTotal();
            string screenshotName = ValidFileNameFromTest("CartSummary");
            string imagePath = TakeScreenshot(_driverWrapper.Driver, screenshotName, "Cart summary page");
            _outputHelper.AddAttachment(@"file:///" + imagePath);


            //Verification
            // Assess coupon removes 15%
            Assert.That(actualDiscount, Is.EqualTo(couponWorth), "Incorrect discount applied from coupon");     //Verify coupon amount
            _outputHelper.WriteLine($"15% discount amount ->\n\tExpected: £{actualDiscount}, Actual: £{actualDiscount}");

            // Assess final total is correct
            Assert.That(actualTotal, Is.EqualTo(expectedTotal), "Final total subtotal incorrect");      //Verify final subtotal
            _outputHelper.WriteLine($"Final subtotal ->\n\tExpected: £{expectedTotal}, Actual: £{actualTotal}");
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
            int randomIndex;

            // Add three random items to the cart
            for (int i = 0; i < 3; i++)
            {
                randomIndex = rnd.Next(productNames.Count());
                shopPage.ClickAddToBasket(productNames[randomIndex]);
                _outputHelper.WriteLine($"Added {productNames[randomIndex]} to the basket");
            }

            _outputHelper.WriteLine("Products added to cart");
        }

        [Given(@"we are viewing the checkout page")]
        public void GivenWeAreViewingTheCheckoutPage()
        {
            // Go to checkout
            _navBar.GoCheckout();
            _outputHelper.WriteLine("Navigated to checkout");
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
            _outputHelper.WriteLine("Enter billing information");

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
            _outputHelper.WriteLine($"New order number is {orderNumber}");

            // Screenshot order summary page
            string screenshotName = ValidFileNameFromTest("OrderSummary");
            string imagePath = TakeScreenshot(_driverWrapper.Driver, screenshotName, "New Order summary page");
            _outputHelper.AddAttachment(@"file:///" + imagePath);
        }

        [Then(@"our account records this new order")]
        public void ThenOurAccountRecordsThisNewOrder()
        {
            // Go to my account
            _navBar.GoAccount();
            _outputHelper.WriteLine("Navigated to account page");

            string orderNumber = (string)_scenarioContext["OrderNumber"];

            // Go to my account's list of orders
            AccountPagePOM accountPage = new(_driverWrapper);
            accountPage.GoOrders();

            // Check if the new order is listed under this account
            OrderListPagePOM orderListPage = new(_driverWrapper);
            bool isOrderCreated = orderListPage.CheckIfOrderInOrderNumbers(orderNumber);    //TODO, maybe move comparison outside of POM so we get the actual order number


            // Screenshot listed account orders
            string screenshotName = ValidFileNameFromTest("AccountOrderList");
            string imagePath = TakeScreenshot(_driverWrapper.Driver, screenshotName, "List of recent account orders");
            _outputHelper.AddAttachment(@"file:///" + imagePath);


            // Assess if previously created order is listed under this account
            Assert.That(isOrderCreated, "Created order not listed under this account");
            _outputHelper.WriteLine($"Is the new order listed under account? {isOrderCreated}");
        }
    }
}
