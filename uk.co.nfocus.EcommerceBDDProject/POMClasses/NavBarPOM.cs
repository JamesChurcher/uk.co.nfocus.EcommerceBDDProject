using OpenQA.Selenium;
using uk.co.nfocus.EcommerceBDDProject.Support;
using uk.co.nfocus.EcommerceBDDProject.Utilities;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class NavBarPOM
    {
        private IWebDriver _driver;

        public NavBarPOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.DoesNotThrow(() => _driver.FindElement(By.LinkText("nFocus Shop")),  //May switch between "nFocus Shop" and "Edgewords Shop"
                                "Not on any edgewords shop webpage or navbar is unavailable");    //Verify we are on the correct website
        }

        //----- Locators -----
        private IWebElement _shopButton => _driver.FindElement(By.LinkText("Shop"));
        private IWebElement _cartButton => _driver.FindElement(By.LinkText("Cart"));
        private IWebElement _checkoutButton => _driver.FindElement(By.LinkText("Checkout"));
        private IWebElement _accountButton => _driver.FindElement(By.LinkText("My account"));
        private IWebElement _blogButton => _driver.FindElement(By.LinkText("Blog"));
        private IWebElement _dismissButton => _driver.FindElement(By.LinkText("Dismiss"));

        private By _addToCartButton = By.LinkText("Add to cart");

        //----- Service methods -----

        // Go to shop page
        public void GoShop()
        {
            _shopButton.Click();
            _driver.WaitUntilElDisplayed(_addToCartButton);  //Wait until shop page has loaded
        }

        // Go to cart page
        public void GoCart()
        {
            _cartButton.Click();
            _driver.WaitUntilUrlSubstring("cart");   //Wait for cart page to load
        }

        // Go to checkout page
        public void GoCheckout()
        {
            _checkoutButton.Click();
            _driver.WaitUntilUrlSubstring("checkout");   //Wait for checkout page to load
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
            _driver.WaitUntilUrlSubstring("my-account"); //Wait for account page to load
        }

        // Go to blog page
        public void GoBlog()
        {
            _blogButton.Click();
            _driver.WaitUntilUrlSubstring("blog");   //Wait for blog page to load
        }

        //Dismiss popup button
        public void DismissPopup()
        {
            _dismissButton.Click();
        }
    }
}
