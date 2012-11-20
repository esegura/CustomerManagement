using CustomerManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerManagement.Model;

namespace CustomerManagementTest
{
    
    
    /// <summary>
    ///This is a test class for PaymentTest and is intended
    ///to contain all PaymentTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PaymentTest
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
            Payment target = RandomObjectFactory.createRandomPayment();
            target.Save();
        }

        /// <summary>
        ///A test for Load
        ///</summary>
        [TestMethod()]
        public void LoadTest()
        {
            Payment expected = RandomObjectFactory.createRandomPayment();
            expected.Save();

            Payment actual = Payment.Load(expected.Id);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Save
        ///</summary>
        [TestMethod()]
        public void UpdateTest()
        {
            Payment target = RandomObjectFactory.createRandomPayment();
            target.Save();

            target.LastFourDigitsOfCreditCard = 1234;
            target.PaymentTransactions[0].AuthCode = "updated";
            target.PaymentTransactions[0].PaymentType.Glacctno = "updated";
            target.PaymentTransactions[0].PaymentStatusCode.StatusCode = "1234";
            target.Save();

            Payment retrieved = Payment.Load(target.Id);

            Assert.AreEqual(target, retrieved);

        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            Payment target = RandomObjectFactory.createRandomPayment();
            target.Save();

            target.Delete();

            try
            {
                Payment.Load(target.Id);
                Assert.Fail();
            }
            catch (System.Exception)
            { }
        }
    }
}
