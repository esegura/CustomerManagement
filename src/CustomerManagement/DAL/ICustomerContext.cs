using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using CustomerManagement.Model;
using CustomerManagement.Util;

namespace CustomerManagement.DAL
{
    public interface ICustomerContext: IMockableDBContext
    {
        IQueryable<Address> Addresses { get; }
        IQueryable<Customer> Customers { get; }
        IQueryable<Invoice> Invoices { get; }
        IQueryable<InvoiceDetail> InvoiceDetails { get; }
        IQueryable<Item> Items { get; }
        IQueryable<ItemPricing> ItemPricings { get; }
        IQueryable<Payment> Payments { get; }
        IQueryable<PaymentType> PaymentTypes { get; }
        IQueryable<Phone> Phones { get; }
    }
}
