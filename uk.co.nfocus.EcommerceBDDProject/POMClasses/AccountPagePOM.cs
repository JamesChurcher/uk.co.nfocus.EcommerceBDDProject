using OpenQA.Selenium;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class AccountPagePOM
    {
        private IWebDriver _driver;

        public AccountPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("my-account"),
                        "Not on the account page");   //Verify we are on the correct page
        }

        //----- Locators -----
        private IWebElement _usernameField => _driver.FindElement(By.Id("username"));
        private IWebElement _passwordField => _driver.FindElement(By.Id("password"));

        private By _loginButtonLocator = By.Name("login");
        private IWebElement _loginButton => _driver.FindElement(_loginButtonLocator);

        private By _logoutButtonLocator = By.LinkText("Logout");
        private IWebElement _logoutButton => _driver.FindElement(_logoutButtonLocator);
        private IWebElement _ordersButton => _driver.FindElement(By.LinkText("Orders"));


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
            _loginButton.Click();
        }

        //Logout by clicking the "logout" button
        public void ClickLogout()
        {
            _logoutButton.Click();
        }

        //Check my orders
        public void GoOrders()
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
            WaitForElDisplayed(_driver, _loginButtonLocator);   //Wait to ensure login button has loaded

            //Login steps
            SetUsername(username);
            SetPassword(password);
            SubmitLoginForm();

            try
            {
                WaitForElDisplayed(_driver, _logoutButtonLocator);  //Wait until login has completed
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
            WaitForElDisplayed(_driver, _logoutButtonLocator);  //Wait to ensure logout button has loaded

            //Logout steps
            ClickLogout();

            try
            {
                WaitForElDisplayed(_driver, _loginButtonLocator);  //Wait until logout has completed
                return true;    //Logout success
            }
            catch (NoSuchElementException)
            {
                return false;   //Failed logout
            }
        }
    }
}
