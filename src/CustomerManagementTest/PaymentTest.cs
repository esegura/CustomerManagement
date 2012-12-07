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
    public class PaymentTest
    {
        string dbConnStr;
        long id;

        [SetUp]
        public void Setup()
        {
            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

            using (var db = new CustomerContext(dbConnStr))
            {
                var payment = new Payment {Amount=300M, Response="response", StatusCode="code" };
                db.Save(payment);
                db.SaveChanges();
                id = payment.Id;
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
                var payment = db.Payments.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(payment.Amount ==300M && payment.Response == "response" && payment.StatusCode == "code", "Incorrect payment info");

            }
        }


        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var payment = db.Payments.FirstOrDefault(p => p.Id == id);
                db.Attach(payment);
                payment.Amount= 700M;
                payment.Response = "response update";
                payment.StatusCode = "Code update";
                db.SaveChanges();
                payment = db.Payments.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(payment.Amount == 700M && payment.Response == "response update" && payment.StatusCode == "Code update", "Incorrect payment info");
            }
        }


        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var payment = db.Payments.FirstOrDefault(p => p.Id == id);
                db.Attach(payment);
                payment.IsDeleted = true;
                db.SaveChanges();
                Assert.IsTrue(db.Payments.Where(p => !p.IsDeleted).Count() == 0, "Not deleted");
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
