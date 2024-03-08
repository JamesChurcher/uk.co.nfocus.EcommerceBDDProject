using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class NavBarPOM
    {
        private IWebDriver _driver;

        public NavBarPOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.FindElement(By.LinkText("nFocus Shop")),
                        Is.Not.Null, 
                        "Not in the edgewords shop or navbar not available");    //Verify we are on the correct website
        }

        //----- Locators -----
        private IWebElement _homeButton => _driver.FindElement(By.LinkText("Home"));
        private IWebElement _shopButton => _driver.FindElement(By.LinkText("Shop"));
        private IWebElement _cartButton => _driver.FindElement(By.LinkText("Cart"));
        private IWebElement _checkoutButton => _driver.FindElement(By.LinkText("Checkout"));
        private IWebElement _accountButton => _driver.FindElement(By.LinkText("My account"));
        private IWebElement _blogButton => _driver.FindElement(By.LinkText("Blog"));
        private IWebElement _dismissButton => _driver.FindElement(By.LinkText("Dismiss"));

        private By _addToCartButton = By.LinkText("Add to cart");

        //----- Service methods -----

        //// Go to home page
        //public void GoHome()
        //{
        //    _homeButton.Click();
        //    //TODO, wait on something else since every page has entry-title
        //    WaitForElDisplayed(_driver, By.ClassName("entry-title"));   //Wait for home page to load
        //}

        // Go to shop page
        public void GoShop()
        {
            _shopButton.Click();
            WaitForElDisplayed(_driver, _addToCartButton);  //Wait until shop page has loaded
        }

        // Go to cart page
        public void GoCart()
        {
            _cartButton.Click();
            WaitForUrlSubstring(_driver, "cart");   //Wait for cart page to load
        }

        // Go to checkout page
        public void GoCheckout()
        {
            _checkoutButton.Click();
            WaitForUrlSubstring(_driver, "checkout");   //Wait for checkout page to load
        }

        // Go to account page
        public void GoAccount()
        {
            //Loop over button click - sometimes logout can be flakey
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _accountButton.Click();
                    if (_driver.Url.Contains("my-account"))
                    {
                        break;
                    }
                }
                catch (ElementClickInterceptedException)
                {
                    //Do nothing - Catch interception error if account click fails
                }

                Thread.Sleep(50);
            }
            WaitForUrlSubstring(_driver, "my-account"); //Wait for account page to load
        }

        // Go to blog page
        public void GoBlog()
        {
            _blogButton.Click();
            WaitForUrlSubstring(_driver, "blog");   //Wait for blog page to load
        }

        //Dismiss popup button
        public void DismissPopup()
        {
            _dismissButton.Click();
        }
    }
}
