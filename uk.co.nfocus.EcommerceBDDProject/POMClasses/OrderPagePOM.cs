using OpenQA.Selenium;
using uk.co.nfocus.EcommerceBDDProject.Support;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class OrderPagePOM
    {
        private IWebDriver _driver;

        public OrderPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("order"),
                        "Did not reach order summary page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private IWebElement _orderNumberLabel => _driver.FindElement(By.ClassName("order"));

        //----- Service methods -----
        public string GetOrderNumber()
        {
            var orderNumber = _orderNumberLabel.Text;
            return orderNumber.Substring(orderNumber.IndexOf("\n") + 1);
        }
    }
}
