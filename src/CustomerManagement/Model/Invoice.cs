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
        
        [DataMember]
        public Type InvoiceType { get; set; }
        
        [DataMember]
        public List<InvoiceDetail> InvoiceDetails { get; set; }
       
        public Invoice(Invoice.Type it)
        {
            this.InvoiceType = it;
            this.InvoiceDetails = new List<InvoiceDetail>();
        }


    }
}
