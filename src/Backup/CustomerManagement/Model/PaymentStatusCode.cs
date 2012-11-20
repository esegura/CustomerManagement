using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class PaymentStatusCode
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int PaymentTransactionId { get; set; }
        [DataMember]
        public string StatusCode { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public PaymentStatusCode()
        {
            this.LastChangedBy = 0; //TODO: get this
        }

        internal PaymentStatusCode(DAL.PaymentStatusCode dalPaymentStatusCode):this()
        {
            this.Id = dalPaymentStatusCode.Id;
            this.PaymentTransactionId = dalPaymentStatusCode.PaymentTransactionId;
            this.StatusCode = dalPaymentStatusCode.StatusCode;
        }

        internal static PaymentStatusCode LoadWithPaymentTransactionId(int paymentTransactionId)
        {
            DAL.CustomersDataContext dc = new CustomerManagement.DAL.CustomersDataContext();
            DAL.PaymentStatusCode dalPaymentStatusCode = dc.PaymentStatusCodes.Where(psc => psc.PaymentTransactionId == paymentTransactionId).Where(psc => !psc.Deleted).Single();
            return new PaymentStatusCode(dalPaymentStatusCode);
        }

        internal void SaveDependent(CustomerManagement.DAL.CustomersDataContext dc, CustomerManagement.DAL.PaymentTransaction dalPaymentTransaction)
        {
            DAL.PaymentStatusCode dalPaymentStatusCode = null;

            if (this.Id == 0)
            {
                dalPaymentStatusCode = new DAL.PaymentStatusCode();
                map(dc, this, dalPaymentStatusCode);
                dalPaymentStatusCode.PaymentTransaction = dalPaymentTransaction;
                this.PaymentTransactionId = dalPaymentTransaction.Id;
                dc.PaymentStatusCodes.InsertOnSubmit(dalPaymentStatusCode);
            }
            else
            {
                dalPaymentStatusCode = findRecord(dc, this.Id);
                map(dc, this, dalPaymentStatusCode);
            }

            dc.SubmitChanges();
            this.Id = dalPaymentStatusCode.Id;
        }

        private static DAL.PaymentStatusCode findRecord(DAL.CustomersDataContext dc,int paymentStatusCodeId)
        {
 	        return dc.PaymentStatusCodes.Where(psc => psc.Id == paymentStatusCodeId).Where(psc => !psc.Deleted).Single();
        }

        private void map(DAL.CustomersDataContext dc, PaymentStatusCode paymentStatusCode, DAL.PaymentStatusCode dalPaymentStatusCode)
        {
            bool isNew = paymentStatusCode.Id == 0;
            bool isModified = false;

            if (dalPaymentStatusCode.PaymentTransactionId != paymentStatusCode.PaymentTransactionId)
            {
                dalPaymentStatusCode.PaymentTransactionId = paymentStatusCode.PaymentTransactionId;
                isModified = true;
            }

            if (dalPaymentStatusCode.StatusCode != paymentStatusCode.StatusCode)
            {
                dalPaymentStatusCode.StatusCode = paymentStatusCode.StatusCode;
                isModified = true;
            }

            if (isNew)
            {
                dalPaymentStatusCode.CreatedBy = paymentStatusCode.LastChangedBy;
                dalPaymentStatusCode.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalPaymentStatusCode.LastChangedBy = paymentStatusCode.LastChangedBy;
                dalPaymentStatusCode.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            PaymentStatusCode other = obj as PaymentStatusCode;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.PaymentTransactionId != other.PaymentTransactionId)
                return false;

            if (this.StatusCode != other.StatusCode)
                return false;

            return true;
        }


        internal static void Delete(CustomerManagement.DAL.CustomersDataContext dc, PaymentStatusCode paymentStatusCode)
        {
            DAL.PaymentStatusCode dalPaymentStatusCode = findRecord(dc, paymentStatusCode.Id);
            dalPaymentStatusCode.Deleted = true;
            dc.SubmitChanges();
        }
    }
}
