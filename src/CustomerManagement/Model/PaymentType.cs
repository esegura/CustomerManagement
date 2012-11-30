using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class PaymentType:TrackableEntity
    {

        [DataMember]
        public Customer Customer{ get; set; }
        
        public int SourceId {get; set;}

        public string Source { get; set; } //Credit Card
    
        public PaymentType()
        {
        }


    }
}
