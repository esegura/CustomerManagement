using CustomerManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;
using CustomerManagement.Model;

namespace CustomerManagementTest
{
    
    
    /// <summary>
    ///This is paymentStatusCodeId test class for CustomerTest and is intended
    ///to contain all CustomerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomerTest
    {
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
        public void SaveCustomerTest()
        {
            Customer target = RandomObjectFactory.createRandomCustomer();
            target.Save(0);
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadCustomerTest()
        {
            Customer expected = RandomObjectFactory.createRandomCustomer();
            expected.Save(0);

            Customer actual = Customer.Load(expected.Id);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void addAddressTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            c.Save(0);

            // add an address
            Address a = RandomObjectFactory.createRandomAddress();
            c.Addresses.Add(a);
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);

            Assert.AreEqual(c, retrieved);
        }

        [TestMethod]
        public void addPhoneTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            c.Save(0);

            // add an address
            Phone p = RandomObjectFactory.createRandomPhone();
            c.Phones.Add(p);
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);

            Assert.AreEqual(c, retrieved);
        }

        [TestMethod]
        public void testSimpleSerialization()
        {
            Customer c = RandomObjectFactory.createRandomCustomerGraph();
            c.Save(0);
            
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Customer));
            serializer.WriteObject(ms, c);

            ms.Seek(0, SeekOrigin.Begin);
            String jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Customer));
            Customer deserializedItem = (Customer)deserializer.ReadObject(ms);

            Assert.AreEqual(c, deserializedItem);
        }

        [TestMethod]
        public void updateCustomerTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            c.Save(0);

            c.MiddleName = "updated";
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Console.Out.Write(retrieved.MiddleName);
        }

        [TestMethod]
        public void updateAddressTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Address a = RandomObjectFactory.createRandomAddress();
            c.Addresses.Add(a);
            c.Save(0);

            a.Line2 = "updated";
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual("updated", retrieved.Addresses[0].Line2);
        }

        [TestMethod]
        public void updateLoginTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Login l = RandomObjectFactory.createRandomLogin();
            c.Logins.Add(l);
            c.Save(0);

            l.FrontDoor = "updated";
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual("updated", retrieved.Logins[0].FrontDoor);
        }

        [TestMethod]
        public void updatePhonesTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Phone p = RandomObjectFactory.createRandomPhone();
            c.Phones.Add(p);
            c.Save(0);

            p.Number = "updated";
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual("updated", retrieved.Phones[0].Number);
        }

        [TestMethod]
        public void deleteAddressTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Address a = RandomObjectFactory.createRandomAddress();
            c.Addresses.Add(a);
            c.Save(0);

            c.Addresses.Remove(a);
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual(0, c.Addresses.Count);
        }

        [TestMethod]
        public void deleteLoginTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Login l = RandomObjectFactory.createRandomLogin();
            c.Logins.Add(l);
            c.Save(0);

            c.Logins.Remove(l);
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual(0, c.Logins.Count);
        }

        [TestMethod]
        public void deletePhoneTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomer();
            Phone p = RandomObjectFactory.createRandomPhone();
            c.Phones.Add(p);
            c.Save(0);

            c.Phones.Remove(p);
            c.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(c, retrieved);
            Assert.AreEqual(0, c.Phones.Count);
        }

        [TestMethod]
        public void deleteCustomerTest()
        {
            Customer c = RandomObjectFactory.createRandomCustomerGraph();
            c.Save(0);

            c.Delete();

            try
            {
                Customer retrieved = Customer.Load(c.Id);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            { }
        }

        [TestMethod]
        public void testUpdateInSerialization()
        {
            Customer c = RandomObjectFactory.createRandomCustomerGraph();
            c.Save(0);

            // Serialize the object
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Customer));
            serializer.WriteObject(ms, c);

            ms.Seek(0, SeekOrigin.Begin);
            String jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            // deserialize the object and modify it itemPricing bit
            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Customer));
            Customer deserializedItem = (Customer)deserializer.ReadObject(ms);
            deserializedItem.MiddleName = "serialized";
            if (deserializedItem.Addresses.Count > 0)
                deserializedItem.Addresses[0].Line1 = "serialized";
            if (deserializedItem.Phones.Count > 0)
                deserializedItem.Phones[0].Number = "serialized";
            if (deserializedItem.Logins.Count > 0)
                deserializedItem.Logins[0].FrontDoor = "serialized";

            // serialize it back, as if going back to the server from the flash app
            ms = new MemoryStream();
            serializer.WriteObject(ms, deserializedItem);

            ms.Seek(0, SeekOrigin.Begin);
            jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            // deserialize the object, now as if it were the server side
            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            deserializer = new DataContractJsonSerializer(typeof(Customer));
            Customer serverDeserializedItem = (Customer)deserializer.ReadObject(ms);
            serverDeserializedItem.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(deserializedItem, retrieved);
            Assert.AreEqual("serialized", retrieved.MiddleName);
            Assert.AreEqual("serialized", retrieved.Addresses[0].Line1);
            Assert.AreEqual("serialized", retrieved.Phones[0].Number);
            Assert.AreEqual("serialized", retrieved.Logins[0].FrontDoor);
        }

        [TestMethod]
        public void testDeleteOfAddressInSerialization()
        {
            Customer c = RandomObjectFactory.createRandomCustomerGraph();
            c.Save(0);

            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Customer));
            serializer.WriteObject(ms, c);

            ms.Seek(0, SeekOrigin.Begin);
            String jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            // deserialize the object and modify it itemPricing bit
            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(Customer));
            Customer deserializedItem = (Customer)deserializer.ReadObject(ms);
            if (deserializedItem.Addresses.Count > 0)
                deserializedItem.Addresses.RemoveAt(0);
            if (deserializedItem.Phones.Count > 0)
                deserializedItem.Phones.RemoveAt(0);
            if (deserializedItem.Logins.Count > 0)
                deserializedItem.Logins.RemoveAt(0);

            // serialize it back, as if going back to the server from the flash app
            ms = new MemoryStream();
            serializer.WriteObject(ms, deserializedItem);

            ms.Seek(0, SeekOrigin.Begin);
            jsonContent = UTF8Encoding.UTF8.GetString(ms.ToArray());
            Console.WriteLine(jsonContent);
            ms.Close();

            // deserialize the object, now as if it were the server side
            ms = new MemoryStream(UTF8Encoding.UTF8.GetBytes(jsonContent));
            deserializer = new DataContractJsonSerializer(typeof(Customer));
            Customer serverDeserializedItem = (Customer)deserializer.ReadObject(ms);
            serverDeserializedItem.Save(0);

            Customer retrieved = Customer.Load(c.Id);
            Assert.AreEqual(deserializedItem, retrieved);
            Assert.AreEqual(c.Addresses.Count - 1, retrieved.Addresses.Count);
            Assert.AreEqual(c.Phones.Count - 1, retrieved.Phones.Count);
            Assert.AreEqual(c.Logins.Count - 1, retrieved.Logins.Count);
        }
    }
}
