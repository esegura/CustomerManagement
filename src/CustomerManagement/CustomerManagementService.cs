using System;
using System.Linq;
using System.Collections.Generic;
using CustomerManagement.Model;
using CustomerManagement.DAL;

namespace CustomerManagement
{
    //TODO Make this the only access point to CRUD for the customer objects
    public class CustomerManagementService
    {
        public CustomerManagementService() {
        }
        #region Customers

        public Customer GetCustomer(long customerId) {
            using (var db = new CustomerContext())
            {
                return db.Customers.FirstOrDefault(p => p.Id == customerId);
            }
        }

        public Customer CreateCustomer(string type, string firstname, string lastname,  string email,  int userId)
        {
            if ((firstname == null) || (firstname.Trim() == ""))
                throw new ArgumentNullException("firstname");

            if ((lastname == null) || (lastname.Trim() == ""))
                throw new ArgumentNullException("lastname");

            if ((email == null) || (lastname.Trim() == ""))
                throw new ArgumentNullException("email");

            if ((email == null) || (lastname.Trim() == ""))
                throw new ArgumentNullException("customer type");

            if (userId <= 0)
                throw new ArgumentNullException("userId");

            Customer newCustomer = new Customer(type, userId);
            newCustomer.FirstName = firstname.Trim();
            newCustomer.LastName = lastname.Trim();
            newCustomer.Email = email.Trim();

            using (var db = new CustomerContext())
            {
                db.Save(newCustomer);
                db.SaveChanges();
            }
            return newCustomer;
        }

        public Customer UpdateCustomer(long customerId, Customer c)
        {

            if (string.IsNullOrWhiteSpace(c.FirstName.Trim()) || string.IsNullOrWhiteSpace(c.LastName.Trim()) || string.IsNullOrWhiteSpace(c.Email.Trim()))
                throw new ArgumentNullException("Missing Customer Information");

            using (var db = new CustomerContext())
            {         
                var customer = db.Customers.FirstOrDefault(p => p.Id == customerId);
                db.Attach(customer);
                customer.FirstName = c.FirstName.Trim();
                customer.LastName = c.LastName.Trim();
                customer.MiddleName = c.MiddleName.Trim();
                customer.Email = customer.Email.Trim();
                customer.CustomerType = customer.CustomerType;

                db.SaveChanges();

                return c;
            }
        }

        public void DeleteCustomer(long customerId)
        {
            using (var db = new CustomerContext())
            {
                var customer = db.Customers.FirstOrDefault(p => p.Id == customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);
                customer.IsDeleted = true;
                db.SaveChanges();
            }
        }
        #endregion

        #region Addresses
        public Address GetAddress(long addressId)
        {
            using (var db = new CustomerContext())
            {
                return db.Addresses.FirstOrDefault(p => p.Id == addressId);
            }
        }

        public Address CreateAddress(long customerId, string address1, string address2, string city, string state, string zip, Address.CountryCode country, Address.TypeEnum type)
        {
            using (var db = new CustomerContext())
            {
                if (string.IsNullOrWhiteSpace(address1.Trim())|| string.IsNullOrWhiteSpace(city.Trim())|| string.IsNullOrWhiteSpace(state)|| string.IsNullOrWhiteSpace(zip.Trim()))
                    throw new ArgumentNullException("Missing Address value");
                
                var customer=db.Customers.FirstOrDefault(c=>c.Id==customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);

                Address address = AddressFactory.Create(country);

                address.Line1 = address1.Trim();
                address.Line2 = address2.Trim();
                address.City = city.Trim();
                address.State = state.Trim();
                address.AddressType = type;   
                address.Customer = customer;
 
                db.Save(address);
                db.SaveChanges();

                return address;
            }
        }

        public Address UpdateAddress(long addressId, Address add)
        {
            if (string.IsNullOrWhiteSpace(add.Line1.Trim()) || string.IsNullOrWhiteSpace(add.City.Trim()) || string.IsNullOrWhiteSpace(add.State.Trim()) || string.IsNullOrWhiteSpace(add.ZipCode.Trim()))
                throw new ArgumentNullException("Missing Address value");

            using (var db = new CustomerContext())
            {
                var address = db.Addresses.FirstOrDefault(a => a.Id == addressId);
                db.Attach(address);
                address.Line1 = add.Line1.Trim();
                address.Line2 = add.Line2.Trim();
                address.City = add.City.Trim();
                address.State = add.State.Trim();
                address.AddressType = add.AddressType;

                db.SaveChanges();
                return add;
            }
        }

        public void DeleteAddress(long addressId)
        {
            using (var db = new CustomerContext())
            {
                var address = db.Addresses.FirstOrDefault(a => a.Id == addressId);
                db.Attach(address);
                address.IsDeleted = true;

                db.SaveChanges();
            }
        }

        #endregion

        #region Phones

        public Phone GetPhone(long phoneId)
        {
            using (var db = new CustomerContext())
            {
                return db.Phones.FirstOrDefault(p => p.Id == phoneId);
            }
        }
        public Phone CreatePhone(long customerId, string number, string code, Phone.Type type) {
                      
            if (string.IsNullOrWhiteSpace(number.Trim()) || string.IsNullOrWhiteSpace(code.Trim()))
                throw new ArgumentNullException("Missing Phone value");
            using (var db = new CustomerContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);

                var phone = new Phone(type);
                phone.Number = number.Trim();
                phone.CountryCallingCode = code.Trim();
                phone.Customer = customer;

                db.Save(phone);

                return phone;
            }        
        }

        public Phone UpdatePhone(long phoneId, Phone ph)
        {
            if (string.IsNullOrWhiteSpace(ph.Number.Trim()) || string.IsNullOrWhiteSpace(ph.CountryCallingCode.Trim()))
                throw new ArgumentNullException("Missing Phone value");
      
            using (var db = new CustomerContext())
            {
                var phone = db.Phones.FirstOrDefault(p => p.Id == phoneId);

                if (phone == null)
                    throw new ArgumentNullException("Phone does not exist");
                db.Attach(phone);
                
                phone.Number = ph.Number.Trim();
                phone.CountryCallingCode = ph.CountryCallingCode.Trim();

                db.SaveChanges();
                return ph;
            }
        }

        public void DeletePhone(long phoneId)
        {
            using (var db = new CustomerContext())
            {
                var phone = db.Phones.FirstOrDefault(p => p.Id == phoneId);

                if (phone == null)
                    throw new ArgumentNullException("Phone does not exist");
                db.Attach(phone);

                phone.IsDeleted = true;
                db.SaveChanges();
         
            }

        }

        #endregion

        #region Invoice

        public Invoice GetInvoice(long invoiceId)
        {
            using (var db = new CustomerContext())
            {
                return db.Invoices.FirstOrDefault(p => p.Id == invoiceId);
            }
        }

        public Invoice createInvoice(long customerId, Invoice.Type type, string note, string paymentnote )
        {
            using (var db = new CustomerContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");
                db.Attach(customer);

                var invoice = new Invoice(type);
                invoice.Note = note.Trim();
                invoice.PaymentNote = paymentnote.TrimEnd();

                db.Save(invoice);

                return invoice;
            }

        }


        public Invoice UpdateInvoice(long  invoiceId, Invoice inv)
        {
            using (var db = new CustomerContext())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");
                db.Attach(invoice);

                invoice.Note = inv.Note.Trim();
                invoice.PaymentNote = inv.PaymentNote.TrimEnd();
                invoice.InvoiceType = inv.InvoiceType;

                db.SaveChanges();

                return inv;
            }

        }

        public void DeleteInvoice(long invoiceId)
        {
            using (var db = new CustomerContext())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");
                db.Attach(invoice);

                invoice.IsDeleted = true;
                db.SaveChanges();

            }
        }



        #endregion

        #region InvoiceDetails

        public InvoiceDetail GetInvoiceDetails(long invoiceDetailId)
        {
            using (var db = new CustomerContext())
            {
                return db.InvoiceDetails.FirstOrDefault(p => p.Id == invoiceDetailId);
            }
        }

        public InvoiceDetail CreateInvoiceDetail(long invoiceId, long itempricingId, int itemunits)
        {
            using (var db = new CustomerContext())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");

                var itempricing = db.ItemPricings.FirstOrDefault(i => i.Id == itempricingId);

                if (itempricing == null)
                    throw new ArgumentNullException("Item Pricing does not exist");
                
                db.Attach(invoice);
                db.Attach(itempricing);

                var invoicedetail = new InvoiceDetail(itempricing);

                invoicedetail.ItemUnits = itemunits;
                invoicedetail.Invoice = invoice;
                invoicedetail.ItemPricing = itempricing;

                db.Save(invoicedetail);
                return invoicedetail;
            }

        }

        public InvoiceDetail UpdateInvoiceDetail(long invoiceDetailId, InvoiceDetail invd)
        {
            using (var db = new CustomerContext())
            {
                var invoiceDetail = db.InvoiceDetails.FirstOrDefault(i => i.Id == invoiceDetailId);

                if (invoiceDetail == null)
                    throw new ArgumentNullException("Invoice does not exist");
                db.Attach(invoiceDetail);

                invoiceDetail.ItemUnits = invd.ItemUnits;


                db.SaveChanges();

                return invoiceDetail;
            }

        }

        public void DeleteInvoiceDetail(long invoiceDetailId)
        {
            using (var db = new CustomerContext())
            {
                var invoiceDetail = db.InvoiceDetails.FirstOrDefault(i => i.Id == invoiceDetailId);

                if (invoiceDetail == null)
                    throw new ArgumentNullException("Invoice does not exist");
                db.Attach(invoiceDetail);

                invoiceDetail.IsDeleted = true;
                db.SaveChanges();

            }
        }


        #endregion

        #region Item

        public Item GetItem(long itemId)
        {
            using (var db = new CustomerContext())
            {
                return db.Items.FirstOrDefault(p => p.Id == itemId);
            }
        }

        public Item CreateItem(string gno, int subdays, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(gno.Trim()) || string.IsNullOrWhiteSpace(itemClass.Trim()) )
                 throw new ArgumentNullException("Missing Item Information");
            using (var db=new CustomerContext()){
            
                var item=new Item();
                item.Glacctno=gno.Trim();
                item.SubscriptionDays=subdays;
                item.ItemClass=itemClass.Trim();

                db.Save(item);

                return item;      
            }       
        }

        public Item UpdateItem(long itemId, Item it)
        {
            using (var db = new CustomerContext())
            {
                var item = db.Items.FirstOrDefault(p => p.Id == itemId);
                if (item ==null)
                    throw new ArgumentNullException("Item does not exist");

                item.Glacctno = it.Glacctno.Trim();
                item.SubscriptionDays = it.SubscriptionDays;
                item.ItemClass = it.ItemClass.Trim();

                return it;
            }
        }

        public void DeleteItem(long itemId)
        {
            using (var db = new CustomerContext())
            {
                var item = db.Items.FirstOrDefault(i => i.Id == itemId);

                if (item == null)
                    throw new ArgumentNullException("Item does not exist");
                db.Attach(item);

                item.IsDeleted = true;
                db.SaveChanges();

            }
        }
        #endregion

        #region ItemPricing

        public ItemPricing GetItemPricing(long itemPricingId)
        {
            using (var db = new CustomerContext())
            {
                return db.ItemPricings.FirstOrDefault(p => p.Id == itemPricingId);
            }
        }


        public ItemPricing CreateItemPricing(long itemId, DateTime start, DateTime end, string overrideGlacctno, decimal unitprice, string description, string promo )
        {
            using (var db = new CustomerContext())
            {
                var item = db.Items.FirstOrDefault(i => i.Id == itemId);

                if (item == null)
                    throw new ArgumentNullException("Item does not exist");

                var itempricing = new ItemPricing(item);
                itempricing.StartDate = start;
                itempricing.EndDate = end;
                itempricing.OverrideGlacctno = overrideGlacctno;
                itempricing.UnitPrice = unitprice;
                itempricing.Description = description;
                itempricing.PromoCode = promo;

                db.Save(itempricing);

                return itempricing;
            }
        }

        public ItemPricing UpdateItemPricing(long itemPricingId, ItemPricing ip)
        {
            using (var db = new CustomerContext())
            {
                var itempricing = db.ItemPricings.FirstOrDefault(i => i.Id == itemPricingId);

                db.Attach(itempricing);

                itempricing.StartDate= ip.StartDate;
                itempricing.EndDate = ip.EndDate;
                itempricing.OverrideGlacctno =ip.OverrideGlacctno;
                itempricing.UnitPrice = ip.UnitPrice;
                itempricing.Description = ip.Description;
                itempricing.PromoCode = ip.PromoCode;

                db.SaveChanges();

                return ip;
            }

        }

        public void DeleteItemPricing(long itemPricingId)
        {
            using (var db = new CustomerContext())
            {
                var itempricing = db.ItemPricings.FirstOrDefault(i => i.Id == itemPricingId);

                if (itempricing == null)
                    throw new ArgumentNullException("Item Pricing does not exist");
                db.Attach(itempricing);

                itempricing.IsDeleted = true;
                db.SaveChanges();

            }
        }


        #endregion

        #region Payment

        public Payment GetPayment(long paymentId)
        {
            using (var db = new CustomerContext())
            {
                return db.Payments.FirstOrDefault(p => p.Id == paymentId);
            }
        }

        public Payment CreatePayment(long paymentTypeId, decimal amount, string response, string statuscode)
        {
            using (var db = new CustomerContext())
            {
                var paymentType = db.PaymentTypes.FirstOrDefault(p => p.Id == paymentTypeId);

                if (paymentType == null)
                    throw new ArgumentNullException("Payment Type does not exist");

                db.Attach(paymentType);
                var payment = new Payment();
                payment.Amount = amount;
                payment.Response = response;
                payment.StatusCode = statuscode;
                payment.PaymentType = paymentType;

                db.Save(paymentType);
                db.SaveChanges();

                return payment;
            }

        }

        public Payment UpdatePayment(long paymentId, Payment pay)
        {

            using (var db = new CustomerContext())
            {
                var payment = db.Payments.FirstOrDefault(p => p.Id == paymentId);
                if (payment == null)
                    throw new ArgumentNullException("Payment does not exist");
                db.Attach(payment);

                payment.Amount = pay.Amount;
                payment.Response = pay.Response;
                payment.StatusCode = pay.StatusCode;

                db.SaveChanges();
                return pay;
            }

        }

        public void DeletePayment(long paymentId)
        {
            using (var db = new CustomerContext())
            {
                var payment = db.Payments.FirstOrDefault(i => i.Id == paymentId);

                if (payment == null)
                    throw new ArgumentNullException("Payment does not exist");
                db.Attach(payment);

                payment.IsDeleted = true;
                db.SaveChanges();

            }
        }
        #endregion

        #region PaymentType

        public PaymentType GetPaymentType(long paymentTypeId)
        {
            using (var db = new CustomerContext())
            {
                return db.PaymentTypes.FirstOrDefault(p => p.Id == paymentTypeId);
            }
        }

        public PaymentType CreatePaymentType(long customerId, string source, int sourceId)
        {

            using (var db = new CustomerContext())
            {
                var customer = db.Customers.FirstOrDefault(p => p.Id == customerId);
                if (customer==null)
                    throw new ArgumentNullException("Customer does not exist");
                db.Attach(customer);

                var paymentType = new PaymentType();
                paymentType.Customer = customer;
                paymentType.Source = source;
                paymentType.SourceId = sourceId;

                db.Save(paymentType);

                return paymentType;

            }
        }

        public PaymentType UpdatePaymentType(long paymentTypeId, PaymentType ptype)
        {
            using (var db = new CustomerContext())
            {
                var paymentType = db.PaymentTypes.FirstOrDefault(p => p.Id == paymentTypeId);
                if (paymentType == null)
                    throw new ArgumentNullException("Payment Type does not exist");
                db.Attach(paymentType);

                paymentType.Source = ptype.Source;
                paymentType.SourceId = ptype.SourceId;

                db.SaveChanges();

                return paymentType;
            }


        }


        public void DeletePaymentType(long paymentTypeId)
        {
            using (var db = new CustomerContext())
            {
                var paymentType = db.PaymentTypes.FirstOrDefault(i => i.Id == paymentTypeId);

                if (paymentType == null)
                    throw new ArgumentNullException("Payment Type does not exist");
                db.Attach(paymentType);

                paymentType.IsDeleted = true;
                db.SaveChanges();

            }
        }

        #endregion

        public Item PurchaseItem(Customer c, Item item)
        {
            return item;
        }

        public bool VerifyBilling()
        {
            return false;
        }

        public Invoice ProcessInvoice()
        {
            var t = Invoice.Type.Invoice;
            return new Invoice(t);
        }

    

  
        /// Gets all the available products; those ones that have not been deleted.
        /// </summary>
        /// <returns>a list of active products; an empty list if no products were found.</returns>
        public  List<Item> GetAllProducts()
        {
            return new List<Item>(Item.LoadAllItems());
        }

  
        /// Retrieves the customer object associated with the user id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <returns>The customer object</returns>
        /// <exception cref="CustomerServiceException">Thrown on user id not found</exception>
        public Customer FindCustomerWithUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentNullException();

            try
            {
                using (var db = new CustomerContext())
                {
                    return db.Customers.FirstOrDefault(c => c.UserId == userId);             
                }
            }
            catch (Exception e)
            {
                throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound, 
                    "Could not find customer record with userId:" + userId, e);
            }
        }


        public IEnumerable<ItemPricing> FindItemPricingsWithItemId(int itemId) 
        {
            if (itemId <= 0)
                throw new ArgumentNullException();

            try
            {
                using (var db = new CustomerContext())
                {
                    return db.ItemPricings.Where(p => p.Item.Id == itemId).OrderByDescending(p=>p.CreatedDate).ToList();   
                }
            }
            catch (Exception e)
            {
                throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound, "Could not find customer record with itemId:" + itemId, e);
            }
        }

        public static void ModifyBalance(int customerId, decimal amount)
        {
            using (var db = new CustomerContext())
            {
               var customer= db.Customers.FirstOrDefault(c => c.Id == customerId);
               customer.ModifyBalance(amount);
            }
        }


    }
}
