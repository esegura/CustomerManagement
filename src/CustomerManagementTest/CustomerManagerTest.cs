using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using Moq.Protected;
using CustomerManagement.Model;
using CustomerManagement;
using CustomerManagement.DAL;


namespace CustomerManagementTest
{
    [TestFixture]
    public class CustomerManagerTest
    {
        string dbConnStr;
        long Id;
        long addrId;
        long phoneId;
        Customer customer;
        CustomerManager cs;
        List<Item> items;

        [SetUp]
        public void TestSetup()
        {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));


            var csMock = new Mock<CustomerManager>();
            csMock.CallBase = true;
            csMock.Protected().SetupGet<Func<CustomerContext>>("Db").Returns(() => new CustomerContext(dbConnStr));
            cs = csMock.Object;

            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = new Customer { CustomerType = "type", UserId = 1, FirstName = "First", LastName = "Last", Email = "hello@g.com", Balance = 12.00M };

                db.Save(cust);
                db.SaveChanges();
                Id = cust.Id;
                db.Attach(cust);

                var addr = new Address(); //AddressFactory.Create(Address.CountryCode.US);
                addr.Country = Address.CountryCode.US;
                addr.AddressType = Address.TypeEnum.BillTo;
                addr.Line1 = "Line1";
                addr.Line2 = "Line2";
                addr.State = "NY";
                addr.ZipCode = "99999";
                cust.Addresses.Add(addr);
                db.SaveChanges();
                addrId = addr.Id;

                var phone = new Phone();
                phone.CountryCallingCode = "Code";
                phone.Number = "123-123-1234";
                phone.PhoneType = Phone.Type.Work;

                cust.Phones.Add(phone);
                db.SaveChanges();
                phoneId = phone.Id;
           
                customer = cust;


                items = new List<Item>
                {
                    new Item {Glacctno="1234", ItemClass="iclass", SubscriptionDays=30, ItemPricings=new List<ItemPricing> {new ItemPricing {Description="describe 1", OverrideGlacctno="og1234", PromoCode="promo code", StartDate=DateTime.Now.AddHours(-1), EndDate=DateTime.Now.AddHours(5), UnitPrice=10.00M},new ItemPricing {Description="describe 1b", OverrideGlacctno="og1234B", PromoCode="promo codeB", StartDate=DateTime.Now.AddHours(-1), EndDate=DateTime.Now.AddHours(5), UnitPrice=52.00M} }},
                    new Item {Glacctno="12345", ItemClass="iclass", SubscriptionDays=31, ItemPricings=new List<ItemPricing> {new ItemPricing {Description="describe 2", OverrideGlacctno="og12345", PromoCode="promo code2", StartDate=DateTime.Now.AddHours(-1), EndDate=DateTime.Now.AddHours(5), UnitPrice=11.00M} }},
                    new Item {Glacctno="123456", ItemClass="iclass", SubscriptionDays=32, ItemPricings=new List<ItemPricing> {new ItemPricing {Description="describe 3", OverrideGlacctno="og123456", PromoCode="promo code3", StartDate=DateTime.Now.AddHours(-1), EndDate=DateTime.Now.AddHours(5), UnitPrice=12.00M} }}
                };

                foreach (var i in items)
                {

                    db.Save(i);

                }
                db.SaveChanges();
          
            }           
        }

        [TearDown]
        public void Teardown()
        {
            using (var dbCtx = new CustomerContext(dbConnStr))
            {
                if (!dbCtx.Database.Delete())
                    throw new Exception("Could not delete temporary test db with connection string: " + dbConnStr);
            }
        }

        #region Customer

        [Test]
        public void TestReadCustomer()
        {
            var cust = cs.GetCustomer(Id);
            Assert.NotNull(cust, "no customer");
        }

        [Test]
        public void TestCreateCustomer()
        {
            var cust = cs.CreateCustomer("Type", "First", "Last", "Email", 1);
            Assert.IsTrue(cust.Id>0, "no new customer");
            Assert.IsTrue(cust.CustomerType == "Type" && cust.FirstName == "First" && cust.LastName == "Last" && cust.Email == "Email", "Not all fields saved correctly");
        }

        [Test]
        public void TestCreateCustomer_WithoutEmail()
        {
            Assert.Throws<ArgumentNullException>(delegate { var cust = cs.CreateCustomer("Type", "First", "Last", "", 1); });
        }

        [Test]
        public void TestCreateCustomer_WithoutType()
        {
            Assert.Throws<ArgumentNullException>(delegate { var cust = cs.CreateCustomer("", "First", "Last", "Email", 1); });
        }

        [Test]
        public void TestCreateCustomer_WithoutFirstName()
        {
            Assert.Throws<ArgumentNullException>(delegate { var cust = cs.CreateCustomer("Type", "", "Last", "Email", 1); });
        }

        [Test]
        public void TestCreateCustomer_WithoutLastName()
        {
            Assert.Throws<ArgumentNullException>(delegate { var cust = cs.CreateCustomer("Type", "First", "", "Email", 1); });
        }



        [Test]
        public void TestUpdateCustomer()
        {         
            var newcust= new Customer { CustomerType = "update type", FirstName = "update First", LastName = "update Last", Email = "update@g.com"};
            var updatecust = cs.UpdateCustomer(Id, newcust);
            var cust = cs.GetCustomer(Id);
            Assert.IsTrue(updatecust.CustomerType == "update type" && updatecust.FirstName == "update First" && updatecust.LastName == "update Last" && updatecust.Email == "update@g.com", "did not update");
            Assert.IsTrue(cust.CustomerType == "update type" && cust.FirstName == "update First" && cust.LastName == "update Last" && cust.Email == "update@g.com", "did not update");
      
        }

        [Test]
        public void TestDeleteCustomer()
        {
            cs.DeleteCustomer(Id);
            var cust = cs.GetCustomer(Id);
            Assert.IsTrue(cust.IsDeleted==true, "did not change IsDelete status");
        }


   

        #endregion

        #region Addresses

        [Test]
        public void TestReadAddress()
        {
            var address = cs.GetAddress(addrId);
            Assert.NotNull(address, "Address not in database");
        }


        [Test]
        public void TestCreateAddress()
        {
            var add = cs.CreateAddress(Id, "Address2 Line1", "Address2 Line2", "San Francisco","CA", "99991", Address.CountryCode.US, Address.TypeEnum.Home);
            Assert.IsTrue(add.Id > 0, "no new customer");
            Assert.IsTrue(add.Line1 == "Address2 Line1" && add.Line2 == "Address2 Line2" && add.State == "CA" && add.City=="San Francisco" &&  add.ZipCode == "99991" && add.Country == Address.CountryCode.US && add.AddressType == Address.TypeEnum.Home);

        }

        [Test]
        public void TestCreateAddress_WithoutLine1()
        {
            Assert.Throws<ArgumentNullException>(delegate { var add = cs.CreateAddress(Id, "", "Address2 Line2", "San Francisco", "CA", "99991", Address.CountryCode.US, Address.TypeEnum.Home); });
        }

        [Test]
        public void TestCreateAddress_WithoutCity()
        {
            Assert.Throws<ArgumentNullException>(delegate { var add = cs.CreateAddress(Id, "Address2 Line1", "Address2 Line2", "", "CA", "99991", Address.CountryCode.US, Address.TypeEnum.Home); });
        }

        [Test]
        public void TestCreateAddress_WithoutState()
        {
            Assert.Throws<ArgumentNullException>(delegate { var add = cs.CreateAddress(Id, "Address2 Line1", "Address2 Line2", "San Francisco", "", "99991", Address.CountryCode.US, Address.TypeEnum.Home); });
        }


        [Test]
        public void TestCreateAddress_WithoutZip()
        {
            Assert.Throws<ArgumentNullException>(delegate { var add = cs.CreateAddress(Id, "Address2 Line1", "Address2 Line2", "San Francisco", "CA", "", Address.CountryCode.US, Address.TypeEnum.Home); });
        }

        [Test]
        public void TestCreateAddress_WithInvalidState()
        {
            Assert.Throws<ArgumentException>(delegate { var add = cs.CreateAddress(Id, "Address2 Line1", "Address2 Line2", "San Francisco", "BCE", "99999", Address.CountryCode.US, Address.TypeEnum.Home); });
        }
        [Test]
        public void TestUpdateAddress()
        {
            var addr = new Address(); // AddressFactory.Create(Address.CountryCode.UK);
            addr.Country = Address.CountryCode.UK;
            addr.Line1 = "Update Line1";
            addr.Line2 = "Update Line2";
            addr.City = "Los Angeles";
            addr.State = "BNE";
            addr.ZipCode = "00000";
            addr.AddressType = Address.TypeEnum.ShipTo;
         
            cs.UpdateAddress(addrId, addr);
            var updatedAdd= cs.GetAddress(addrId);
            Assert.IsTrue(updatedAdd.Line1 == "Update Line1" && updatedAdd.Line2 == "Update Line2" && updatedAdd.State == "BNE" && updatedAdd.City=="Los Angeles" && updatedAdd.ZipCode == "00000" && updatedAdd.Country == Address.CountryCode.UK && updatedAdd.AddressType==Address.TypeEnum.ShipTo);
   
        }

        [Test]
        public void TestUpdateAddress_WithInvalidState()
        {
            Assert.Throws<ArgumentException>(delegate {
                var addr = new Address();
                addr.Country = Address.CountryCode.UK;
                addr.Line1 = "Update Line1";
                addr.Line2 = "Update Line2";
                addr.City = "Los Angeles";
                addr.State = "NY";
                addr.ZipCode = "00000";
                addr.AddressType = Address.TypeEnum.ShipTo;
                cs.UpdateAddress(addrId, addr);
            });
        }

        [Test]
        public void TestDeleteAddress()
        {
            cs.DeleteAddress(addrId);
            var add = cs.GetAddress(addrId);
            Assert.IsTrue(add.IsDeleted == true, "did not change IsDelete status");
        }

        #endregion

        #region Phone
        [Test]
        public void TestReadPhone()
        {
            var phone = cs.GetPhone(phoneId);
            Assert.NotNull(phoneId, "no phone");
        }

        [Test]
        public void TestCreatePhone()
        {
            var phone = cs.CreatePhone(Id, "999-999-5555", "Code", Phone.Type.Cell);
            Assert.IsTrue(phone.Id > 0, "no new phone");
            Assert.IsTrue(phone.Number == "999-999-5555" && phone.CountryCallingCode == "Code" && phone.PhoneType == Phone.Type.Cell, "did not save all fields correctly");
        }

        [Test]
        public void TestCreatePhone_WithoutNumber()
        {
            Assert.Throws<ArgumentNullException>(delegate { var phone = cs.CreatePhone(Id, "", "Code", Phone.Type.Cell); });
        }

        [Test]
        public void TestCreatePhone_WithoutCode()
        {
            Assert.Throws<ArgumentNullException>(delegate { var phone = cs.CreatePhone(Id, "999-999-555", "", Phone.Type.Cell); });
        }

        [Test]
        public void TestUpdatePhone()
        {
            var phone = new Phone();
            phone.Number = "999-999-9999";
            phone.PhoneType = Phone.Type.Work;
            phone.CountryCallingCode="code update";
            phone=cs.UpdatePhone(phoneId, phone);
            Assert.IsTrue(phone.Number == "999-999-9999" && phone.CountryCallingCode == "code update" && phone.PhoneType == Phone.Type.Work, "did not update phone");
        }

        [Test]
        public void TestDeletePhone()
        {
            cs.DeletePhone(phoneId);
            var phone = cs.GetPhone(phoneId);
            Assert.IsTrue(phone.IsDeleted == true, "did not change IsDelete status");
        }

        #endregion

        #region Invoice
        [Test]
        public void Test_Create_ReadInvoice()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

            //create
            var invoice=cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            //read
            var newinvoice = cs.GetInvoice(invoice.Id);
            Assert.IsTrue(newinvoice.InvoiceType == Invoice.Type.Invoice && newinvoice.PaymentNote == "Payment Note" && newinvoice.Note == "Note" && newinvoice.InvoiceDetails.Count() == 3, "Incorrect invoice properities");
            Assert.IsTrue(newinvoice.InvoiceDetails[0].ItemUnits == 3, "Incorrect Invoice detail properties");
            Assert.IsTrue(newinvoice.InvoiceDetails[1].ItemUnits == 4 ,"Incorrect Invoice detail properties");
            Assert.IsTrue(newinvoice.InvoiceDetails[2].ItemUnits == 5, "Incorrect Invoice detail properties");
            Assert.IsTrue(cs.GetInvoiceDetail(newinvoice.InvoiceDetails[0].Id).ItemPricing.OverrideGlacctno == "og1234", "Incorrect Item Pricing");
            Assert.IsTrue(cs.GetInvoiceDetail(newinvoice.InvoiceDetails[1].Id).ItemPricing.OverrideGlacctno == "og12345", "Incorrect Item Pricing");
            Assert.IsTrue(cs.GetInvoiceDetail(newinvoice.InvoiceDetails[2].Id).ItemPricing.OverrideGlacctno == "og123456", "Incorrect Item Pricing");
           
        }

        [Test]
        public void Test_Create_ReadInvoice_WithoutUiITems()
        {
            Assert.Throws<ArgumentNullException>(delegate {
                List<UiItem> uiitems = new List<UiItem>();
                var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
  
            });
        
        }

        [Test]
        public void Test_Create_ReadInvoice_WithVoid()
        {
            Assert.Throws<ArgumentNullException>(delegate
            {
                List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };
                var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Void, uiitems, "Note", "Payment Note");

            });

        }

        [Test]
        public void TestUpdateInvoice()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");

            var inv = new Invoice { InvoiceType = Invoice.Type.Credit, Note = "Update Note", PaymentNote = "Payment Note Update" };
            cs.UpdateInvoice(invoice.Id, inv);

            var updateinvoice = cs.GetInvoice(invoice.Id);
            Assert.IsTrue(updateinvoice.InvoiceType == Invoice.Type.Credit && updateinvoice.Note == "Update Note" && updateinvoice.PaymentNote == "Payment Note Update", "Incorrect Invoice changes");


        }

        [Test]
        public void TestVoidInvoice()
        {
                 List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

         
            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");

            cs.VoidInvoice(invoice.Id);
            var inv = cs.GetInvoice(invoice.Id);
            Assert.IsTrue(inv.InvoiceType==Invoice.Type.Void, "did not change IsDelete status");
        }

        #endregion

        #region  InvoiceDetails (see Invoice)

        [Test]
        public void TestGetInvoiceDetail()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

            var inv=cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");

            var detail = cs.GetInvoiceDetail(inv.InvoiceDetails.First().Id);
            Assert.IsTrue( detail.ItemUnits == 3, "Did not read invoice detail");
        }

        [Test]
        public void TestUpdateInvoiceDetail()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

            var inv = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");

            var upinv = new InvoiceDetail { ItemUnits = 25, ItemPricing = items[0].ItemPricings[1] };
            cs.UpdateInvoiceDetail(inv.InvoiceDetails.First().Id, upinv);
            var updatedinvoice=cs.GetInvoiceDetail(inv.InvoiceDetails.First().Id);
            Assert.IsTrue(updatedinvoice.ItemUnits == 25 && updatedinvoice.ItemPricing.UnitPrice == 52M, "Did not update correctly");

        }

        [Test]
        public void TestDeleteInvoiceDetail()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };

            var inv = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            cs.DeleteInvoiceDetail(inv.InvoiceDetails.First().Id);
            var dinv = cs.GetInvoiceDetail(inv.InvoiceDetails.First().Id);
            Assert.IsTrue(dinv.IsDeleted == true, "did not change IsDelete status");
        }

        #endregion


        #region Item

        [Test]
        public void TestReadItem()
        {
            var item = cs.GetItem(items[0].Id);
            Assert.NotNull(item, "Item not in database");
            Assert.IsTrue(item.Glacctno == "1234" && item.ItemClass == "iclass" && item.SubscriptionDays == 30, "Incorrect Read");
      
        }

        [Test]
        public void TestCreateItem()
        {
            var item = cs.CreateItem("gno", 25, "classitem");
            Assert.IsTrue(item.Id > 0, "no new item");
            Assert.IsTrue(item.Glacctno == "gno" && item.ItemClass == "classitem" && item.SubscriptionDays == 25, "Not all fields are saved correctly");
        }

        [Test]
        public void TestUpdateItem()
        {
            var item = cs.GetItem(items[0].Id);
            item.ItemClass = "class update";
            item.Glacctno = "gno update";
            item.SubscriptionDays = 33;
            Assert.IsTrue(item.Glacctno == "gno update" && item.ItemClass == "class update" && item.SubscriptionDays == 33, "Not all fields are saved correctly");
        }

        [Test]
        public void TestDeleteItem()
        {
            cs.DeleteItem(items[0].Id);
            var item = cs.GetItem(items[0].Id);
            Assert.IsTrue(item.IsDeleted == true, "did not change IsDelete status");
        }
  
        #endregion

        #region Payment

        [Test]
        public void Test_Create_ReadPayment()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            var payment = cs.CreatePayment(invoice.Id, paymenttype.Id, 120M, "Response", "statusCode");
            Assert.IsTrue(payment.Id>0, "Payment not in database");
        }

    

        [Test]
        public void TestUpdatePayment()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            var payment = cs.CreatePayment(invoice.Id, paymenttype.Id, 120M, "Response", "statusCode");
            var  updatepaym = new Payment { Response = "Response update", StatusCode = "code update", Amount = 100M};
            cs.UpdatePayment(payment.Id, updatepaym);
            var updatedpayment = cs.GetPayment(payment.Id);
      
            Assert.IsTrue(updatedpayment.Response=="Response update" && updatedpayment.StatusCode=="code update" && updatedpayment.Amount==100M, "Not all fields are updated correctly");
        }

        [Test]
        public void TestDeletePayment()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            var payment = cs.CreatePayment(invoice.Id, paymenttype.Id, 120M, "Response", "statusCode");
            Assert.Throws<Exception>(delegate
            {
                cs.DeletePayment(payment.Id);
            });
  
        }

        #endregion

        #region PaymentType

        [Test]
        public void Test_Create_Read_PaymentType()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            Assert.IsTrue(paymenttype.Id > 0, "Did not save in database");
            var paym = cs.GetPaymentType(paymenttype.Id);
            Assert.IsTrue(paym.Source == "Source" && paym.SourceId == 3, "Did not read correctly");

        }


        [Test]
        public void TestUpdatePaymentType()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            var updatepay = new PaymentType { Source = "Source Update", SourceId = 4 };
            cs.UpdatePaymentType(paymenttype.Id, updatepay);
            var updatedpayment = cs.GetPaymentType(paymenttype.Id);
            Assert.IsTrue(updatedpayment.Source == "Source Update" && updatedpayment.SourceId == 4, "Did not read correctly");
        }

        [Test]
        public void TestDeletePaymentType()
        {
            List<UiItem> uiitems = new List<UiItem>
            {
                    new UiItem {Item=items[0], Pricing=items[0].ItemPricings[0], ItemUnits=3},
                    new UiItem {Item=items[1], Pricing=items[1].ItemPricings[0], ItemUnits=4},
                    new UiItem {Item=items[2], Pricing=items[2].ItemPricings[0], ItemUnits=5},
            };


            var invoice = cs.CreateInvoice(customer.Id, Invoice.Type.Invoice, uiitems, "Note", "Payment Note");
            var paymenttype = cs.CreatePaymentType(customer.Id, "Source", 3);
            cs.DeletePaymentType(paymenttype.Id);
            var updatedpayment = cs.GetPaymentType(paymenttype.Id);
            Assert.IsTrue(updatedpayment.IsDeleted==true, "Did not read correctly");
        }

 
        #endregion



        private string buildConnectionString(string connString, Func<string> randomGenerator)
        {
            var sb = new StringBuilder(connString);
            int posStartCatalog = connString.IndexOf("Initial Catalog");
            int posEndCatalog = connString.IndexOf(';', posStartCatalog);
            string oldDbName = connString.Substring(posStartCatalog, posEndCatalog - posStartCatalog);
            sb.Replace(oldDbName, string.Format("{0}_{1}", oldDbName, randomGenerator()));
            return sb.ToString();
        }
    }
}
