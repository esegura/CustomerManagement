using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.DAL;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    public class ItemPricing:TrackableEntity
    {

        [DataMember]
        public Item Item { get; set; }  
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public string OverrideGlacctno { get; set; }
        [DataMember]
        public decimal UnitPrice { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string PromoCode { get; set; }

        public ItemPricing()
        {
 
        }

        public ItemPricing(Item item)
        {
            this.Item = item;
        }



    }
}
