using OpenQA.Selenium;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;
using uk.co.nfocus.EcommerceBDDProject.Utilities;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class ShopPagePOM
    {
        private IWebDriver _driver;
        private Dictionary<string, IWebElement> _productToElementDict;  //Product name -> "Add to cart" button element

        public ShopPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("shop"),
                        "Not on the shop page");   //Verify we are on the correct page

            _productToElementDict = new();
            GetProductElements();   //Populate _productToElementDict dictionary, stores pairs of product names and their "Add to cart" button
        }

        //----- Locators -----
        private IWebElement _cartItemCountLabel => _driver.FindElement(By.ClassName("count"));
        private IReadOnlyList<IWebElement> _productsInShopElements => _driver.FindElements(By.ClassName("product"));
        private By _productName = By.TagName("h2");
        private By _productAddToCartButton = By.LinkText("Add to cart");


        //----- Service methods -----

        //Add the given product to cart by clicking the respective "Add to cart" button
        //  Param -> productName: The product to add to the cart
        public void ClickAddToBasket(string productName)
        {
            //Verify product is in dictionary
            if (!_productToElementDict.ContainsKey(productName))
            {
                throw new KeyNotFoundException($"Requested product \"{productName}\" was not found in the shop");
            }

            //Count how many items are in basket
            int count = StringToInt(_cartItemCountLabel.Text);

            _productToElementDict[productName].Click();

            //Wait until basket has registered new item and count has incremented
            _driver.NewWaitObject().Until(drv => count+1 == StringToInt(_cartItemCountLabel.Text));
        }

        //Create a dictionary of all product names paired with the button element that will add it to the cart
        //Populates the _productToElementDict dictionary
        private void GetProductElements()
        {
            _productToElementDict = new();

            //Loop over products on the page and make pairs between product name and the "Add to cart" button element
            foreach (IWebElement element in _productsInShopElements)
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
