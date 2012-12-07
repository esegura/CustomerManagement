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
        public Customer Customer { get; set; }              
        [DataMember]
        public string CountryCallingCode { get; set; }
        [DataMember]
        public string Number { get; set; }

        public string phone { get; set; }
        [DataMember]
        public Type PhoneType {
            get
            {
                return (Type)Enum.Parse(typeof(Type), phone, true);
            }
            set
            {
                this.phone= value.ToString();
            }
        }

        public Phone()
        {

        }

    }
}