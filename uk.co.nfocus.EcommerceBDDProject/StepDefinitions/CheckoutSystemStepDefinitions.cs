using OpenQA.Selenium;
using System;
using TechTalk.SpecFlow;
using uk.co.nfocus.ecommerce_mini_project.POMClasses;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.StepDefinitions
{
    [Binding]
    public class CheckoutSystemStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        private IWebDriver _driver; //TODO > Make wrapper

        private NavBarPOM _navBar;

        private const decimal couponWorth = 0.15M;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;

            _driver = (IWebDriver)_scenarioContext["NewDriver"];
        }


        //----- Background -----
        [Given(@"we are logged in")]
        public void GivenWeAreLoggedIn()
        {
            string testUsername = "newexampleemail@email.com";
            string testPassword = "MyPassword12345@";

            // Create NavBar POM instance
            _navBar = new(_driver);
            _scenarioContext["NavBarPOMObject"] = _navBar;

            // Navigate to account login page
            _navBar.GoAccount();
            Console.WriteLine("Navigated to login page");

            // Login to said account
            AccountPagePOM loginPage = new(_driver);

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
            //TODO > Add the given quantity of each product to the cart

            Console.WriteLine($" Provide a quantity {quantity} of product {product}");

            // Add to basket
            ShopPagePOM shopPage = new(_driver);

            // If given a list of products, seperate them and loop over each one
            foreach(string item in product.Split(','))
            {
                // Add the given quantity of product to cart
                for (int i = quantity; i > 0; i--)
                {
                    shopPage.ClickAddToBasket(item);
                }
            }

            Console.WriteLine("Add product to cart");
        }

        [Given(@"we are viewing the cart page")]
        public void GivenWeAreViewingTheCartPage()
        {
            //TODO > Go to the cart page

            // View cart
            _navBar.GoCart();
            Console.WriteLine("Navigated to cart");
        }

        [When(@"a (.*)% discount code '([^']*)' is applied")]
        public void WhenADiscountCodeIsApplied(int p0, string edgewords)
        {
            //TODO > Apply the discount code

            string testDiscountCode = "edgewords";

            // Apply coupon
            CartPagePOM cartPage = new(_driver);
            bool discountStatus = cartPage.ApplyDiscountExpectSuccess(testDiscountCode);
            Assert.That(discountStatus, "Could not apply discount");   //Verify discount was applied
            Console.WriteLine("Applied coupon code");
        }

        [Then(@"the correct amount is subtracted from the total")]
        public void ThenTheCorrectAmountIsSubtractedFromTheTotal()
        {
            //TODO > Read subtotal, total, shipping, and subtracted discount from page
            //TODO > Calculate actual discount amount applied

            CartPagePOM cartPage = new(_driver);

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

            //TODO > Compare with expected

            //Verification
            // Assess coupon removes 15%
            try     //Verify coupon amount
            {
                Assert.That(actualDiscount, Is.EqualTo(expectedDiscount), "Incorrect discount applied");
            }
            catch (AssertionException)   //TODO > Catch Assert exceptions only
            {
                //Do nothing
            }
            Console.WriteLine($"15% discount amount ->\n\tExpected: £{actualDiscount}, Actual: £{actualDiscount}");

            // Assess final total is correct
            try     //Verify final subtotal
            {
                Assert.That(actualTotal, Is.EqualTo(expectedTotal), "Final total subtotal incorrect");
            }
            catch (AssertionException)   //TODO > Catch Assert exceptions only
            {
                //Do nothing
            }
            Console.WriteLine($"Final subtotal ->\n\tExpected: £{expectedTotal}, Actual: £{actualTotal}");

            // Screenshot the cart summary
            ScrollToElement(_driver, _driver.FindElement(By.ClassName("order-total")));
            TakeScreenshot(_driver, "TestCase1_CartSummary", "Cart summary page");
        }


        //----- Testcase 2 -----
        [Given(@"we have items in the cart")]
        public void GivenWeHaveItemsInTheCart()
        {
            //TODO > Add an product to the cart
            _scenarioContext.Pending();
        }

        [Given(@"we are viewing the checkout page")]
        public void GivenWeAreViewingTheCheckoutPage()
        {
            //TODO > Go to checkout page
            _scenarioContext.Pending();
        }

        [When(@"a purchase is completed")]
        public void WhenAPurchaseIsCompleted()
        {
            //TODO > Fill in billing information
            //TODO > Place order
            _scenarioContext.Pending();
        }

        [Then(@"a new order is created")]
        public void ThenANewOrderIsCreated()
        {
            //TODO > Capture order number
            _scenarioContext.Pending();
        }

        [Then(@"our account records this new order")]
        public void ThenOurAccountRecordsThisNewOrder()
        {
            //TODO > Go to account orders page
            //TODO > Get all order numbers from accounts orders page
            //TODO > Compare with captured ordernumber
            _scenarioContext.Pending();
        }
    }
}
