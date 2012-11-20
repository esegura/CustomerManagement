using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using CustomerManagement.Model.AddressType;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    [KnownType("GetKnownType")]
    public class Customer
    {
        public enum Type { Regular } // Defines what types of customers are there

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public String FirstName { get; set; }
        [DataMember]
        public String MiddleName { get; set; }
        [DataMember]
        public String LastName { get; set; }
        [DataMember]
        public Type CustomerType { get; set; }
        [DataMember]
        public List<Address> Addresses { get; set; }
        [DataMember]
        public List<Phone> Phones { get; set; }
        [DataMember]
        public List<Login> Logins { get; set; }
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public int UserId { get; set; }

        // Load will return null if the dalItem is not found, or throw an exception if an error happens
        public static Customer Load(int customerId)
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Customer c = findRecord(dc, customerId);

            Customer bo = new Customer();
            map(c,
                dc.CustomerTypes.Where(ct => ct.Id == c.CustomerTypeId).Single(),
                Address.LoadWithCustomerId(dc, customerId),
                Phone.LoadWithCustomerId(dc, customerId),
                Login.LoadWithCustomerId(dc, customerId),
                bo);

            return bo;
        }

        private static DAL.Customer findRecord(DAL.CustomersDataContext dc, int customerId)
        {
            return dc.Customers.Where(record => record.Id == customerId).Where(record => !record.Deleted).Single();
        }

        private static void map(DAL.Customer dalCustomer, DAL.CustomerType dalCustomerType,
            IEnumerable<Address> addresses, IEnumerable<Phone> phones, IEnumerable<Login> logins, Customer bo)
        {
            bo.Id = dalCustomer.Id;
            bo.FirstName = dalCustomer.FirstName;
            bo.MiddleName = dalCustomer.MiddleName;
            bo.LastName = dalCustomer.LastName;
            bo.CustomerType = (Type)Enum.Parse(typeof(Type), dalCustomerType.TypeName);
            bo.Balance = dalCustomer.Balance;
            bo.UserId = dalCustomer.UserId;

            bo.Addresses = new List<Address>(addresses);
            bo.Phones = new List<Phone>(phones);
            bo.Logins = new List<Login>(logins);
        }

        public Customer(Customer.Type ct, int userId):this()
        {
            this.CustomerType = ct;
            this.UserId = userId;
        }

        private Customer() 
        {
            this.Addresses = new List<Address>();
            this.Phones = new List<Phone>();
            this.Logins = new List<Login>();
        }

        /// <summary>
        /// Saves the object to the db. Throws an exception if any the required values has not been set
        /// </summary>
        internal void Save(int actionPerformerId)
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Customer dalCustomer = null;

            if (this.Id == 0)
            {
                dalCustomer = new DAL.Customer();
                map(dc, this, dalCustomer, actionPerformerId);
                dc.Customers.InsertOnSubmit(dalCustomer);
            }
            else
            {
                dalCustomer = findRecord(dc, this.Id);
                map(dc, this, dalCustomer, actionPerformerId);
            }

            dc.SubmitChanges();
            this.Id = dalCustomer.Id;

            // save all new objects and update existing ones if modified
            foreach (var item in this.Addresses)
            {
                item.SaveDependent(dc, dalCustomer, actionPerformerId); 
            }
            // delete all missing objects
            foreach (var item in Address.LoadWithCustomerId(dc, this.Id))
            {
                if (!this.Addresses.Contains(item))
                    Address.Delete(dc, item);
            }

            foreach (var item in this.Phones)
            {
                item.SaveDependent(dc, dalCustomer);
            }
            foreach (var item in Phone.LoadWithCustomerId(dc, this.Id))
            {
                if (!this.Phones.Contains(item))
                    Phone.Delete(dc, item);
            }

            foreach (var item in this.Logins)
            {
                item.SaveDependent(dc, dalCustomer);
            }
            foreach (var item in Login.LoadWithCustomerId(dc, this.Id))
            {
                if (!this.Logins.Contains(item))
                    Login.Delete(dc, item);
            }
        }

        private void map(DAL.CustomersDataContext dc, Customer customer, DAL.Customer dalCustomer, int actionPerformerId)
        {
            bool isNew = customer.Id == 0;
            bool isModified = false;

            if (dalCustomer.FirstName != customer.FirstName)
            {
                dalCustomer.FirstName = customer.FirstName;
                isModified = true;
            }

            if (dalCustomer.MiddleName != customer.MiddleName)
            {
                dalCustomer.MiddleName = customer.MiddleName;
                isModified = true;
            }

            if (dalCustomer.LastName != customer.LastName)
            {
                dalCustomer.LastName = customer.LastName;
                isModified = true;
            }

            DAL.CustomerType ct = customer.CustomerType.findCustomerType(dc);
            if ((dalCustomer.CustomerType == null) || (dalCustomer.CustomerType.Id != ct.Id))
            {
                dalCustomer.CustomerType = ct;
                isModified = true;
            }

            if (dalCustomer.Balance != customer.Balance)
            {
                dalCustomer.Balance = customer.Balance;
                isModified = true;
            }

            if (dalCustomer.UserId != customer.UserId)
            {
                dalCustomer.UserId = customer.UserId;
                isModified = true;
            }

            if (isNew)
            {
                dalCustomer.CreatedBy = actionPerformerId;
                dalCustomer.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalCustomer.LastChangedBy = actionPerformerId;
                dalCustomer.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Customer other = obj as Customer;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.FirstName != other.FirstName)
                return false;

            if (this.MiddleName != other.MiddleName)
                return false;

            if (this.LastName != other.LastName)
                return false;

            if (this.CustomerType != other.CustomerType)
                return false;

            if (this.Addresses.Count != other.Addresses.Count)
                return false;

            if (this.Phones.Count != other.Phones.Count)
                return false;

            if (this.Logins.Count != other.Logins.Count)
                return false;

            foreach (var item in this.Addresses)
            {
                if (!other.Addresses.Contains(item))
                    return false;
            }

            foreach (var item in this.Phones)
            {
                if (!other.Phones.Contains(item))
                    return false;
            }

            foreach (var item in this.Logins)
            {
                if (!other.Logins.Contains(item))
                    return false;
            }

            return true;
        }

        public void Delete()
        {
            if (this.Id == 0)
                throw new InvalidOperationException("Object has not been persisted, and thus cannot be deleted");

            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Customer dalCustomer = findRecord(dc, this.Id);
            dalCustomer.Deleted = true;
            dc.SubmitChanges();

            // delete all dependent objects
            foreach (var item in this.Addresses)
            {
                Address.Delete(dc, item);
            }

            foreach (var item in this.Phones)
            {
                Phone.Delete(dc, item);
            }

            foreach (var item in this.Logins)
            {
                Login.Delete(dc, item);
            }
        }

        internal static Customer LoadWithUserId(int userId)
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Customer c = findRecordByUserId(dc, userId);

            Customer bo = new Customer();
            map(c,
                dc.CustomerTypes.Where(ct => ct.Id == c.CustomerTypeId).Single(),
                Address.LoadWithCustomerId(dc, c.Id),
                Phone.LoadWithCustomerId(dc, c.Id),
                Login.LoadWithCustomerId(dc, c.Id),
                bo);

            return bo;
        }

        private static DAL.Customer findRecordByUserId(DAL.CustomersDataContext dc, int userId)
        {
            return dc.Customers.Where(record => record.UserId == userId).Where(record => !record.Deleted).Single();
        }

        private static System.Type[] GetKnownType()
        {
            return AddressFactory.GetAddressTypes();
        }

        internal void ModifyBalance(decimal amount)
        {
            using (var dc = new DAL.CustomersDataContext())
            {
                dc.Customers.Single(c => c.Id == this.Id).Balance += amount;
                dc.SubmitChanges();
            }
        }
    }

    /// <summary>
    /// Extends the enummeration Customer.Type to provide easy mapping between the in-object enum values with in-DB stored objects
    /// </summary>
    public static class CustomerTypeExtension
    {
        public static DAL.CustomerType findCustomerType(this Customer.Type ct, DAL.CustomersDataContext dc)
        {
            // this must exist. If it doesn't, it's because the CustomerType table in the DB is out of sync with the enum below.
            return dc.CustomerTypes.Where(rec => rec.TypeName == Enum.GetName(ct.GetType(), ct)).First();
        }
    }

}
