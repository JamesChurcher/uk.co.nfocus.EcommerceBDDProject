using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uk.co.nfocus.EcommerceBDDProject.POMClasses;
using uk.co.nfocus.EcommerceBDDProject.Support;

namespace uk.co.nfocus.EcommerceBDDProject.Support
{
    [Binding]
    internal class Hooks
    {
        private readonly ScenarioContext _scenarioContext;

        //private IWebDriver _driver;

        private WebDriverWrapper _driverWrapper;

        public Hooks(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;
        }

        [Before]
        public void Setup()
        {
            // Get environment variables
            string browser = Environment.GetEnvironmentVariable("BROWSER");
            Console.WriteLine($"Browser is set to: {browser}");

            //string webUrl = TestContext.Parameters["WebAppUrl"];
            //Console.WriteLine("The website url is " + webUrl);

            // Default to Edge if browser env is null
            if (browser == null)
            {
                browser = "edge";
                Console.WriteLine("BROWSER env not set: Setting default to edge");
            }

            //Instantiate a browser based on variable
            switch (browser)
            {
                case "firefox":
                    _driverWrapper.Driver = new FirefoxDriver();
                    break;
                case "chrome":
                    ChromeOptions options = new ChromeOptions();
                    options.BrowserVersion = "canary"; //stable/beta/dev/canary/num
                    _driverWrapper.Driver = new ChromeDriver(options);
                    break;
                default:
                    _driverWrapper.Driver = new EdgeDriver();
                    break;
            }

            //_scenarioContext["NewDriver"] = _driverWrapper.Driver;

            // Go to shop url
            _driverWrapper.Driver.Navigate().GoToUrl("https://www.edgewordstraining.co.uk/demo-site/");
            Console.WriteLine("Navigated to site");

            // Dismiss popup
            _driverWrapper.Driver.FindElement(By.LinkText("Dismiss")).Click();    //TODO > Move to POM class or wrapper
        }

        [After]
        public void Teardown()
        {
            //Get navbar object from session
            NavBarPOM navbar = (NavBarPOM)_scenarioContext["NavBarPOMObject"];

            //Navigate back to the cart to clear it
            navbar.GoCart();

            CartPagePOM cartPage = new(_driverWrapper);

            //Remove the discount and products if they exist
            cartPage.MakeCartEmpty();
            Console.WriteLine("Removed items from cart");

            //Navigate to my account to log out
            navbar.GoAccount();

            //TODO, figure out why driver thinks it has pressed button when it has not
            // Logout
            AccountPagePOM accountPage = new(_driverWrapper);
            bool logoutStatus = accountPage.LogoutExpectSuccess();
            Assert.That(logoutStatus, "Could not logout");   //Verify successful logout

            Console.WriteLine("Logout from account");

            Console.WriteLine("--Test Complete!--");

            // Quit and dispose of driver
            _driverWrapper.Driver.Quit();
        }
    }
}
