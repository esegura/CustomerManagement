using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    public class InvoiceDetail: TrackableEntity
    {

        [DataMember]
        public Invoice Invoice { get; set; }           //   int InvoiceHeaderId { get; set; }
        [DataMember]
        public ItemPricing ItemPricing { get; set; }  //    int ItemPricingId { get; set; }
        [DataMember]
        public int ItemUnits { get; set; }

        private InvoiceDetail()
        {
         
        }

        public InvoiceDetail(ItemPricing ip):this()
        {
            this.ItemPricing = ip;
        }


    }
}
