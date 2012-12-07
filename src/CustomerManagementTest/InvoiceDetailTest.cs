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
    public class InvoiceDetailDetail
    {
        string dbConnStr;
        long invId;
        [SetUp]
        public void TestSetup()
        {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));


            using (var db = new CustomerContext(dbConnStr))
            {
                var invoicedetail = new InvoiceDetail { ItemUnits=10 };
                db.Save(invoicedetail);
                db.SaveChanges();
                invId = invoicedetail.Id;
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
                var invoicedetail = db.InvoiceDetails.FirstOrDefault(p => p.Id == invId);
                Assert.IsTrue(invoicedetail.Id == invId, "Wrong invoicedetail");
            }
        }

        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoicedetail = db.InvoiceDetails.FirstOrDefault(p => p.Id == invId);
                db.Attach(invoicedetail);
                invoicedetail.ItemUnits = 12;
                db.SaveChanges();
                var upinvoicedetail = db.InvoiceDetails.FirstOrDefault();
                Assert.IsTrue(upinvoicedetail.ItemUnits == 12, "Wrong invoicedetail");
            }

        }

        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var invoicedetail = db.InvoiceDetails.FirstOrDefault(p => p.Id == invId);
                db.Attach(invoicedetail);
                invoicedetail.IsDeleted = true;
                db.SaveChanges();
                Assert.AreEqual(db.InvoiceDetails.Where(i => !i.IsDeleted).Count(), 0, "No deletes");

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
