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

namespace uk.co.nfocus.ecommerce_mini_project.Utilities
{
    internal static class TestHelper
    {
        //private static string _screenshotPath = @"C:\Users\JamesChurcher\OneDrive - nFocus Limited\Pictures\Screenshots\";
        //public static string ScreenshotPath => _screenshotPath;
        private static string _screenshotPath = Directory.GetCurrentDirectory() + @"\..\..\..\Screenshots\";

        //Enums for payment methods
        public enum PaymentMethod
        {
            cheque,
            cod
        }

        // Explicit wait for element to be displayed
        public static void WaitForElDisplayed(IWebDriver driver, By locator)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(drv => drv.FindElement(locator).Displayed);
        }

        //Explicit wait for url to contain substring
        public static void WaitForUrlSubstring(IWebDriver driver, string urlSubstring)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(drv => drv.Url.Contains(urlSubstring));
        }

        // Explicit wait for an already given element object to be displayed
        public static void WaitForElDisplayed(IWebDriver driver, IWebElement element)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(4));
            wait.Until(drv => element.Displayed);
        }

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
            return Decimal.Parse(myString, NumberStyles.AllowCurrencySymbol | NumberStyles.Number);
        }

        // Clears then sends string to given text field
        public static void ClearAndSendToTextField(IWebElement element, string myString)
        {
            element.Clear();
            element.SendKeys(myString);
        }

        // Take screenshot
        public static void TakeScreenshot(IWebDriver driver, string name, string description)
        {
            var path = _screenshotPath + name + ".png";

            ITakesScreenshot ssdriver = driver as ITakesScreenshot;
            Screenshot screenshot = ssdriver.GetScreenshot();
            screenshot.SaveAsFile(path);

            TestContext.AddTestAttachment(path, description);
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

        // Get only the numerical characters from the text of a located web element
        // Returns integer
        public static int EleToInt(IWebDriver driver, By locator)
        {
            var text = driver.FindElement(locator).Text;
            return StringToInt(text);
        }

        public static void SaveAndAttachScreenShot(Screenshot screenshot, string name, string description=null)
        {
            screenshot.SaveAsFile($"{_screenshotPath}{name}.png");
            TestContext.AddTestAttachment($"{_screenshotPath}{name}.png", description);
        }

        // Scroll element into view
    }
}
