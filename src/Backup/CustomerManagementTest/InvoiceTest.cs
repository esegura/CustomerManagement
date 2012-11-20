using CustomerManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using CustomerManagement.Model;

namespace CustomerManagementTest
{
    
    
    /// <summary>
    ///This is paymentStatusCodeId test class for InvoiceTest and is intended
    ///to contain all InvoiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class InvoiceTest
    {

        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in paymentStatusCodeId class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveInvoiceTest()
        {
            Invoice i = RandomObjectFactory.createRandomInvoice();

            Item item = RandomObjectFactory.createRandomItem();
            ItemPricing ip = RandomObjectFactory.createRandomItemPricing();
            item.ItemPricings.Add(ip);
            item.Save();

            i.InvoiceDetails.Add(RandomObjectFactory.createRandomInvoiceDetail(ip));
            i.Save();
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadInvoiceTest()
        {
            Invoice expected = RandomObjectFactory.createRandomInvoice();
            expected.Save();

            Invoice actual = Invoice.Load(expected.Id);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void DeleteInvoiceTest()
        {
            Invoice i = RandomObjectFactory.createRandomInvoice();
            i.Save();

            i.Delete();

            try
            {
                Invoice retrieved = Invoice.Load(i.Id);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            { }
        }

        [TestMethod]
        public void UpdateInvoiceTest()
        {
            Item item = RandomObjectFactory.createRandomItem();
            ItemPricing ip = RandomObjectFactory.createRandomItemPricing();
            item.ItemPricings.Add(ip);
            item.Save();

            Invoice i = RandomObjectFactory.createRandomInvoice();
            i.InvoiceDetails.Add(RandomObjectFactory.createRandomInvoiceDetail(ip));
            i.Save();

            i.InvoiceType = Invoice.Type.Refund;
            i.InvoiceDetails[0].ItemUnits = 3;
            i.Save();

            Invoice retrieved = Invoice.Load(i.Id);
            Assert.AreEqual(i, retrieved);
        }

        [TestMethod]
        public void DeleteInvoiceDetailTest()
        {
            Item item = RandomObjectFactory.createRandomItem();
            ItemPricing ip = RandomObjectFactory.createRandomItemPricing();
            item.ItemPricings.Add(ip);
            item.Save();

            Invoice i = RandomObjectFactory.createRandomInvoice();
            i.InvoiceDetails.Add(RandomObjectFactory.createRandomInvoiceDetail(ip));
            i.Save();

            i.InvoiceDetails.RemoveAt(0);
            i.Save();

            Invoice retrieved = Invoice.Load(i.Id);
            Assert.AreEqual(i, retrieved);
            Assert.AreEqual(0, retrieved.InvoiceDetails.Count);
        }
    }
}
