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
    public class PaymentTypeType
    {

        string dbConnStr;
        long id;

        [SetUp]
        public void Setup()
        {
            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

            using (var db = new CustomerContext(dbConnStr))
            {
                var paymenttype = new PaymentType {Source="creditcard", SourceId=1};
                db.Save(paymenttype);
                db.SaveChanges();
                id = paymenttype.Id;
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
                var paymenttype = db.PaymentTypes.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(paymenttype.Source == "creditcard" && paymenttype.SourceId==1, "Incorrect paymenttype info");

            }
        }


        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var paymenttype = db.PaymentTypes.FirstOrDefault(p => p.Id == id);
                db.Attach(paymenttype);
                paymenttype.Source = "credit card update";
                paymenttype.SourceId = 2;
                db.SaveChanges();
                paymenttype = db.PaymentTypes.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(paymenttype.Source == "credit card update" && paymenttype.SourceId == 2, "Incorrect paymenttype info");
            }
        }


        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var paymenttype = db.PaymentTypes.FirstOrDefault(p => p.Id == id);
                db.Attach(paymenttype);
                paymenttype.IsDeleted = true;
                db.SaveChanges();
                Assert.IsTrue(db.PaymentTypes.Where(p => !p.IsDeleted).Count() == 0, "Not deleted");
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
