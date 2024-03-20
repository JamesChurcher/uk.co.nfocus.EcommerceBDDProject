using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Infrastructure;
using uk.co.nfocus.EcommerceBDDProject.POCOClasses;
using uk.co.nfocus.EcommerceBDDProject.POMClasses;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.StepDefinitions
{
    [Binding]
    public class CheckoutSystemStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly WebDriverWrapper _driverWrapper;
        private readonly ISpecFlowOutputHelper _outputHelper;

        private NavBarPOM _navBar;
        private ScreenshotToggle _screenshotToggle;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper, ISpecFlowOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;
            _outputHelper = outputHelper;
            _screenshotToggle = (ScreenshotToggle)scenarioContext["ScreenshotToggle"];
        }


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
            Assert.That(loginStatus, "Could not login");   //Verify successful login        //TODO, Remove assert
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
        public void ThenTheCorrectAmountIsSubtractedFromTheTotal(string discountInput)
        {
            //Convert string to a percentage stored as a Decimal
            Decimal couponWorth = Decimal.Parse(StringToInt(discountInput).ToString());

            //Convert percentage to decimal
            couponWorth /= 100M;

            CartPagePOM cartPage = new(_driverWrapper);

            // Get subtotal from webage
            Decimal subtotal = cartPage.GetCartSubtotal();

            // Get shipping cost from webpage
            Decimal shippingCost = cartPage.GetShippingCost();

            // Calculate actual and expected discounts
            Decimal discountDeduction = cartPage.GetAppliedDiscount();     // Get actual discount amount

            // Caculate actual and expected totals
            Decimal expectedTotal = (subtotal * (1 - couponWorth)) + shippingCost;
            Decimal actualTotal = cartPage.GetCartTotal();

            // Get string representation of discount as percentage
            string actualCouponWorth = (discountDeduction / subtotal * 100).ToString("0.############################") + "%";
            string expectedCouponWorth = (couponWorth * 100).ToString("0.############################") + "%";


            // Screenshot the cart summary
            cartPage.ScrollToOrderTotal();
            TakeScreenshotAndAddToContext(_screenshotToggle, _driverWrapper, _outputHelper, "CartSummary", "Cart summary page");


            // Report testing information
            _outputHelper.WriteLine(
                $"Subtotal is £{subtotal.ToString("#.##")}\n" +
                $"Deduction is £{discountDeduction.ToString("#.##")}\n" +
                $"Expected discount is {couponWorth * 100}%\n" +
                $"Actual discount applied is {discountDeduction / subtotal * 100}%");
            _outputHelper.WriteLine(
                $"Shipping is £{shippingCost.ToString("#.##")}\n" +
                $"Expected total is £{expectedTotal.ToString("#.##")}\n" +
                $"Actual total is £{actualTotal.ToString("#.##")}");

            //Verification
            // Assess coupon removes 15%
            Assert.That(discountDeduction / subtotal, Is.EqualTo(couponWorth), $"Incorrect discount applied from coupon. Expected {expectedCouponWorth} but was {actualCouponWorth}");     //Verify coupon amount

            // Assess final total is correct
            Assert.That(actualTotal, Is.EqualTo(expectedTotal), "Final total subtotal incorrect");      //Verify final subtotal
        }


        [Given(@"we add (.*) randomly chosen items to the cart")]
        public void GivenWeAddRandomlyChosenItemsToTheCart(int quantity)
        {
            // Add to basket
            ShopPagePOM shopPage = new(_driverWrapper);

            // Add a random item to the basket
            string[] productNames = shopPage.GetProductNames();

            // Select three random items to add to the cart
            Random rnd = new();
            int randomIndex;

            // Add three random items to the cart
            for (int i = 0; i < quantity; i++)
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
            // Create POCO instance from table
            BillingDetailsPOCO billingDetails = billInfoTable.CreateInstance<BillingDetailsPOCO>();

            // Enter billing information
            CheckoutPagePOM checkoutPage = new(_driverWrapper);
            _outputHelper.WriteLine("Enter billing information");

            checkoutPage.CheckoutExpectSuccess(billingDetails);
            _outputHelper.WriteLine("Order submitted");
        }

        [Then(@"a new order is created")]
        public void ThenANewOrderIsCreated()
        {
            // Order summary page
            OrderPagePOM orderPage = new(_driverWrapper);
            string orderNumber = orderPage.GetOrderNumber();
            _scenarioContext["OrderNumber"] = orderNumber;
            _outputHelper.WriteLine($"New order number is #{orderNumber}");


            // Screenshot order summary page
            TakeScreenshotAndAddToContext(_screenshotToggle, _driverWrapper, _outputHelper, "OrderSummary", "New Order summary page");
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
            bool isOrderCreated = orderListPage.CheckIfOrderInOrderNumbers(orderNumber);


            // Screenshot listed account orders
            TakeScreenshotAndAddToContext(_screenshotToggle, _driverWrapper, _outputHelper, "AccountOrderList", "List of recent account orders");


            // Assess if previously created order is listed under this account
            if (isOrderCreated)
                _outputHelper.WriteLine($"Order #{orderNumber} is recorded under this account");
            else
                _outputHelper.WriteLine($"No order #{orderNumber} is recorded under this account");

            Assert.That(isOrderCreated, $"Created order #{orderNumber} not listed under this account");
        }
    }
}
