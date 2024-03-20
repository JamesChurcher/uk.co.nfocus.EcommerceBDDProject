using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using uk.co.nfocus.EcommerceBDDProject.POCOClasses;
using uk.co.nfocus.EcommerceBDDProject.Support;
using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;
using uk.co.nfocus.EcommerceBDDProject.Utilities;

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
        // First name in the FirstName field
        public CheckoutPagePOM SetFirstName(string name)
        {
            _firstNameField.ClearAndSendKeys(name);
            return this;
        }

        // Last name in the lastname field
        public CheckoutPagePOM SetLastName(string name)
        {
            _lastNameField.ClearAndSendKeys(name);
            return this;
        }

        // Street in the Street field
        public CheckoutPagePOM SetStreetAddress(string street)
        {
            _streetField.ClearAndSendKeys(street);
            return this;
        }

        // City in the City field
        public CheckoutPagePOM SetCityField(string city)
        {
            _cityField.ClearAndSendKeys(city);
            return this;
        }

        // Postcode in the Postcode field
        public CheckoutPagePOM SetPostcodeField(string postcode)
        {
            _postcodeField.ClearAndSendKeys(postcode);
            return this;
        }

        // Phone number in the PhoneNumber field
        public CheckoutPagePOM SetPhoneNumberField(string phoneNumber)
        {
            _phoneNumberField.ClearAndSendKeys(phoneNumber);
            return this;
        }

        // Select the Country from the dropdown
        public CheckoutPagePOM SelectCounrtyDropdown(string country)
        {
            new SelectElement(_countryDropDown).SelectByText(country);
            return this;
        }

        // Select payment method from radio buttons
        public CheckoutPagePOM SelectPaymentMethod(PaymentMethod method)
        {
            bool flag = false;

            //Loop over radio click until it is loaded
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    //Find the payment method and click
                    _GetChildPaymentMethod(method).Click();
                    flag = true;

                    break;
                }
                catch (Exception)   //Catches StaleElementReferenceException and ElementClickInterceptedException
                {
                    Thread.Sleep(150);  //Pause while page loads
                }
            }

            // If trying to select payment method was still unsuccessful
            if (!flag)
            {
                throw new ElementNotVisibleException("Could not click element to select payment method");
            }

            return this;
        }

        // Place and submit the order by clicking the place order button
        public void ClickPlaceOrder()
        {
            _placeOrderButton.Click();
            _driver.WaitUntilUrlSubstring("order");  //Wait for order summary page to show
        }

        //----- Higher level helpers -----

        public void CheckoutExpectSuccess(BillingDetailsPOCO billingDetails)
        {
            // Set text field information
            SetFirstName(billingDetails.FirstName);
            SetLastName(billingDetails.LastName);
            SetStreetAddress(billingDetails.Street);
            SetCityField(billingDetails.City);
            SetPostcodeField(billingDetails.Postcode);
            SetPhoneNumberField(billingDetails.PhoneNumber);

            // Select from dropdown
            SelectCounrtyDropdown(billingDetails.Country);

            // Select payment method
            SelectPaymentMethod(billingDetails.PaymentMethod);

            bool flag = false;

            //Loop over button click until it is loaded onto page
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    ClickPlaceOrder();  //Place order
                    flag = true;

                    break;
                }
                catch (Exception)      //Catch if the place order button has not loaded yet
                {
                    Thread.Sleep(150);  //Pause while page loads
                }
            }

            // If trying to place an order was still unsuccessful
            if (!flag)
            {
                throw new ElementNotVisibleException("Could not click element to place order");
            }
        }
    }
}
