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
    public class AddressTest
    {
        string dbConnStr;
        long id;

        [SetUp]
        public void TestSetup()
        {

            dbConnStr = buildConnectionString(CustomerManagementTest.Properties.Settings.Default.TestDb, () => DateTimeOffset.UtcNow.ToString("yyyy-MM-dd_hh:mm:ssZ"));

  
            using (var db = new CustomerContext(dbConnStr))
            {

                var address = new Address(); // AddressFactory.Create(Address.CountryCode.US);
                address.Country=Address.CountryCode.US;
                address.Line1 = "Line 1";
                address.Line2 = "Line 2";
                address.State = "NY";
                address.ZipCode = "99999";
                address.Note = "Note";
                address.City="New York";
                address.AddressType = Address.TypeEnum.BillTo;
   
                db.Save(address);
                db.SaveChanges();
                id = address.Id;
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
        public void Read()
        {
            using (var db=new CustomerContext(dbConnStr)) {
                var address = db.Addresses.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(address.Line1 == "Line 1", "Wrong address");
            }

        }

        [Test]

        public void Create()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var address = new Address(); // AddressFactory.Create(Address.CountryCode.US);
                address.Country = Address.CountryCode.US;
                address.Line1 = "Line 1A";
                address.Line2 = "Line 2A";
                address.State = "NY";
                address.ZipCode = "99999";
                address.Note = "Note";
                address.City = "New York";
                address.AddressType = Address.TypeEnum.BillTo;

                db.Save(address);
                db.SaveChanges();
                Assert.AreEqual(db.Addresses.Count(), 2, "Did not add address");
                var add=db.Addresses.FirstOrDefault(a=>a.Id==address.Id);
                Assert.IsTrue(add.Line1 == "Line 1A" && add.Line2 == "Line 2A" && add.City == "New York" && add.State == "NY" && add.ZipCode == "99999" && add.Note == "Note" && add.AddressType == Address.TypeEnum.BillTo, "Did not save all fields");
       

            }
        }


        [Test]
        public void Update()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var address = db.Addresses.FirstOrDefault(p => p.Id == id);
                db.Attach(address);
                address.Line1 = "Update Line 1A";
                address.Line2 = "Update Line 2A";
                address.City = "Los Angeles";
                address.State = "CA";
                address.ZipCode = "90000";
                address.Note = "Note Update";

                address.AddressType = Address.TypeEnum.ShipTo;

                db.SaveChanges();
                var add=db.Addresses.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(add.Line1 == "Update Line 1A" && add.Line2 == "Update Line 2A" && add.City == "Los Angeles" && add.State == "CA" && add.ZipCode == "90000" && add.Note == "Note Update" && add.AddressType == Address.TypeEnum.ShipTo, "Did not update");
                
            }
        }


        [Test]
        public void Update_WithDifferentCountry()//Test shows you should not allow to change county
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var address = db.Addresses.FirstOrDefault(p => p.Id == id);
                db.Attach(address);
                address.Country = Address.CountryCode.UK;
              
                address.State = "BNE";
                db.SaveChanges();
             
                var add = db.Addresses.FirstOrDefault(p => p.Id == id);

                Assert.IsTrue(add.Country == Address.CountryCode.UK, "Country did not change");
                address = db.Addresses.FirstOrDefault(p => p.Id == id);
                address.State = "BNE";
                db.SaveChanges();
           

            }
        }

        [Test]
        public void Delete()
        {
            using (var db = new CustomerContext(dbConnStr))
            {
                var address = db.Addresses.FirstOrDefault(p => p.Id == id);
                db.Attach(address);
                address.IsDeleted = true;
                db.SaveChanges();
                Assert.AreEqual(db.Addresses.Where(a => !a.IsDeleted).Count(), 0);

            }
        }


        [Test]
        public void TestCountryString_And_Enum() {
            using (var db = new CustomerContext(dbConnStr))
            {
                var address = db.Addresses.FirstOrDefault(p => p.Id == id);
                Assert.IsTrue(address.country == "US", "Country not set correctly");
                Assert.IsTrue(address.Country == Address.CountryCode.US, "Country not converting correctly");
                address.Country = Address.CountryCode.UK; 
                Assert.IsTrue(address.country == "UK", "Country not set correctly");

            }
        }
/*
        [Test]

        public void TestStateString_And_Enum()
        {
            using (var db = new CustomerContext(dbConnStr))
            {

                var address = new Address();// AddressFactory.Create(Address.CountryCode.US);
                address.Country = Address.CountryCode.US;
                Assert.Throws<ArgumentException>( delegate { address.State = "USSS"; });
                address = address = new Address();
                address.Country = Address.CountryCode.UK;
                Assert.Throws<ArgumentException>(delegate { address.State = "CA"; });


                var  address2 = new Address();
                address2.Country = Address.CountryCode.UK;
                Assert.DoesNotThrow(delegate { address2.State = "BNE"; });
                
               
            }
        }
*/
        public void TestAddressType()
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
