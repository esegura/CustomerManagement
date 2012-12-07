using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract]
    public class UiItem
    {
        [DataMember]
        public Item Item { get; set; }
        [DataMember]
        public ItemPricing Pricing { get; set; } //need to determine this by ItemPricing
        [DataMember]
        public int ItemUnits { get; set; }
 
    }
}
