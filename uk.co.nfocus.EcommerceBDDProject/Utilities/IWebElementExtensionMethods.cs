using OpenQA.Selenium;

namespace uk.co.nfocus.EcommerceBDDProject.Utilities
{
    internal static class IWebElementExtensionMethods
    {
        // Clears then sends string to given text field
        public static void ClearAndSendKeys(this IWebElement element, string myString)
        {
            element.Clear();
            element.SendKeys(myString);
        }
    }
}
