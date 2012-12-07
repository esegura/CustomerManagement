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
        public Invoice Invoice { get; set; }          
        [DataMember]
        public ItemPricing ItemPricing { get; set; } 
        [DataMember]
        public int ItemUnits { get; set; }


        public InvoiceDetail()
        {
         
        }


    }
}
