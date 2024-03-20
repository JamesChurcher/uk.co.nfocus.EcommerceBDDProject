using OpenQA.Selenium;
using uk.co.nfocus.EcommerceBDDProject.Support;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class OrderListPagePOM
    {
        private IWebDriver _driver;

        public OrderListPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("my-account/orders"),
                        "Not on the account order list page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private IReadOnlyList<IWebElement> _allOrderNumbers => _driver.FindElements(By.PartialLinkText("#"));


        //----- Service methods -----

        //Get order numbers for all of my orders
        //  Params  -> orderNumber: The order number to check
        //  Returns -> (bool) if order is listed under the account
        public bool CheckIfOrderInOrderNumbers(string orderNumber)
        {
            var orderNumbers = _allOrderNumbers;

            foreach (var order in orderNumbers)
            {
                if (order.Text.Contains(orderNumber))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
