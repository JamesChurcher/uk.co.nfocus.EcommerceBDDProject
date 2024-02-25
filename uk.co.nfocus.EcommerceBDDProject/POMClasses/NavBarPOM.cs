using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static uk.co.nfocus.ecommerce_mini_project.Utilities.TestHelper;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class NavBarPOM
    {
        private IWebDriver _driver;

        public NavBarPOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.FindElement(By.LinkText("Edgewords Shop")),
                        Is.Not.Null, 
                        "Not in the edgewords shop or navbar not available");    //Verify we are on the correct website
        }

        //Locators
        private IWebElement _homeButton => _driver.FindElement(By.LinkText("Home"));
        private IWebElement _shopButton => _driver.FindElement(By.LinkText("Shop"));
        private IWebElement _cartButton => _driver.FindElement(By.LinkText("Cart"));
        private IWebElement _checkoutButton => _driver.FindElement(By.LinkText("Checkout"));
        private IWebElement _accountButton => _driver.FindElement(By.LinkText("My account"));
        private IWebElement _blogButton => _driver.FindElement(By.LinkText("Blog"));

        //Service methods

        // Go to home page
        public void GoHome()
        {
            _homeButton.Click();
            WaitForElDisplayed(_driver, By.ClassName("entry-title"));   //Wait for login page to load
        }

        // Go to shop page
        public void GoShop()
        {
            _shopButton.Click();
            WaitForElDisplayed(_driver, By.LinkText("Add to cart"));  //Wait until shop page has loaded
        }

        // Go to cart page
        public void GoCart()
        {
            _cartButton.Click();
            WaitForUrlSubstring(_driver, "cart");   //Wait for login page to load
        }

        // Go to checkout page
        public void GoCheckout()
        {
            _checkoutButton.Click();
            WaitForUrlSubstring(_driver, "checkout");   //Wait for login page to load
        }

        // Go to account page
        public void GoAccount()
        {
            _accountButton.Click();
            WaitForUrlSubstring(_driver, "my-account"); //Wait for login page to load
        }

        // Go to blog page
        public void GoBlog()
        {
            _blogButton.Click();
            WaitForUrlSubstring(_driver, "blog");   //Wait for login page to load
        }
    }
}
