using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerManagement;
using CustomerManagement.Model;

namespace CustomerManagementTest
{
    class RandomObjectFactory
    {
        private static Random rnd = new Random((int)DateTime.Now.Ticks);

        internal static Customer createRandomCustomer()
        {
            Customer c = new Customer(Customer.Type.Regular, 0);
            c.FirstName = "FirstName";
            c.MiddleName = "MiddleName";
            c.LastName = "LastName";
            return c;
        }

        internal static Address createRandomAddress()
        {
            string Line1 = "line1" + rnd.Next(1000000);
            string Line2 = "line2" + rnd.Next(1000000);
            string City = "city" + rnd.Next(1000000);
            string State = "CA";
            string ZipCode = rnd.Next(10000).ToString();
            Address.CountryCode Country = Address.CountryCode.US;

            Address a = CustomerManagementService.CreateAddress(Address.TypeEnum.Home, Line1, Line2, City, State, ZipCode, Country);
            return a;
        }

        internal static Phone createRandomPhone()
        {
            Phone p = new Phone(Phone.Type.Home);
            p.CountryCallingCode = "1" + rnd.Next(10);
            p.Number = rnd.Next().ToString();
            return p;
        }

        internal static Login createRandomLogin()
        {
            Login l = new Login();
            l.FrontDoor = "frontDoor";
            return l;
        }

        internal static Customer createRandomCustomerGraph()
        {
            Customer c = createRandomCustomer();

            int addressCount = rnd.Next(10) + 1;
            for (int i = 0; i < addressCount; i++)
            {
                c.Addresses.Add(createRandomAddress());
            }

            int loginsCount = rnd.Next(10) + 1;
            for (int i = 0; i < loginsCount; i++)
            {
                c.Logins.Add(createRandomLogin());
            }

            int phonesCount = rnd.Next(10) + 1;
            for (int i = 0; i < phonesCount; i++)
            {
                c.Phones.Add(createRandomPhone());
            }

            return c;
        }

        internal static Invoice createRandomInvoice()
        {
            Customer c = createRandomCustomer();
            c.Save(0);

            Invoice i = new Invoice(Invoice.Type.Invoice);
            i.CustomerId = c.Id;
            return i;
        }


        internal static Item createRandomItem()
        {
            Item i = new Item();
            i.Glacctno = "glacctno" + rnd.Next(100);
            i.SubscriptionDays = 10;
            i.ItemClass = "ItemClass";

            int itemPricingCount = rnd.Next(10) + 1;
            for (int j = 0; j < itemPricingCount; j++)
            {
                i.ItemPricings.Add(createRandomItemPricing());
            }

            return i;
        }

        internal static ItemPricing createRandomItemPricing()
        {
            ItemPricing ip = new ItemPricing();
            ip.Description = "desc";
            ip.EndDate = DateTime.Now;
            ip.OverrideGlacctno = "override";
            ip.PromoCode = "promo";
            ip.StartDate = DateTime.Now;
            ip.UnitPrice = new Decimal(1.4);
            return ip;
        }

        internal static InvoiceDetail createRandomInvoiceDetail(ItemPricing ip)
        {
            InvoiceDetail id = new InvoiceDetail(ip);
            id.ItemUnits = rnd.Next(10000);
            return id;
        }

        internal static Payment createRandomPayment()
        {
            Invoice i = createRandomInvoice();
            i.Save();

            Payment p = new Payment();
            p.InvoiceId = i.Id;
            p.LastFourDigitsOfCreditCard = (short) rnd.Next(10000);
            p.ExpirationMonth = (byte) rnd.Next(12);
            p.ExpirationYear = (short) (2010 + rnd.Next(100));
            p.PaymentTransactions.Add(createRandomPaymentTransaction());

            return p;
        }

        private static PaymentTransaction createRandomPaymentTransaction()
        {
            PaymentTransaction pt = new PaymentTransaction();
            pt.PaymentType = createRandomPaymentType();
            pt.PaymentId = 0; // need to get this when saving holder
            pt.Amount = new decimal(rnd.NextDouble());
            pt.AuthCode = rnd.Next().ToString();
            pt.PaymentStatusCode = createRandomPaymentStatusCode();
            return pt;
        }

        private static PaymentType createRandomPaymentType()
        {
            PaymentType pt = new PaymentType();
            pt.CardType = rnd.Next().ToString();
            pt.Glacctno = rnd.Next().ToString();
            return pt;
        }

        private static PaymentStatusCode createRandomPaymentStatusCode()
        {
            PaymentStatusCode psc = new PaymentStatusCode();
            psc.PaymentTransactionId = 0; // need to get this when saving holder
            psc.StatusCode = rnd.Next().ToString();
            return psc;
        }

    }
}
