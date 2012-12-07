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
    public class InvoiceTest
    {
        string dbConnStr;
        long invId;
        [SetUp]
        public void TestSetup()
        {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));


            using (var db = new CustomerContext(dbConnStr))
            {
                var customer = new Customer { CustomerType = "type", UserId = 1, FirstName = "First", LastName = "Last", Email = "hello@g.com" };
                db.Save(customer);
                db.SaveChanges();
                var invoice = new Invoice { Customer = customer, InvoiceType = Invoice.Type.Invoice, Note="Note", PaymentNote="Payment Note" } ;
                db.Save(invoice);
                db.SaveChanges();
                invId = invoice.Id;

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

        [Test]
        public void Create_Read()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoice = db.Invoices.FirstOrDefault(p=>p.Id==invId);
                Assert.IsTrue(invoice.Id == invId, "Wrong invoice");
            }
        }

        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoice = db.Invoices.FirstOrDefault(p => p.Id == invId);
                db.Attach(invoice);
                invoice.Note = "Note Update";
                invoice.PaymentNote = "Payment Note Update";
                db.SaveChanges();
                var upinvoice = db.Invoices.FirstOrDefault();
                Assert.IsTrue(upinvoice.Note == "Note Update", "Wrong invoice");
                Assert.IsTrue(upinvoice.PaymentNote == "Payment Note Update", "Wrong invoice");

            }

        }

        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoice = db.Invoices.FirstOrDefault(p => p.Id == invId);
                db.Attach(invoice);
                invoice.IsDeleted = true;
                db.SaveChanges();
                Assert.AreEqual(db.Invoices.Where(i => !i.IsDeleted).Count(), 0, "No deletes");

            }
        }

        [Test]
        public void InvoiceType()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoice = db.Invoices.FirstOrDefault(p => p.Id == invId); 
                Assert.IsTrue(invoice.InvoiceType==Invoice.Type.Invoice, "Wrong Type");
            }

        }


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
