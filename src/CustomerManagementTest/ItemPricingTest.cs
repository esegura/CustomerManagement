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
    public class ItemPricingTest
    {
        string dbConnStr;
        long id;

        [SetUp]
        public void Setup()
        {
            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

            using (var db = new CustomerContext(dbConnStr))
            {
                var item = new Item { ItemClass = "iclass", SubscriptionDays = 30, Glacctno = "1234" };

                var itempricing = new ItemPricing { Item = item, OverrideGlacctno = "abcd", PromoCode = "code", UnitPrice = 30M, StartDate = DateTime.UtcNow.AddHours(-1), EndDate = DateTime.UtcNow.AddHours(300) };
                db.Save(itempricing);
                db.SaveChanges();
                id = itempricing.Id;
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
                var item = db.ItemPricings.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(item.OverrideGlacctno == "abcd" && item.PromoCode == "code" && item.UnitPrice == 30M, "Incorrect item pricing");

            }
        }


        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var itempricing = db.ItemPricings.FirstOrDefault(p => p.Id == id);
                db.Attach(itempricing);
                itempricing.OverrideGlacctno = "abcd update";
                itempricing.PromoCode = "code update";
                itempricing.UnitPrice = 70M;
                db.SaveChanges();
                itempricing = db.ItemPricings.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(itempricing.OverrideGlacctno == "abcd update" && itempricing.PromoCode == "code update" && itempricing.UnitPrice == 70M, "Incorrect item pricing");
            }
        }


        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var itempricing = db.ItemPricings.FirstOrDefault(p => p.Id == id);
                db.Attach(itempricing);
                itempricing.IsDeleted = true;
                db.SaveChanges();
                Assert.IsTrue(db.ItemPricings.Where(p=>!p.IsDeleted).Count()==0, "Not deleted");
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
