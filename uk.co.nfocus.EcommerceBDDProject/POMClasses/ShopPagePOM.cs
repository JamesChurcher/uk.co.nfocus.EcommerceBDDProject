using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class ShopPagePOM
    {
        private IWebDriver _driver;
        private Dictionary<string, IWebElement> _productToElementDict;

        public ShopPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("shop"),
                        "Not on the shop page");   //Verify we are on the correct page

            GetProductElements();
        }

        //----- Locators -----
        //private By _viewCartButtonLocator => By.LinkText("View cart");
        private IWebElement _cartItemCountLabel => _driver.FindElement(By.ClassName("count"));
        //private IReadOnlyList<IWebElement> _addToCartButtons => _driver.FindElements(By.LinkText("Add to cart"));

        private IReadOnlyList<IWebElement> _productsInShopElement => _driver.FindElements(By.ClassName("product"));
        private By _productName = By.TagName("h2");
        private By _productAddToCartButton = By.LinkText("Add to cart");


        //----- Service methods -----

        //Add the given product to cart by clicking the respective "Add to cart" button
        //  Param -> productName: The product to add to the cart
        public void ClickAddToBasket(string productName)
        {
            //Count of how many items are in basket
            int count = StringToInt(_cartItemCountLabel.Text);

            IWebElement element = _productToElementDict[productName];
            element.Click();
            count++;

            //Wait until basket has registered new item and count has incremented
            GetWaitObject(_driver).Until(drv => count == StringToInt(_cartItemCountLabel.Text));
            //WaitForValueChange(_driver, count, StringToInt(_cartItemCountLabel.Text));
            //Console.WriteLine("Cart count is " + _cartItemCountLabel.Text);
        }

        //Create a dictionary of all product names paired with the button element that will add it to the cart
        private void GetProductElements()
        {
            //Get all elements that represent a product on the page
            IReadOnlyList<IWebElement> products = _productsInShopElement;

            //Create a dictionary to hold pairings between product name and button to add to basket
            _productToElementDict = new();

            //Loop over products and create pairings between product name and add to cart button element
            foreach (IWebElement element in products)
            {
                string name = element.FindElement(_productName).Text;
                IWebElement button = element.FindElement(_productAddToCartButton);

                _productToElementDict.Add(name, button);
            }
        }

        //Get the product names of all products on the shop page
        //  Returns -> (string[]) array of all products listed on the shop
        public string[] GetProductNames()
        {
            var keys = _productToElementDict.Keys;
            string[] names = new string[keys.Count];

            keys.CopyTo(names, 0);

            return names;
        }
    }
}
