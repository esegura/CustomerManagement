using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class CreditCard: IPaymentSource
    {
        public enum Statuses { Valid, Declined };
        [DataMember]
        public string Glacctno { get; set; }
        [DataMember]
        public string FullName {get; set;}
        [DataMember]
        public short LastFourDigitsOfCreditCard { get; set; }
        [DataMember]
        public byte ExpirationMonth { get; set; }
        [DataMember]
        public short ExpirationYear { get; set; }
        [DataMember]
        public string Status { get; set; }

        #region Implemention for IPaymentSource
        public string PaymentType
        {
            get { 
                return "creditcard";
            }
        }

        public void Process()
        {


        }
        #endregion
    }
}
