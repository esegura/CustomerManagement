using System;
using System.Linq;
using System.Collections.Generic;
using CustomerManagement.Model;
using CustomerManagement.DAL;

namespace CustomerManagement
{
    //TODO Make this the only access point to CRUD for the customer objects
    public class CustomerManager
    {
        protected virtual Func<CustomerContext> Db { get { return ()=>new CustomerContext(); } }

        public CustomerManager() {
        }
        #region Customers

        public Customer GetCustomer(long customerId) {
            using (var db = Db())
            {
                return db.Customers.FirstOrDefault(p => p.Id == customerId);
            }
        }

        public Customer CreateCustomer(string type, string firstname, string lastname,  string email,  int userId)
        {
            if (string.IsNullOrWhiteSpace(firstname.Trim()))
                throw new ArgumentNullException("firstname");

            if (string.IsNullOrWhiteSpace(lastname.Trim()))
                throw new ArgumentNullException("lastname");

            if (string.IsNullOrWhiteSpace(email.Trim()))
                throw new ArgumentNullException("email");

            if (string.IsNullOrWhiteSpace(type.Trim()))
                throw new ArgumentNullException("customer type");

            if (userId <= 0)
                throw new ArgumentNullException("userId");

            Customer newCustomer = new Customer { CustomerType = type, UserId = userId, FirstName = firstname.Trim(), LastName = lastname.Trim(), Email = email.Trim() };

            using (var db = Db())
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

            using (var db = Db())
            {         
                var customer = db.Customers.FirstOrDefault(p => p.Id == customerId);
                if (customer==null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);
                customer.FirstName = c.FirstName.Trim();
                customer.LastName = c.LastName.Trim();
                if (!string.IsNullOrWhiteSpace(c.MiddleName))
                customer.MiddleName = c.MiddleName.Trim();
                customer.Email = c.Email.Trim();
                customer.CustomerType = c.CustomerType;

                db.SaveChanges();

                return c;
            }
        }

        public void DeleteCustomer(long customerId)
        {
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.Addresses.FirstOrDefault(p => p.Id == addressId);
            }
        }

        public Address CreateAddress(long customerId, string address1, string address2, string city, string state, string zip, Address.CountryCode country, Address.TypeEnum type)
        {
            using (var db = Db())
            {
                if (string.IsNullOrWhiteSpace(address1)|| string.IsNullOrWhiteSpace(city)|| string.IsNullOrWhiteSpace(state)|| string.IsNullOrWhiteSpace(zip))
                    throw new ArgumentNullException("Missing Address value");
                
                var customer=db.Customers.FirstOrDefault(c=>c.Id==customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);

                Address address = new Address(); // AddressFactory.Create(country);
                address.Country = country;
                address.Line1 = address1.Trim();
                if (!string.IsNullOrWhiteSpace(address2.Trim()))
                address.Line2 = address2.Trim();
                address.City = city.Trim();
                address.State = Enum.Parse(address.GetStateEnumType(), state.Trim()).ToString(); ;//add validation
                address.ZipCode = zip.Trim();
                address.AddressType = type;   
                address.Customer = customer;
 
                db.Save(address);
                db.SaveChanges();

                return address;
            }
        }

        public Address UpdateAddress(long addressId, Address add) //Will not work properly for changingcountry
        {
            if (string.IsNullOrWhiteSpace(add.Line1) || string.IsNullOrWhiteSpace(add.City) || string.IsNullOrWhiteSpace(add.State) || string.IsNullOrWhiteSpace(add.ZipCode))
                throw new ArgumentNullException("Missing Address value");

            using (var db = Db())
            {
                var address = db.Addresses.FirstOrDefault(a => a.Id == addressId);

                if (address ==null)
                    throw new ArgumentNullException("Address does not exist");

                db.Attach(address);
                address.Country = add.Country;
                address.State = Enum.Parse(address.GetStateEnumType(), add.State.Trim()).ToString(); 
                address.Line1 = add.Line1.Trim();
                if (!string.IsNullOrWhiteSpace(add.Line2.Trim()))
                address.Line2 = add.Line2.Trim();
                address.City = add.City.Trim();
    
                address.AddressType = add.AddressType;
                address.Note = add.Note;
                address.ZipCode = add.ZipCode;
  

                db.SaveChanges();
                return add;
            }
        }

        public void DeleteAddress(long addressId)
        {
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.Phones.FirstOrDefault(p => p.Id == phoneId);
            }
        }
        
        public Phone CreatePhone(long customerId, string number, string code, Phone.Type type) {
                      
            if (string.IsNullOrWhiteSpace(number) || string.IsNullOrWhiteSpace(code))
                throw new ArgumentNullException("Missing Phone value");
            using (var db = Db())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");

                db.Attach(customer);

                var phone = new Phone();
                phone.PhoneType   = type;
                phone.Number = number.Trim();
                phone.CountryCallingCode = code.Trim();
                phone.Customer = customer;

                db.Save(phone);
                db.SaveChanges();

                return phone;
            }        
        }

        public Phone UpdatePhone(long phoneId, Phone ph)
        {
            if (string.IsNullOrWhiteSpace(ph.Number.Trim()) || string.IsNullOrWhiteSpace(ph.CountryCallingCode.Trim()))
                throw new ArgumentNullException("Missing Phone value");
      
            using (var db = Db())
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
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.Invoices.Include("InvoiceDetails").FirstOrDefault(p => p.Id == invoiceId);
            }
        }

        public Invoice CreateInvoice(long customerId, Invoice.Type type, List<UiItem> uiitems, string note, string paymentnote ) // need to include itempricings/itemunits
        {
            using (var db = Db())
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == customerId);

                if (customer == null)
                    throw new ArgumentNullException("Customer does not exist");
                if (uiitems.Count==0)
                    throw new ArgumentNullException("There are no items");
                if (type==Invoice.Type.Void)
                    throw new ArgumentNullException("Cannot create void invoice");

                db.Attach(customer);

                var invoice = new Invoice
                {
                    Customer=customer,
                    InvoiceType=type,
                    Note=note.Trim(),
                    PaymentNote=paymentnote.Trim()
                };

                //Create Invoice Details

                invoice.InvoiceDetails = MapInvoiceDetails(uiitems);

                db.Save(invoice);
                db.SaveChanges();

                return invoice;
            }

        }


        public Invoice UpdateInvoice(long  invoiceId, Invoice inv) 
        {
            using (var db = Db())
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

        public void VoidInvoice(long invoiceId)
        {
            using (var db = Db())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);
                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");
                db.Attach(invoice);
                invoice.InvoiceType = Invoice.Type.Void;
                db.SaveChanges();
            }
        }

        public void DeleteInvoice(long invoiceId) //Should we just void invoices instead of changing to IsDelete to true???
        {
            using (var db = Db())
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

        public InvoiceDetail GetInvoiceDetail(long invoiceDetailId)
        {
            using (var db = Db())
            {
                return db.InvoiceDetails.Include("ItemPricing").FirstOrDefault(p => p.Id == invoiceDetailId);
            }
        }

        //Should add GetInvoiceDetails????
        public List<InvoiceDetail> GetInvoiceDetails(long invoiceDetailId)
        {
            using (var db = Db())
            {
                return db.InvoiceDetails.Include("ItemPricing").Where(p => p.Id == invoiceDetailId).ToList();
            }
        }

        public List<InvoiceDetail> MapInvoiceDetails(List<UiItem> uiitems)
        {
            List<InvoiceDetail> details = new List<InvoiceDetail>();

            foreach (var uiitem in uiitems)
            {
                var detail = new InvoiceDetail {ItemPricing=uiitem.Pricing,  ItemUnits=uiitem.ItemUnits };
                details.Add(detail);
            }

            return details;
        }

        public InvoiceDetail CreateInvoiceDetail(long invoiceId, long itempricingId, int itemunits)
        {
            using (var db = Db())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");

                var itempricing = db.ItemPricings.FirstOrDefault(i => i.Id == itempricingId);

                if (itempricing == null)
                    throw new ArgumentNullException("Item Pricing does not exist");
                
                db.Attach(invoice);
                db.Attach(itempricing);

                var invoicedetail = new InvoiceDetail();

                invoicedetail.ItemUnits = itemunits;
                invoicedetail.Invoice = invoice;
                invoicedetail.ItemPricing = itempricing;

                db.Save(invoicedetail);
                return invoicedetail;
            }

        }

        public InvoiceDetail UpdateInvoiceDetail(long invoiceDetailId, InvoiceDetail invd)
        {
            using (var db = Db())
            {
                var invoiceDetail = db.InvoiceDetails.FirstOrDefault(i => i.Id == invoiceDetailId);

                if (invoiceDetail == null)
                    throw new ArgumentNullException("Invoice does not exist");
                if (invd.ItemPricing == null)
                    throw new ArgumentNullException("Item Pricing does not exist");
                
                db.Attach(invoiceDetail);

                invoiceDetail.ItemUnits = invd.ItemUnits;
                invoiceDetail.ItemPricing = invd.ItemPricing;


                db.SaveChanges();

                return invoiceDetail;
            }

        }

        public void DeleteInvoiceDetail(long invoiceDetailId)
        {
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.Items.FirstOrDefault(p => p.Id == itemId);
            }
        }

        public Item CreateItem(string gno, int subdays, string itemClass)
        {
            if (string.IsNullOrWhiteSpace(gno.Trim()) || string.IsNullOrWhiteSpace(itemClass.Trim()) )
                 throw new ArgumentNullException("Missing Item Information");
            using (var db=Db()){
            
                var item=new Item();
                item.Glacctno=gno.Trim();
                item.SubscriptionDays=subdays;
                item.ItemClass=itemClass.Trim();

                db.Save(item);
                db.SaveChanges();
                return item;      
            }       
        }

        public Item UpdateItem(long itemId, Item it)
        {
            using (var db = Db())
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
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.ItemPricings.FirstOrDefault(p => p.Id == itemPricingId);
            }
        }


        public ItemPricing CreateItemPricing(long itemId, DateTime start, DateTime end, string overrideGlacctno, decimal unitprice, string description, string promo )
        {
            using (var db = Db())
            {
                var item = db.Items.FirstOrDefault(i => i.Id == itemId);

                if (item == null)
                    throw new ArgumentNullException("Item does not exist");

                var itempricing = new ItemPricing
                {
                    StartDate = start,
                    EndDate = end,
                    OverrideGlacctno = overrideGlacctno,
                    UnitPrice = unitprice,
                    Description = description,
                    PromoCode = promo
                };

                db.Save(itempricing);
                db.SaveChanges();

                return itempricing;
            }
        }

        public ItemPricing UpdateItemPricing(long itemPricingId, ItemPricing ip)
        {

            throw new Exception("This is not a valid function, itempricing should never be updated");
           
            using (var db = Db())
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
            using (var db = Db())
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
            using (var db = Db())
            {
                return db.Payments.FirstOrDefault(p => p.Id == paymentId);
            }
        }

        public Payment CreatePayment(long invoiceId, long paymentTypeId, decimal amount, string response, string statuscode)// link to invoice
        {
            using (var db = Db())
            {
                var invoice = db.Invoices.FirstOrDefault(i => i.Id == invoiceId);

                if (invoice == null)
                    throw new ArgumentNullException("Invoice does not exist");

                var paymentType = db.PaymentTypes.FirstOrDefault(p => p.Id == paymentTypeId);

                if (paymentType == null)
                    throw new ArgumentNullException("Payment Type does not exist");

               

                db.Attach(paymentType);
                db.Attach(invoice);
                var payment = new Payment 
                { 
                    Invoice=invoice,  
                    Amount = amount, 
                    Response = response, 
                    StatusCode = statuscode, 
                    PaymentType = paymentType 
                };

                db.Save(payment);
                db.SaveChanges();

                return payment;
            }

        }

        public Payment UpdatePayment(long paymentId, Payment pay)
        {

            using (var db = Db())
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
            throw new Exception("This is not a valid function, payment shoudl never be deleted");

            using (var db = Db())
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
            using (var db = Db())
            {
                return db.PaymentTypes.FirstOrDefault(p => p.Id == paymentTypeId);
            }
        }

        public PaymentType CreatePaymentType(long customerId, string source, int sourceId)
        {

            using (var db = Db())
            {
                var customer = db.Customers.FirstOrDefault(p => p.Id == customerId);
                if (customer==null)
                    throw new ArgumentNullException("Customer does not exist");
                db.Attach(customer);

                var paymentType = new PaymentType
                {
                    Customer = customer,
                    Source = source,
                    SourceId = sourceId
                };
                db.Save(paymentType);
                db.SaveChanges();

                return paymentType;

            }
        }

        public PaymentType UpdatePaymentType(long paymentTypeId, PaymentType ptype)
        {
            using (var db = Db())
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
            using (var db = Db())
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
            return null; // new Invoice(t);
        }
 
        //Gets all the available products; those ones that have not been deleted.

        public  List<Item> GetAllProducts()
        {
            using (var db = new CustomerContext())
            {
                return db.Items.Where(item => !item.IsDeleted).ToList();
            }
        }

  
        // Retrieves the customer object associated with the user id
        public Customer FindCustomerWithUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentNullException();

            using (var db = Db())
            {
                var customer = db.Customers.FirstOrDefault(c => c.UserId == userId);
                if (customer==null)
                    throw new CustomerServiceException(CustomerServiceException.ErrorCode.CustomerNotFound,"Could not find customer record with userId:" + userId, null);
                return customer;           
            }
        }


        public IEnumerable<ItemPricing> FindItemPricingsWithItemId(int itemId) 
        {
            if (itemId <= 0)
                throw new ArgumentNullException();

            using (var db = Db())
            {
                var itempricings= db.ItemPricings.Where(p => p.Item.Id == itemId).OrderByDescending(p => p.CreatedDate).ToList();         
                return itempricings;
            }
        }


        //modify customer balance
        public void ModifyBalance(long customerId, decimal amount)
        {
            using (var db = Db())
            {
               var customer= db.Customers.FirstOrDefault(c => c.Id == customerId);
               db.Attach(customer);
               customer.Balance += amount;
               db.SaveChanges();

            }
        }


    }
}
