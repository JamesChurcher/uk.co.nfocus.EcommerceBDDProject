using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using TechTalk.SpecFlow.Infrastructure;
using uk.co.nfocus.EcommerceBDDProject.POMClasses;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.Support
{
    [Binding]
    internal class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private WebDriverWrapper _driverWrapper;
        private readonly ISpecFlowOutputHelper _outputHelper;

        public Hooks(ScenarioContext scenarioContext, WebDriverWrapper driverWrapper, ISpecFlowOutputHelper outputHelper)
        {
            _scenarioContext = scenarioContext;
            _driverWrapper = driverWrapper;
            _outputHelper = outputHelper;
        }

        [Before]
        public void Setup()
        {
            // Get environment variables
            // Get username and password and make available during test
            string username = TestContext.Parameters["Username"];
            string password = TestContext.Parameters["Password"];

            // Check if runfile contains usernme and password
            if (username == null || password == null)
            {
                throw new NotFoundException("Could not set Username and Password, env variables not found");
            }
            else
            {
                _outputHelper.WriteLine("Username and Password have been set");
            }

            // Pass username and password to steps
            _scenarioContext["Username"] = username;
            _scenarioContext["Password"] = password;

            // Get browser version to use
            string browser = Environment.GetEnvironmentVariable("BROWSER");
            _outputHelper.WriteLine($"Browser is set to: {browser}");

            // Default to Edge if browser env is null
            if (string.IsNullOrEmpty(browser))
            {
                browser = "edge";
                _outputHelper.WriteLine("BROWSER env not set: Setting default to edge");
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

            // Get if screenshots should be taken
            ScreenshotToggle screenshotToggle;
            bool parseStatus = Enum.TryParse<ScreenshotToggle>(Environment.GetEnvironmentVariable("SCREENSHOTTOGGLE"), out screenshotToggle);
            _outputHelper.WriteLine("Screenshots toggle set to: " + TestContext.Parameters["ScreenshotToggle"]);

            // Set default toggle if null
            if (!parseStatus)
            {
                screenshotToggle = ScreenshotToggle.All;
                _outputHelper.WriteLine("ScreenshotsToggle param not set: Setting default to All");
            }

            _scenarioContext["ScreenshotToggle"] = screenshotToggle;

            // Get the url of the website
            string webUrl = TestContext.Parameters["WebAppUrl"];
            _outputHelper.WriteLine("The website url is " + webUrl);

            // Go to shop url
            _driverWrapper.Driver.Navigate().GoToUrl(webUrl);
            _outputHelper.WriteLine("Navigated to site");
        }

        [After]
        public void Teardown()
        {
            //Get navbar object from session
            NavBarPOM navbar = (NavBarPOM)_scenarioContext["NavBarPOMObject"];
            //NavBarPOM navbar = new(_driverWrapper);

            //Navigate back to the cart to clear it
            navbar.GoCart();

            CartPagePOM cartPage = new(_driverWrapper);

            //Remove the discount and products if they exist
            cartPage.MakeCartEmpty();
            _outputHelper.WriteLine("Removed items from cart");

            //Navigate to my account to log out
            navbar.GoAccount();

            // Logout
            AccountPagePOM accountPage = new(_driverWrapper);
            bool logoutStatus = accountPage.LogoutExpectSuccess();
            Assert.That(logoutStatus, "Could not logout");   //Verify successful logout

            _outputHelper.WriteLine("Logout from account");

            _outputHelper.WriteLine("--Test Complete!--");

            // Quit and dispose of driver
            _driverWrapper.Driver.Quit();
        }
    }
}
