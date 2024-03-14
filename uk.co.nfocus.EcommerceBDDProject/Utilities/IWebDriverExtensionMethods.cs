using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace uk.co.nfocus.EcommerceBDDProject.Utilities
{
    internal static class IWebDriverExtensionMethods
    {
        // Get a new driver wait object
        public static WebDriverWait NewWaitObject(this IWebDriver driver, int timeout = 4)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

        // Explicit wait for element to be displayed
        public static void WaitUntilElDisplayed(this IWebDriver driver, By locator)
        {
            WebDriverWait wait = driver.NewWaitObject();
            wait.Until(drv => drv.FindElement(locator).Displayed);
        }

        //Explicit wait for url to contain substring
        public static void WaitUntilUrlSubstring(this IWebDriver driver, string urlSubstring)
        {
            WebDriverWait wait = driver.NewWaitObject();
            wait.Until(drv => drv.Url.Contains(urlSubstring));
        }
    }
}
