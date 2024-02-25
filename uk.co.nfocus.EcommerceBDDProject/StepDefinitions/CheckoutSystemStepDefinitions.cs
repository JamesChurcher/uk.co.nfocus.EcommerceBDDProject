using System;
using TechTalk.SpecFlow;

namespace uk.co.nfocus.EcommerceBDDProject.StepDefinitions
{
    [Binding]
    public class CheckoutSystemStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;

        public CheckoutSystemStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        //----- Background -----
        [Given(@"we are logged in")]
        public void GivenWeAreLoggedIn()
        {
            //TODO > Login to account
            _scenarioContext.Pending();
        }

        [Given(@"we are on the shop page")]
        public void GivenWeAreOnTheShopPage()
        {
            //TODO > Go to shop page
            _scenarioContext.Pending();
        }


        //----- Testcase 1 -----
        [Given(@"we add '([^']*)' of '([^']*)' to the cart")]
        public void GivenWeAddOfToTheCart(string quantity, string item)
        {
            //TODO > Add the given quantity of each item to the cart
            Console.WriteLine($" Provide a quantity {quantity} of item {item}");
            _scenarioContext.Pending();
        }

        [Given(@"we are viewing the cart page")]
        public void GivenWeAreViewingTheCartPage()
        {
            //TODO > Go to the cart page
            _scenarioContext.Pending();
        }

        [When(@"a (.*)% discount code '([^']*)' is applied")]
        public void WhenADiscountCodeIsApplied(int p0, string edgewords)
        {
            //TODO > Apply the discount code
            _scenarioContext.Pending();
        }

        [Then(@"the correct amount is subtracted from the total")]
        public void ThenTheCorrectAmountIsSubtractedFromTheTotal()
        {
            //TODO > Read subtotal, total, shipping, and subtracted discount from page
            //TODO > Calculate actual discount amount applied
            //TODO > Compare with expected
            _scenarioContext.Pending();
        }

        //----- Testcase 2 -----
        [Given(@"we have items in the cart")]
        public void GivenWeHaveItemsInTheCart()
        {
            //TODO > Add an item to the cart
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
