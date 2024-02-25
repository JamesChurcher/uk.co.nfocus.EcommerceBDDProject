using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using uk.co.nfocus.ecommerce_mini_project.Utilities;
using static uk.co.nfocus.ecommerce_mini_project.Utilities.TestHelper;

namespace uk.co.nfocus.ecommerce_mini_project.POMClasses
{
    internal class CheckoutPagePOM
    {
        private IWebDriver _driver;

        public CheckoutPagePOM(IWebDriver driver)
        {
            this._driver = driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("checkout"),
                        "Not on the checkout page");   //Verify we are on the correct page
        }

        //Locators

        private IWebElement _firstNameField => _driver.FindElement(By.Id("billing_first_name"));
        //public string SetFirstName
        //{
        //    get => _firstNameField.Text;
        //    set => _firstNameField.SendKeys(value);
        //}

        private IWebElement _lastNameField => _driver.FindElement(By.Id("billing_last_name"));
        private IWebElement _countryDropDown => _driver.FindElement(By.Id("billing_country"));
        private IWebElement _streetField => _driver.FindElement(By.Id("billing_address_1"));
        private IWebElement _cityField => _driver.FindElement(By.Id("billing_city"));
        private IWebElement _postcodeField => _driver.FindElement(By.Id("billing_postcode"));
        private IWebElement _phoneNumberField => _driver.FindElement(By.Id("billing_phone"));
        //private IWebElement _paymentMethodRadio => _driver.FindElement(By.CssSelector("label[for=\"payment_method_cheque\"]"))
        private IWebElement _paymentMethodRadio => _driver.FindElement(By.ClassName("payment_methods"));    //Payment method parent element
        //private IWebElement _paymentMethodRadioCheck => _driver.FindElement(By.CssSelector(".payment_method_cheque > label"));
        //private IWebElement _paymentMethodRadioCOD => _driver.FindElement(By.CssSelector(".payment_method_cod > label"));
        private IWebElement _placeOrderButton => _driver.FindElement(By.Id("place_order"));
        //private IWebElement _placeOrderButton => _driver.FindElement(By.LinkText("Place order"));

        //Service methods

        // Set the street in the first street field
        public CheckoutPagePOM SetFirstName(string name)
        {
            ClearAndSendToTextField(_firstNameField, name);
            //_firstNameField.Clear();
            //_firstNameField.SendKeys(name);
            return this;
        }

        // Set the street in the last street field
        public CheckoutPagePOM SetLastName(string name)
        {
            ClearAndSendToTextField(_lastNameField, name);
            //_lastNameField.Clear();
            //_lastNameField.SendKeys(name);
            return this;
        }

        // Set the street in the street address field
        public CheckoutPagePOM SetStreetAddress(string street)
        {
            ClearAndSendToTextField(_streetField, street);
            //_streetField.Clear();
            //_streetField.SendKeys(street);
            return this;
        }

        // Set the town or city in the city field
        public CheckoutPagePOM SetCityField(string city)
        {
            ClearAndSendToTextField(_cityField, city);
            //_cityField.Clear();
            //_cityField.SendKeys(city);
            return this;
        }

        // Set the postcode in the postcode field
        public CheckoutPagePOM SetPostcodeField(string postcode)
        {
            ClearAndSendToTextField(_postcodeField, postcode);
            //_postcodeField.Clear();
            //_postcodeField.SendKeys(postcode);
            return this;
        }

        // Set the phone number in the phone number field
        public CheckoutPagePOM SetPhoneNumberField(string phoneNumber)
        {
            ClearAndSendToTextField(_phoneNumberField, phoneNumber);
            //_phoneNumberField.Clear();
            //_phoneNumberField.SendKeys(phoneNumber);
            return this;
        }

        // Select country from the dropdown
        public CheckoutPagePOM SelectCounrtyDropdown(string country)
        {
            new SelectElement(_countryDropDown).SelectByText(country);
            return this;
        }

        // Select payment method
        public CheckoutPagePOM SelectPaymentMethod(PaymentMethod method)
        {
            //Create locator from input enum for which payment method to select
            var locator = By.CssSelector($".payment_method_{method.ToString()} > label");

            //Loop over radio click until it is loaded
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    //Console.WriteLine("For loop i is " + i);

                    //Find the payment method and click
                    _paymentMethodRadio.FindElement(By.CssSelector($".payment_method_{method} > label")).Click();

                    break;
                }
                catch (Exception)   //Catches StaleElementReferenceException and ElementClickInterceptedException
                {
                    Thread.Sleep(150);  //Pause while page loads
                }
                // TODO > Maybe catch no element exception too?
            }

            return this;
        }

        // Place and submit the order by clicking the place order button
        public void ClickPlaceOrder()
        {
            _placeOrderButton.Click();
            WaitForUrlSubstring(_driver, "order");  //Wait for order summary page to show
            //new WebDriverWait(_driver, TimeSpan.FromSeconds(4)).Until(drv => drv.Url.Contains("order"));
        }

        //Highlevel service methods

        public void CheckoutExpectSuccess(string firstName, string lastName, string country, string street, string city, string postcode, string phoneNumber, PaymentMethod paymentMethod)
        {
            // Set text field information
            SetFirstName(firstName);
            SetLastName(lastName);
            SetStreetAddress(street);
            SetCityField(city);
            SetPostcodeField(postcode);
            SetPhoneNumberField(phoneNumber);

            // Select from dropdown
            SelectCounrtyDropdown(country);

            // Select payment method
            SelectPaymentMethod(paymentMethod);

            //Thread.Sleep(3000);

            //Loop over button click until it is loaded onto page
            for (int i=0; i<15; i++)
            {
                try
                {
                    //Console.WriteLine("For loop i is " + i);
                    ClickPlaceOrder();
                    break;
                }
                catch (Exception)      //Catch if the place order button has not loaded yet
                {
                    Thread.Sleep(150);  //Pause while page loads
                }
                // TODO > Maybe catch no element exception too?
            }
        }
    }
}
