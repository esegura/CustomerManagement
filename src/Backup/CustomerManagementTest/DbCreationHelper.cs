using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomerManagement.DAL;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CustomerManagementTest
{
    [TestClass]
    public class DbCreationHelper
    {
        [TestMethod] // so we can run directly.
        public void CreateDb()
        {
            var connStr = ConfigurationManager.ConnectionStrings[
                "CustomerManagement.Properties.Settings.CustomersConnectionString"].ConnectionString;
            var db = new CustomersDataContext(connStr);

            if (!db.DatabaseExists())
                db.CreateDatabase();
        }
    }
}
