using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    public class OnAccountPayment //
    {
        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public string Glacctno { get; set; }

        #region Implemention for IPaymentSource
        public string PaymentType
        {
            get { return "onaccount"; }
        }

        public void Process()
        {

        }
        #endregion
    }
}
