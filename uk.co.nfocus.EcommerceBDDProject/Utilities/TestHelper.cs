using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using TechTalk.SpecFlow.Infrastructure;

namespace uk.co.nfocus.EcommerceBDDProject.Utilities
{
    internal static class TestHelper
    {
        //private static string _screenshotPath = @"C:\Users\JamesChurcher\OneDrive - nFocus Limited\Pictures\Screenshots\";
        //public static string ScreenshotPath => _screenshotPath;
        //private static string _screenshotPath = Directory.GetCurrentDirectory() + @"\..\..\..\Screenshots\";
        private static string _screenshotPath = new Uri(Directory.GetCurrentDirectory() + @"\..\..\..\Screenshots\").AbsolutePath;

        //Enums for payment methods
        public enum PaymentMethod
        {
            cheque,
            cod
        }

        //----- Waits -----
        // Get a new driver wait object
        public static WebDriverWait GetWaitObject(IWebDriver driver, int timeout=4)
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

        ////WARNING - does not work, actualNum should be an expression that is evaluated constantly during wait until
        ////          but it is evaluated once on function call and therefore times out. Can you pass expression by reference?
        ////Explicit wait for change in value
        //public static void WaitForValueChange(IWebDriver driver, int expectedNum, int actualNum)
        //{
        //    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
        //    wait.Until(drv => expectedNum == actualNum);
        //}


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

        // Take screenshot
        public static string TakeScreenshot(IWebDriver driver, string name, string description)
        {
            var path = _screenshotPath + name + ".png";

            ITakesScreenshot ssdriver = driver as ITakesScreenshot;
            ssdriver.GetScreenshot().SaveAsFile(path);

            TestContext.AddTestAttachment(path, description);

            return path;
        }

        // Scroll the given element into view
        public static void ScrollToElement(IWebDriver driver, IWebElement element)
        {
            // Check if the driver supports JS
            IJavaScriptExecutor? jsdriver = driver as IJavaScriptExecutor;

            if(jsdriver != null)
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

        // Output to Console and Livingdoc api
        public static void WriteLine(string text, ISpecFlowOutputHelper logger)
        {
            Console.WriteLine(text);
            logger.WriteLine(text);
        }


        // Get only the numerical characters from the text of a located web element
        // Returns integer
        //public static int EleToInt(IWebDriver driver, By locator)
        //{
        //    var text = driver.FindElement(locator).Text;
        //    return StringToInt(text);
        //}

        //public static void SaveAndAttachScreenShot(Screenshot screenshot, string name, string description=null)
        //{
        //    screenshot.SaveAsFile($"{_screenshotPath}{name}.png");
        //    TestContext.AddTestAttachment($"{_screenshotPath}{name}.png", description);
        //}
    }
}
