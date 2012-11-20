using System;
using System.Collections.Generic;
using CustomerManagement.Model;

namespace CustomerManagement
{
    //TODO Make this the only access point to CRUD for the customer objects
    public class CustomerManagementService
    {
        public static Customer CreateCustomer(Customer.Type type, string firstname, string lastname, int userId)
        {
            if ((firstname == null) || (firstname.Trim() == ""))
                throw new ArgumentNullException("firstname");

            if ((lastname == null) || (lastname.Trim() == ""))
                throw new ArgumentNullException("lastname");

            if (userId <= 0)
                throw new ArgumentNullException("userId");

            Customer newCustomer = new Customer(type, userId);
            newCustomer.FirstName = firstname.Trim();
            newCustomer.LastName = lastname.Trim();
            newCustomer.Save(userId); // The user is creating his own record
            return newCustomer;
        }

        public static Address CreateAddress(Address.TypeEnum type, string line1, string line2, string city, string state, string zip, Address.CountryCode country)
        {
            if ((line1 == null) || (line1.Trim() == ""))
                throw new ArgumentNullException("line1");

            if ((state == null) || (state.Trim() == ""))
                throw new ArgumentNullException("state");

            if ((zip == null) || (zip.Trim() == ""))
                throw new ArgumentNullException("zip");

            Address result = null;

            result = AddressFactory.Create(country);
            result.Line1 = line1.Trim();
            result.Line2 = (line2 != null)? line2.Trim():null;
            result.City = (city != null)?city.Trim():null;
            result.State = state.Trim();
            result.ZipCode = zip.Trim();
            result.AddressType = type;

            return result;
        }

        public static Address CreateAddress(Address.TypeEnum type, string line1, string line2, string city, string state, string zip, string country)
        {
            return CreateAddress(type, line1, line2, city, state, zip, (Address.CountryCode)Enum.Parse(typeof(Address.CountryCode), country));
        }

        /// <summary>
        /// Gets all the available products; those ones that have not been deleted.
        /// </summary>
        /// <returns>a list of active products; an empty list if no products were found.</returns>
        public static List<Item> GetAllProducts()
        {
            return new List<Item>(Item.LoadAllItems());
        }

        public static void Save(Customer customer, int actionPerformerId)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            customer.Save(actionPerformerId);
        }

        /// <summary>
        /// Retrieves the customer object associated with the user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The customer object</returns>
        /// <exception cref="CustomerServiceException">Thrown on user id not found</exception>
        public static Customer FindCustomerWithUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentNullException();

            try
            {
                return Customer.LoadWithUserId(userId);
            }
            catch (Exception e)
            {
                throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound, 
                    "Could not find customer record with userId:" + userId, e);
            }
        }

        public static IEnumerable<ItemPricing> FindItemPricingsWithItemId(int itemId)
        {
            if (itemId <= 0)
                throw new ArgumentNullException();

            try
            {
                return ItemPricing.LoadWithItemId(itemId);
            }
            catch (Exception e)
            {
                throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound, "Could not find customer record with itemId:" + itemId, e);
            }
        }

        public static void ModifyBalance(int customerId, decimal amount)
        {
            var c = Customer.Load(customerId);
            c.ModifyBalance(amount);
        }

        public static Customer FindCustomerWithId(int customerId)
        {
            if (customerId <= 0)
                throw new ArgumentNullException();

            try
            {
                return Customer.Load(customerId);
            }
            catch (Exception e)
            {
                throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound,
                    "Could not find customer record with userId:" + customerId, e);
            }
        }
    }
}
