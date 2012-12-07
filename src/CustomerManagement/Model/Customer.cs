using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using CustomerManagement.Model.AddressType;
using CustomerManagement.Util;
using CustomerManagement.DAL;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
   // [KnownType("GetKnownType")] Do we need this?
    public class Customer: TrackableEntity
    {

        [DataMember]
        public String FirstName { get; set; }
        [DataMember]
        public String MiddleName { get; set; }
        [DataMember]
        public String LastName { get; set; }
        [DataMember]
        public String Email { get; set; }
        [DataMember]
        public string CustomerType { get; set; }
   
        [DataMember]
        public List<Address> Addresses { get; set; }
        [DataMember]
        public List<Phone> Phones { get; set; }

        public List<PaymentType> PaymentTypes { get; set; }
        
        [DataMember]
        public decimal Balance { get; set; }
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public PaymentType DefaultPaymentType { get; set; }
/*
        public Customer(string type, int userId):this()
        {
            this.CustomerType = type;
            this.UserId = userId;
        }
*/
        public Customer() 
        {
            this.Addresses = new List<Address>();
            this.Phones = new List<Phone>();
        }

 /*
        private static System.Type[] GetKnownType()
        {
            return AddressFactory.GetAddressTypes();
        }
*/

    }



}
