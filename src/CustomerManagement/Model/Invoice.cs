using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Invoice: TrackableEntity
    {
        public enum Type { Invoice, Credit, Refund, Void }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public string PaymentNote { get; set; } //put address info (deserialize) and customer info and current payment information

        [DataMember]
        public Customer Customer { get; set; }

        public string invoiceType { get; set; }

        [DataMember]
        public Type InvoiceType
        {
            get
            {
                return (Invoice.Type)Enum.Parse(typeof(Invoice.Type), invoiceType, true);
            }
            set
            {
                this.invoiceType = value.ToString();
            }
        }

        [DataMember]
        public List<InvoiceDetail> InvoiceDetails { get; set; }
     
        /*
        private Invoice(Invoice.Type it)
        {
            this.InvoiceType = it;
            this.InvoiceDetails = new List<InvoiceDetail>();
        }

        public Invoice(Customer cust, Invoice.Type it)
            : this(it)
        {
            this.Customer = cust;
        }
        */
        public Invoice()
        {

        }

    }
}
