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
    public class ItemTest
    {
        string dbConnStr;
        long itemId;
        [SetUp]
        public void TestSetup()
        {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));


            using (var db = new CustomerContext(dbConnStr))
            {
                var item = new Item { ItemClass = "iclass", SubscriptionDays = 30, Glacctno = "1234" };
                db.Save(item);
                db.SaveChanges();
                itemId = item.Id;

            }

        }

        [Test]
        public void Create_Read()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var item = db.Items.First(p => p.Id == itemId);
                Assert.AreEqual(item.Id, itemId, "Not reading correct item");
                Assert.IsTrue(item.SubscriptionDays == 30, "wrong item");
                Assert.IsTrue(item.Glacctno == "1234", "wrong item");
            }

        }

        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var item = db.Items.First(p => p.Id == itemId);
                db.Attach(item);
                item.ItemClass = "iclass update";
                item.SubscriptionDays = 40;
                item.Glacctno = "1234 update";
                db.SaveChanges();
                db.Items.First(p => p.Id == itemId);
                Assert.IsTrue(item.ItemClass == "iclass update" && item.SubscriptionDays == 40 && item.Glacctno == "1234 update", "Did nto update");
            }

        }

        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var item = db.Items.First(p => p.Id == itemId);
                db.Attach(item);
                item.IsDeleted = true;
                db.SaveChanges();
                Assert.IsTrue(db.Items.Where(p => !p.IsDeleted).Count() == 0, "Did not delete");
           }
           

        }

        [Test]
        public void TestItemPricing()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var item = db.Items.First(p => p.Id == itemId);
                db.Attach(item);
                var itempricing = new ItemPricing { Item=item, OverrideGlacctno="abcd", PromoCode="code", UnitPrice=30M, StartDate=DateTime.UtcNow.AddHours(-1), EndDate=DateTime.UtcNow.AddHours(300)   };
                item.ItemPricings.Add(itempricing);
                db.SaveChanges();
                item = db.Items.First(p => p.Id == itemId);
                Assert.IsTrue(item.ItemPricings.Count == 1, "Did not save Item pricing");

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
