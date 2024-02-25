using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class OrderPagePOM
    {
        private IWebDriver _driver;

        public OrderPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("order"),
                        "Did not reach order summary page");   //Verify we are on the correct page
        }

        //Locators
        private IWebElement _orderNumberLabel => _driver.FindElement(By.ClassName("order"));

        //Service methods
        public string GetOrderNumber()
        {
            var orderNumber = _orderNumberLabel.Text;
            return orderNumber.Substring(orderNumber.IndexOf("\n")+1);
        }

        //Highlevel service methods
    }
}
