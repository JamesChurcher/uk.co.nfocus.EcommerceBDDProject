using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.Utilities;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class AccountPagePOM
    {
        private IWebDriver _driver;

        public AccountPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url, 
                        Does.Contain("my-account"), 
                        "Not on the account page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private IWebElement _usernameField => _driver.FindElement(By.Id("username"));
        private IWebElement _passwordField => _driver.FindElement(By.Id("password"));
        private IWebElement _submitFormButton => _driver.FindElement(By.Name("login"));     //TODO > Add waits
        private IWebElement _logoutButton => _driver.FindElement(By.LinkText("Logout"));    //TODO > Add waits
        private IWebElement _ordersButton => _driver.FindElement(By.LinkText("Orders"));
        private IReadOnlyList<IWebElement> _allOrderNumbers => _driver.FindElements(By.PartialLinkText("#"));


        //----- Service methods -----

        //Set username in the username field
        public AccountPagePOM SetUsername(string username)
        {
            _usernameField.SendKeys(username);
            return this;
        }

        //Set password in the password field
        public AccountPagePOM SetPassword(string password)
        {
            _passwordField.SendKeys(password);
            return this;
        }

        //Login by clicking the "login" button
        public void SubmitLoginForm()
        {
            _submitFormButton.Click();
        }

        //Logout by clicking the "logout" button
        public void ClickLogout()
        {
            _logoutButton.Click();
        }

        //Check my orders
        public void ClickOrders()
        {
            _ordersButton.Click();
            WaitForUrlSubstring(_driver, "my-account/orders");  //Wait for order summary page to show
        }


        //----- Higher level helpers -----

        //Login to account by providing username, password, and submitting form
        //  Params  -> username: Username, password: Password
        //  Returns -> (bool) if login was successful status
        public bool LoginExpectSuccess(string username, string password)
        {
            SetUsername(username);
            SetPassword(password);
            SubmitLoginForm();

            try
            {
                WaitForElDisplayed(_driver, By.LinkText("Logout"));  //Wait until login has completed
                return true;    //Login success
            }
            catch (NoSuchElementException)
            {
                return false;   //Failed login
            }
        }

        //Logout of the currently logged in account
        //  Returns -> (bool) if logout was successful status
        public bool LogoutExpectSuccess()
        {
            ClickLogout();

            try
            {
                WaitForElDisplayed(_driver, By.Name("login"));  //Wait until logout has completed
                return true;    //Logout success
            }
            catch (NoSuchElementException)
            {
                return false;   //Failed logout
            }
        }

        //Get order numbers for all of my orders
        //  Params  -> orderNumber: The order number to check
        //  Returns -> (bool) if order is listed under the account
        public bool CheckIfOrderInOrderNumbers(string orderNumber)
        {
            ClickOrders();
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
