using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using CustomerManagement.Properties;
using CustomerManagement.Util;
using CustomerManagement.Model;

namespace CustomerManagement.DAL
{
    public class CustomerContext: MockableDbContext, ICustomerContext
    {
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<Payment> Payments{ get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<ItemPricing> ItemPricings { get; set; }
        public virtual DbSet<PaymentType> PaymentTypes { get; set; }
        public virtual DbSet<Phone> Phones { get; set; }

        #region  Implementation ICustomerContext

        IQueryable<Address> ICustomerContext.Addresses {
            get { return this.Addresses; }
        }
        IQueryable<Customer> ICustomerContext.Customers
        {
            get { return this.Customers; }
        }
        IQueryable<Invoice> ICustomerContext.Invoices
        {
            get { return this.Invoices; }
        }

        IQueryable<InvoiceDetail> ICustomerContext.InvoiceDetails 
        {
            get { return this.InvoiceDetails; }
        }
        IQueryable<Item> ICustomerContext.Items
        {
            get { return this.Items; } 
        }
        IQueryable<ItemPricing> ICustomerContext.ItemPricings 
        { 
            get { return this.ItemPricings; } 
        }

        IQueryable<Payment> ICustomerContext.Payments
        { 
            get { return this.Payments; } 
        }
        IQueryable<PaymentType> ICustomerContext.PaymentTypes 
        { 
            get { return this.PaymentTypes; } 
        }
        IQueryable<Phone> ICustomerContext.Phones
        { 
            get { return this.Phones; } 
        }
        #endregion

        public CustomerContext() :
            base(Properties.Settings.Default.CustomersConnectionString)
        { }

        public CustomerContext(string nameOrConnectionString) :
            base(nameOrConnectionString)
        { }
    }
}
