using static uk.co.nfocus.EcommerceBDDProject.Utilities.TestHelper;

namespace uk.co.nfocus.EcommerceBDDProject.POCOClasses
{
    internal class BillingDetailsPOCO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string phoneNumber { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
