using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static uk.co.nfocus.ecommerce_mini_project.Utilities.TestHelper;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class ShopPagePOM
    {
        private IWebDriver _driver;

        public ShopPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("shop"),
                        "Not on the shop page");   //Verify we are on the correct page
        }

        //Locators
        //private IWebElement _usernameField => _driver.FindElement(By.LinkText("View cart"));
        private IReadOnlyList<IWebElement> _addToCartButtons => _driver.FindElements(By.LinkText("Add to cart"));
        
        //Service methods

        //Click the add to basket button
        public void ClickAddToBasket()
        {
            //_driver.FindElement(By.LinkText("Add to cart")).Click();

            _addToCartButtons[0].Click();
            WaitForElDisplayed(_driver, By.LinkText("View cart"));  //Wait for confirmation of cart addition
        }
    }
}
