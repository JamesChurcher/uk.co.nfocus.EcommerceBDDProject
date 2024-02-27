using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.Utilities;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class OrderListPagePOM
    {
        private IWebDriver _driver;

        public OrderListPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url, 
                        Does.Contain("my-account/orders"), 
                        "Not on the account order list page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private IReadOnlyList<IWebElement> _allOrderNumbers => _driver.FindElements(By.PartialLinkText("#"));


        //----- Service methods -----


        //----- Higher level helpers -----

        //Get order numbers for all of my orders
        //  Params  -> orderNumber: The order number to check
        //  Returns -> (bool) if order is listed under the account
        public bool CheckIfOrderInOrderNumbers(string orderNumber)
        {
            var orderNumbers = _allOrderNumbers;

            foreach(var order in orderNumbers)
            {
                //Console.WriteLine($"Does current order {order.Text} contain {orderNumber}");
                if (order.Text.Contains(orderNumber))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
