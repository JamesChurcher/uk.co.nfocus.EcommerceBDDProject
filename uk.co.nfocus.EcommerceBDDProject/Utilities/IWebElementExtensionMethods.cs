using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
