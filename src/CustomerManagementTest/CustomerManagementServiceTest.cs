using System;
using CustomerManagement;
using CustomerManagement.Model;
using CustomerManagement.Model.AddressType;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerManagementTest
{
    /// <summary>
    ///This is a test class for CustomerManagementServiceTest and is intended
    ///to contain all CustomerManagementServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CustomerManagementServiceTest
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
        //Use ClassCleanup to run code after all tests in a class have run
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
        public void SaveTest()
        {
            int actionPerformerId = 1;
            Customer customer = CustomerManagementService.CreateCustomer(Customer.Type.Regular, "first", "last", 1);
            Address address = CustomerManagementService.CreateAddress(Address.TypeEnum.BillTo, "line1", "", "", "BEN", "123", "UK");
            customer.Addresses.Add(address);

            CustomerManagementService.Save(customer, actionPerformerId);
            Assert.AreNotEqual(0, customer.Id);
            Assert.AreNotEqual(0, address.Id);
        }

        /// <summary>
        ///A test for CreateAddress
        ///</summary>
        [TestMethod()]
        public void CreateAddressTest()
        {
            Address.TypeEnum type = Address.TypeEnum.BillTo;
            string line1 = "line1";
            string line2 = string.Empty;
            string city = string.Empty;
            string state = "CA";
            string zip = "12345";
            string country = "US";

            Address actual = CustomerManagementService.CreateAddress(type, line1, line2, city, state, zip, country);
            Assert.AreEqual(state, actual.State);
        }

        [TestMethod]
        public void GetStatesTest()
        {
            Address address = CustomerManagementService.CreateAddress(Address.TypeEnum.BillTo, "line1", "", "", "CT", "a", Address.CountryCode.AU);
            System.Type theEnum = address.GetStateEnumType();

            foreach (var item in  Enum.GetValues(theEnum))
            {
                Assert.IsTrue(Enum.IsDefined(typeof(AUAddress.AUStateCode), item));
                Console.WriteLine(item);
            }
            
        }
    }
}
