using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.ecommerce_mini_project.Utilities;
using static uk.co.nfocus.ecommerce_mini_project.Utilities.TestHelper;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class CartPagePOM
    {
        private IWebDriver _driver;

        public CartPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("cart"),
                        "Not in the cart / on cart page");   //Verify we are on the correct page
        }

        //Locators
        private IWebElement _discountCodeField => _driver.FindElement(By.Id("coupon_code"));
        //private IWebElement _discountCodeField
        //{
        //    get
        //    {
        //        WaitForElDisplayed(_driver, By.Id("coupon_code"));
        //        return _driver.FindElement(By.Id("coupon_code"));
        //    }
        //}
        private IWebElement _applyDiscountButton => _driver.FindElement(By.Name("apply_coupon"));
        private IWebElement _removeFromCartButton => _driver.FindElement(By.ClassName("remove"));
        private IReadOnlyList<IWebElement> _removeFromCartButtons => _driver.FindElements(By.ClassName("remove"));
        private IWebElement _removeDiscountButton => _driver.FindElement(By.LinkText("[Remove]"));

        private IWebElement _cartDiscountLabel => _driver.FindElement(By.CssSelector(".cart-discount .amount"));
        //private IWebElement _cartDiscountLabel
        //{
        //    get
        //    {
        //        WaitForElDisplayed(_driver, By.CssSelector(".cart-discount .amount"));
        //        return _driver.FindElement(By.CssSelector(".cart-discount .amount"));
        //    }
        //}
        private IWebElement _cartTotalLabel => _driver.FindElement(By.CssSelector(".order-total bdi"));
        private IWebElement _cartSubtotalLabel => _driver.FindElement(By.CssSelector(".cart-subtotal bdi"));
        private IWebElement _cartShippingCostLabel => _driver.FindElement(By.CssSelector(".shipping bdi"));

        //Service methods

        //Enter discount code
        public CartPagePOM SetDiscountCode(string code)
        {
            WaitForElDisplayed(_driver, _discountCodeField);
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
            WaitForElDisplayed(_driver, _cartDiscountLabel);
            return StringToDecimal(_cartDiscountLabel.Text);
        }

        //Get the cart subtotal and format as a decimal type
        public decimal GetCartSubtotal()
        {
            WaitForElDisplayed(_driver, _cartSubtotalLabel);
            return StringToDecimal(_cartSubtotalLabel.Text);
        }

        //Get the cart total and format as a decimal type
        public decimal GetCartTotal()
        {
            WaitForElDisplayed(_driver, _cartTotalLabel);
            return StringToDecimal(_cartTotalLabel.Text);
        }

        //Get the shipping cost and format as a decimal type
        public decimal GetShippingCost()
        {
            WaitForElDisplayed(_driver, _cartShippingCostLabel);
            return StringToDecimal(_cartShippingCostLabel.Text);
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
            ClickRemoveDiscountButton();

            //Wait until discount is no longer applied
            new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => (drv.FindElements(By.LinkText("[Remove]")).Count == 0));

            // Loop over the remove button for every item in the cart
            for(int i=_removeFromCartButtons.Count; i>0; i--)
            {
                _removeFromCartButtons[0].Click();
            }

            WaitForElDisplayed(_driver, By.ClassName("cart-empty"));  //Wait for empty cart to be loaded
        }
    }
}
