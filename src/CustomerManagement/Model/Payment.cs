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
    public class Payment:TrackableEntity
    {
        public enum StatusCodess {Success, Error} //make sure it can be "static"
      
        [DataMember]
        public PaymentType PaymentType { get; set; }
        [DataMember]
        public decimal Amount { get; set; }
        [DataMember]
        public string Response { get; set; } //response from payment system
        [DataMember]
        public string StatusCode { get; set; } 
   
        public Payment()
        {
        }

    }
}
