using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using uk.co.nfocus.EcommerceBDDProject.Support;
using uk.co.nfocus.EcommerceBDDProject.Utilities;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POMClasses
{
    internal class CheckoutPagePOM
    {
        private IWebDriver _driver;

        public CheckoutPagePOM(WebDriverWrapper driverWrapper)
        {
            this._driver = driverWrapper.Driver;  //Provide driver

            Assert.That(_driver.Url,
                        Does.Contain("checkout"),
                        "Not on the checkout page");   //Verify we are on the correct page
        }

        //----- Locators -----
        //Billing info input fields
        private IWebElement _firstNameField => _driver.FindElement(By.Id("billing_first_name"));
        private IWebElement _lastNameField => _driver.FindElement(By.Id("billing_last_name"));
        private IWebElement _countryDropDown => _driver.FindElement(By.Id("billing_country"));
        private IWebElement _streetField => _driver.FindElement(By.Id("billing_address_1"));
        private IWebElement _cityField => _driver.FindElement(By.Id("billing_city"));
        private IWebElement _postcodeField => _driver.FindElement(By.Id("billing_postcode"));
        private IWebElement _phoneNumberField => _driver.FindElement(By.Id("billing_phone"));

        //Payment method
        private IWebElement _paymentMethodRadio => _driver.FindElement(By.ClassName("payment_methods"));    //Payment method parent element
        
        //Get element from the given enum that defines which payment method to select
        //  Param  -> (PaymentMethod) method: The payment method to use during checkout
        //  Return -> (IWebElement) the element that, when clicked, sets the desired payment method
        private IWebElement _GetChildPaymentMethod(PaymentMethod method) 
        { 
            return _paymentMethodRadio.FindElement(By.CssSelector($".payment_method_{method} > label")); 
        }

        private IWebElement _placeOrderButton => _driver.FindElement(By.Id("place_order"));     //Place order button

        //----- Service methods -----

        // Setters
        // First name in the firstName field
        public CheckoutPagePOM SetFirstName(string name)
        {
            ClearAndSendToTextField(_firstNameField, name);
            return this;
        }

        // Last name in the lastname field
        public CheckoutPagePOM SetLastName(string name)
        {
            ClearAndSendToTextField(_lastNameField, name);
            return this;
        }

        // Street in the street field
        public CheckoutPagePOM SetStreetAddress(string street)
        {
            ClearAndSendToTextField(_streetField, street);
            return this;
        }

        // City in the city field
        public CheckoutPagePOM SetCityField(string city)
        {
            ClearAndSendToTextField(_cityField, city);
            return this;
        }

        // Postcode in the postcode field
        public CheckoutPagePOM SetPostcodeField(string postcode)
        {
            ClearAndSendToTextField(_postcodeField, postcode);
            return this;
        }

        // Phone number in the phoneNumber field
        public CheckoutPagePOM SetPhoneNumberField(string phoneNumber)
        {
            ClearAndSendToTextField(_phoneNumberField, phoneNumber);
            return this;
        }

        // Select the country from the dropdown
        public CheckoutPagePOM SelectCounrtyDropdown(string country)
        {
            new SelectElement(_countryDropDown).SelectByText(country);
            return this;
        }

        // Select payment method from radio buttons
        public CheckoutPagePOM SelectPaymentMethod(PaymentMethod method)
        {
            

            //Loop over radio click until it is loaded
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    //Console.WriteLine("For loop i is " + i);

                    //Find the payment method and click
                    _GetChildPaymentMethod(method).Click();

                    break;
                }
                catch (Exception)   //Catches StaleElementReferenceException and ElementClickInterceptedException
                {
                    Thread.Sleep(150);  //Pause while page loads
                }
                // TODO, maybe catch no element exception too?
            }

            return this;
        }

        // Place and submit the order by clicking the place order button
        public void ClickPlaceOrder()
        {
            _placeOrderButton.Click();
            WaitForUrlSubstring(_driver, "order");  //Wait for order summary page to show
        }

        //----- Higher level helpers -----

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
                // TODO, maybe catch no element exception too?
            }
        }
    }
}
