using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.Support;
using uk.co.nfocus.EcommerceBDDProject.Utilities;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class CartPagePOM
    {
        private IWebDriver _driver;

        public CartPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("cart"),
                        "Not in the cart / on cart page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private By _discountCodeLocator = By.Id("coupon_code");
        private IWebElement _discountCodeField => _driver.FindElement(By.Id("coupon_code"));
        private IWebElement _applyDiscountButton => _driver.FindElement(By.Name("apply_coupon"));
        private IWebElement _removeFromCartButton => _driver.FindElement(By.ClassName("remove"));
        private IReadOnlyList<IWebElement> _removeFromCartButtons => _driver.FindElements(By.ClassName("remove"));
        private IWebElement _removeDiscountButton => _driver.FindElement(By.LinkText("[Remove]"));

        private By _cartDiscountLocator = By.CssSelector(".cart-discount .amount");
        private By _cartTotalLocator = By.CssSelector(".order-total bdi");
        private By _cartSubtotalLocator = By.CssSelector(".cart-subtotal bdi");
        private By _cartShippingCostLocator = By.CssSelector(".shipping bdi");
        private IWebElement _cartDiscountLabel => _driver.FindElement(_cartDiscountLocator);
        private IWebElement _cartTotalLabel => _driver.FindElement(_cartTotalLocator);
        private IWebElement _cartSubtotalLabel => _driver.FindElement(_cartSubtotalLocator);
        private IWebElement _cartShippingCostLabel => _driver.FindElement(_cartShippingCostLocator);

        private IWebElement _bannerMessage => _driver.FindElement(By.ClassName("woocommerce-message"));
        //private IReadOnlyList<IWebElement> _removeFromCartButtons => _driver.FindElements(By.CssSelector(".product-remove > a"));

        //----- Service methods -----

        //Enter discount code
        public CartPagePOM SetDiscountCode(string code)
        {
            WaitForElDisplayed(_driver, _discountCodeLocator);
            _discountCodeField.Clear();
            _discountCodeField.SendKeys(code);
            return this;
        }

        //Submit the discount
        public void SubmitDiscountForm()
        {
            _applyDiscountButton.Click();
        }

        //Remove discount from cart
        public void ClickRemoveDiscountButton()
        {
            _removeDiscountButton.Click();
        }
        
        //Remove the top item from the cart
        public void ClickRemoveItemButton()
        {
            _removeFromCartButton.Click();
        }

        //Get the cost removed by the discount and format as a decimal type
        public decimal GetAppliedDiscount()
        {
            //WaitForElDisplayed(_driver, _cartDiscountLabel);    //TODO remove
            WaitForElDisplayed(_driver, _cartDiscountLocator);
            return StringToDecimal(_cartDiscountLabel.Text);
        }

        //Get the cart subtotal and format as a decimal type
        public decimal GetCartSubtotal()
        {
            //WaitForElDisplayed(_driver, _cartSubtotalLabel);    //TODO remove
            WaitForElDisplayed(_driver, _cartSubtotalLocator);
            return StringToDecimal(_cartSubtotalLabel.Text);
        }

        //Get the cart total and format as a decimal type
        public decimal GetCartTotal()
        {
            //WaitForElDisplayed(_driver, _cartTotalLabel);   //TODO remove
            WaitForElDisplayed(_driver, _cartTotalLocator);
            return StringToDecimal(_cartTotalLabel.Text);
        }

        //Get the shipping cost and format as a decimal type
        public decimal GetShippingCost()
        {
            //WaitForElDisplayed(_driver, _cartShippingCostLabel);    //TODO remove
            WaitForElDisplayed(_driver, _cartShippingCostLocator);
            return StringToDecimal(_cartShippingCostLabel.Text);
        }

        //Get banner message text
        public string GetBannerMessageText()
        {
            try
            {
                return _bannerMessage.Text;
            }
            catch (NoSuchElementException)
            {
                return "";
            }
        }

        //Does banner message text contain substring
        public bool DoesBannerMessageContain(string substring)
        {
            try
            {
                Console.WriteLine($"Does {_bannerMessage.Text} contain the string {substring} ?");

                bool contains = _bannerMessage.Text.Contains(substring);
                return contains;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        //Wait for banner message to contain text
        public void WaitUntilBannerMessageContains(string substring)
        {
            new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => DoesBannerMessageContain(substring));
        }

        //Highlevel service methods

        //Applied the given discount code to the current cart
        //  Params  -> discountCode: The discount code to apply
        //  Returns -> (bool) if discount code was applied successfully
        public bool ApplyDiscountExpectSuccess(string discountCode)
        {
            SetDiscountCode(discountCode);
            SubmitDiscountForm();

            try
            {
                WaitForElDisplayed(_driver, By.LinkText("[Remove]"));  //Wait until discount has been applied
                return true;    //Coupon applied
            }
            catch (NoSuchElementException)
            {
                return false;   //Coupon rejected
            }
        }

        //Remove the discount and items from cart
        public void MakeCartEmpty()
        {
            //Remove discount
            try
            {
                ClickRemoveDiscountButton();
            }
            catch (NoSuchElementException)
            {
                //If there was no discount applied then do nothing
                Console.WriteLine("No discount applied to cart");
            }

            //Wait until the remove discount link is gone
            new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => 0 == _driver.FindElements(By.LinkText("[Remove]")).Count); //TODO, make helper method
            //WaitForValueChange(_driver, 0, _driver.FindElements(By.LinkText("[Remove]")).Count);

            int count = _removeFromCartButtons.Count;
            for (int i = count; i > 0; i--)     //Loop for every remove product button in the cart
            {
                _removeFromCartButtons[0].Click(); //Remove the top product
                count--;

                //Wait until the number of remove product buttons has decreased by 1
                new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => count == _removeFromCartButtons.Count);    //TODO, make helper method
                //WaitForValueChange(_driver, count, _removeFromCartButtons.Count);
            }

            WaitForElDisplayed(_driver, By.ClassName("cart-empty"));  //Wait for empty cart to be loaded
        }
    }
}
