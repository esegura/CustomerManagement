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
    public class CustomerTest
    {
        Customer customer;
        long custId;
        CustomerManager cs;
        string dbConnStr;

        [SetUp]
        public void TestSetup() {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

            var csMock = new Mock<CustomerManager>();
            csMock.CallBase = true;
            csMock.Protected().SetupGet<Func<CustomerContext>>("Db").Returns(() => new CustomerContext(dbConnStr));
            cs = csMock.Object;


            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = new Customer { CustomerType = "type", UserId = 1, FirstName = "First", MiddleName="MiddleName", LastName = "Last", Email = "hello@g.com",Balance=12.00M};

                db.Save(cust);            
                db.SaveChanges();
                custId = cust.Id;
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
        public void TestModifyBalance()
        {
            cs.ModifyBalance(custId, 5M);
           
            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = db.Customers.First(p => p.Id == custId);
                Assert.AreEqual(cust.Balance, 17M, "Balance is not updated");
                Assert.IsTrue(cust.FirstName == "First", "wrong name");
            }
        }

        [Test]
        public void TestRead()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = db.Customers.First(p => p.Id == custId);
                Assert.AreEqual(cust.Id, custId, "Not reading correct customer");
                Assert.IsTrue(cust.FirstName == "First" && cust.LastName == "Last" && cust.MiddleName == "MiddleName" && cust.CustomerType == "type", "Did not read correct customer");
            }
        }

        [Test]
        public void TestCreate()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = new Customer { CustomerType = "type", UserId = 2, FirstName = "First", LastName = "Last", Email = "hello@g.com",Balance=12.00M};
                db.Save(cust);
                db.SaveChanges();
                var count = db.Customers.Count();
                var newcust = db.Customers.First(p => p.Id == cust.Id);
                Assert.IsTrue(count == 2, "Incorrect count");
                Assert.IsTrue(newcust.UserId == 2 && cust.Id == newcust.Id, "Customer does not match");

            }
        }

        [Test]
        public void TestUpdate()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = db.Customers.First(p => p.Id == custId);
                db.Attach(cust);
                cust.FirstName = "New Name";
                cust.LastName = "New Name";
                cust.MiddleName = "New Name";
                cust.CustomerType = "New Type";
                db.SaveChanges();
                cust = db.Customers.First(p => p.Id == custId);
                Assert.IsTrue(cust.FirstName == "New Name", "no change");
                Assert.IsTrue(cust.LastName == "New Name", "no change");
                Assert.IsTrue(cust.MiddleName == "New Name", "no change");
                Assert.IsTrue(cust.CustomerType == "New Type", "no change");
            }
        }

        [Test]
        public void TestDelete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var cust = db.Customers.First(p => p.Id == custId);
                db.Attach(cust);
                cust.IsDeleted = true;
                db.SaveChanges();
                cust = db.Customers.First(p => p.Id == custId);
                Assert.IsTrue(cust.IsDeleted == true, "Is not set to delete");
            }
        }


        [Test]
        public void TestDefaultPaymentType()
        {

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
