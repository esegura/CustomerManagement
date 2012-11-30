using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Phone : TrackableEntity
    {
        public enum Type { Home, Work, Cell }

        [DataMember]
        public Customer Customer { get; set; }              // int CustomerId { get; private set; }
        [DataMember]
        public string CountryCallingCode { get; set; }
        [DataMember]
        public string Number { get; set; }
        [DataMember]
        public Type PhoneType { get; private set; }

        public Phone(Phone.Type at)
        {
            this.PhoneType = at;
        }

    }
}