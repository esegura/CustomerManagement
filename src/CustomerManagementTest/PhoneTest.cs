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
    public class PhoneTest
    {
        string dbConnStr;
        long id;

        [SetUp]
        public void Setup()
        {
            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

            using (var db = new CustomerContext(dbConnStr))
            {
                var phone = new Phone {Number="123-456-7899", PhoneType=Phone.Type.Work, CountryCallingCode="Code"  };
                db.Save(phone);
                db.SaveChanges();
                id = phone.Id;
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
                var phone = db.Phones.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(phone.Number == "123-456-7899" && phone.PhoneType ==Phone.Type.Work && phone.CountryCallingCode == "Code", "Incorrect phone info");

            }
        }


        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var phone = db.Phones.FirstOrDefault(p => p.Id == id);
                db.Attach(phone);
                phone.Number= "9987654321";
                phone.PhoneType = Phone.Type.Cell;
                phone.CountryCallingCode = "Code update";
                db.SaveChanges();
                phone = db.Phones.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(phone.Number == "9987654321" && phone.PhoneType == Phone.Type.Cell && phone.CountryCallingCode == "Code update", "Incorrect update");
            }
        }


        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var phone = db.Phones.FirstOrDefault(p => p.Id == id);
                db.Attach(phone);
                phone.IsDeleted = true;
                db.SaveChanges();
                Assert.IsTrue(db.Phones.Where(p => !p.IsDeleted).Count() == 0, "Not deleted");
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
