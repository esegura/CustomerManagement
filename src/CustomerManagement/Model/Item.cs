using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;
using CustomerManagement.DAL;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Item:TrackableEntity
    {
        [DataMember]
        public string Glacctno { get; set; }
        [DataMember]
        public int SubscriptionDays { get; set; }
        [DataMember]
        public string ItemClass { get; set; }
        [DataMember]
        public List<ItemPricing> ItemPricings { get; set; }
        
        public Item()
        {
            this.ItemPricings = new List<ItemPricing>();
        }
 
    }
}
