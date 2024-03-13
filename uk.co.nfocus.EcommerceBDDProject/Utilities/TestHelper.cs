using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Infrastructure;
using uk.co.nfocus.EcommerceBDDProject.Support;

namespace uk.co.nfocus.EcommerceBDDProject.Utilities
{
    internal static class TestHelper
    {
        private static string _screenshotPath = new Uri(Directory.GetCurrentDirectory() + @"\..\..\..\Screenshots\").AbsolutePath;

        //Enums for payment methods
        public enum PaymentMethod
        {
            cheque,
            cod
        }

        public enum ScreenshotToggle
        {
            All,
            //Failed,
            None
        }

        //----- Waits -----
        // Get a new driver wait object
        public static WebDriverWait GetWaitObject(IWebDriver driver, int timeout = 4)
        {
            return new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
        }

        // Explicit wait for element to be displayed
        public static void WaitForElDisplayed(IWebDriver driver, By locator)
        {
            WebDriverWait wait = GetWaitObject(driver);
            wait.Until(drv => drv.FindElement(locator).Displayed);
        }

        //Explicit wait for url to contain substring
        public static void WaitForUrlSubstring(IWebDriver driver, string urlSubstring)
        {
            WebDriverWait wait = GetWaitObject(driver);
            wait.Until(drv => drv.Url.Contains(urlSubstring));
        }

        //----- Type conversion helpers -----
        // Remove all non numerical characters from a string
        // Returns integer
        public static int StringToInt(string myString)
        {
            return int.Parse(Regex.Replace(myString, "[^0-9]", ""));
        }

        // Convert a string including currency symbols to a decimal
        // Returns decimal
        public static decimal StringToDecimal(string myString)
        {
            return Decimal.Parse(myString, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, new CultureInfo("en-GB"));
        }


        // Clears then sends string to given text field
        public static void ClearAndSendToTextField(IWebElement element, string myString)
        {
            element.Clear();
            element.SendKeys(myString);
        }


        //----- File management -----
        // Take screenshot
        public static string TakeScreenshot(IWebDriver driver, string name)
        {
            var path = _screenshotPath + name + ".png";

            ITakesScreenshot ssdriver = driver as ITakesScreenshot;
            ssdriver.GetScreenshot().SaveAsFile(path);

            return path;
        }

        // Scroll the given element into view
        public static void ScrollToElement(IWebDriver driver, IWebElement element)
        {
            // Check if the driver supports JS
            IJavaScriptExecutor? jsdriver = driver as IJavaScriptExecutor;

            if (jsdriver != null)
            {
                Thread.Sleep(1000);     // Could not get page to scroll without sleeping
                jsdriver.ExecuteScript("arguments[0].scrollIntoView(false)", element);   // Scroll to element
            }
        }

        // Create valid filename from test name
        public static string ValidFileNameFromTest(string text)
        {
            string myString = TestContext.CurrentContext.Test.Name + "_" + text;
            myString = myString.Replace("\"", "");
            return myString;
        }

        // Take screenshot and add to context
        public static void TakeScreenshotAndAddToContext(ScreenshotToggle screenshotToggle, WebDriverWrapper driverWrapper, ISpecFlowOutputHelper outputHelper, string name, string description)
        {
            if (screenshotToggle != ScreenshotToggle.All)
                return;

            string screenshotName = ValidFileNameFromTest(name);
            string imagePath = TakeScreenshot(driverWrapper.Driver, screenshotName);

            TestContext.AddTestAttachment(imagePath, description);
            outputHelper.AddAttachment(@"file:///" + imagePath);
        }
    }
}
