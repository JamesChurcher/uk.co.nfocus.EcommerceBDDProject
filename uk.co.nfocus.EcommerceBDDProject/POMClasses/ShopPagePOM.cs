using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
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

            GetProductElements();
        }

        //----- Locators -----
        //private By _viewCartButtonLocator => By.LinkText("View cart");
        private IWebElement _cartItemCountLabel => _driver.FindElement(By.ClassName("count"));
        private IReadOnlyList<IWebElement> _addToCartButtons => _driver.FindElements(By.LinkText("Add to cart"));
        private Dictionary<string, IWebElement> _productToElement;

        //----- Service methods -----

        //Click the add to basket button
        //public void ClickAddToBasket()
        //{
        //    //_driver.FindElement(By.LinkText("Add to cart")).Click();

        //    //_addToCartButtons[0].Click();
        //    foreach (IWebElement element in _addToCartButtons)
        //    {
        //        Console.WriteLine("Item on shop page: " + element.GetAttribute("aria-label"));
        //    }
        //    //WaitForElDisplayed(_driver, By.LinkText("View cart"));  //Wait for confirmation of cart addition
        //}

        //Click the add to basket button
        public void ClickAddToBasket(string productName)
        {
            int count = StringToInt(_cartItemCountLabel.Text);

            IWebElement element = _productToElement[productName];
            element.Click();
            count++;

            new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => count == StringToInt(_cartItemCountLabel.Text));
            //Console.WriteLine("Cart count is " + _cartItemCountLabel.Text);
        }

        //Create a dictionary of all product names paired with the button element that will add it to the cart
        private void GetProductElements()
        {
            //Get all elements that represent a product on the page
            IReadOnlyList<IWebElement> products = _driver.FindElements(By.ClassName("product"));

            //Create a dictionary to hold pairings between product name and button to add to basket
            _productToElement = new();

            //Loop over products and create pairings between product name and add to cart button element
            foreach (IWebElement element in products)
            {
                string name = element.FindElement(By.TagName("h2")).Text;
                IWebElement button = element.FindElement(By.LinkText("Add to cart"));

                _productToElement.Add(name, button);
            }

            //Output pairings - for testing purposes
            //foreach (KeyValuePair<string, IWebElement> pair in _productToElement)
            //{
            //    Console.WriteLine($"I found a pair {pair.Key} and {pair.Value.GetAttribute("href")} button");
            //}
        }

        //Get the product names of all products on the shop page
        //  Returns -> (string[]) array of all products listed on the shop
        public string[] GetProductNames()
        {
            var keys = _productToElement.Keys;
            string[] names = new string[keys.Count];

            keys.CopyTo(names, 0);

            return names;
        }
    }
}
