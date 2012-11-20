using CustomerManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using CustomerManagement.Model;

namespace CustomerManagementTest
{
    
    
    /// <summary>
    ///This is paymentStatusCodeId test class for ItemTest and is intended
    ///to contain all ItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemTest
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
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadItemTest()
        {
            Item expected = RandomObjectFactory.createRandomItem();
            expected.Save();

            Item actual = Item.Load(expected.Id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LoadAllItemsTest()
        {
            List<Item> allItems = new List<Item>(Item.LoadAllItems());

            Assert.IsTrue(allItems.Count > 0);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void SaveItemTest()
        {
            Item target = RandomObjectFactory.createRandomItem();
            target.Save();
        }

        [TestMethod]
        public void ItemSerializationTest()
        {
            Item target = RandomObjectFactory.createRandomItem();
            target.Save();

            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Item));
            serializer.WriteObject(ms, target);
            
            ms.Seek(0, SeekOrigin.Begin);
            String jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Item));
            Item deserializedItem = (Item)deserializer.ReadObject(ms);

            Assert.AreEqual(target, deserializedItem);
        }

        [TestMethod]
        public void UpdateItemTest()
        {
            Item i = RandomObjectFactory.createRandomItem();
            i.Save();

            i.Glacctno = "updated";
            i.ItemPricings[0].Description = "updated";
            i.Save();

            Item retrieved = Item.Load(i.Id);
            Assert.AreEqual(i, retrieved);
            Assert.AreEqual("updated", retrieved.Glacctno);
            Assert.AreEqual("updated", retrieved.ItemPricings[0].Description);
        }

        [TestMethod]
        public void DeleteItemTest()
        {
            Item i = RandomObjectFactory.createRandomItem();
            i.Save();

            i.Delete();

            try
            {
                Item retrieved = Item.Load(i.Id);
                Assert.Fail("should have thrown exception");
            }
            catch (InvalidOperationException)
            { 
                //success
            }
        }

        [TestMethod]
        public void DeleteItemPricingTest()
        {
            Item i = RandomObjectFactory.createRandomItem();
            i.Save();

            ItemPricing ip = i.ItemPricings[0];
            i.ItemPricings.Remove(ip);
            i.Save();

            Item retrieved = Item.Load(i.Id);
            Assert.AreEqual(i, retrieved);

            Assert.IsFalse(retrieved.ItemPricings.Contains(ip));
        }
    }
}
