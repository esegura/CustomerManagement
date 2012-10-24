using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class PaymentTransaction
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int PaymentId { get; set; }
        [DataMember]
        public PaymentType PaymentType { get; set; }
        [DataMember]
        public Decimal Amount { get; set; }
        [DataMember]
        public string AuthCode { get; set; }
        [DataMember]
        public PaymentStatusCode PaymentStatusCode { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public PaymentTransaction()
        {
            this.LastChangedBy = 0; //TODO: get this
        }

        internal PaymentTransaction(DAL.PaymentTransaction dalPaymentTransaction)
            : this()
        {
            map(dalPaymentTransaction, this);
        }

        private static void map(DAL.PaymentTransaction dalPaymentTransaction, PaymentTransaction paymentTransaction)
        {
            paymentTransaction.Id = dalPaymentTransaction.Id;
            paymentTransaction.PaymentId = dalPaymentTransaction.PaymentHeaderId;
            paymentTransaction.PaymentType = PaymentType.Load(dalPaymentTransaction.PaymentTypeId);
            paymentTransaction.Amount = dalPaymentTransaction.Amount;
            paymentTransaction.AuthCode = dalPaymentTransaction.AuthCode;
            paymentTransaction.PaymentStatusCode = PaymentStatusCode.LoadWithPaymentTransactionId(dalPaymentTransaction.Id);
        }

        internal static IEnumerable<PaymentTransaction> LoadWithPaymentId(DAL.CustomersDataContext dc, int paymentId)
        {
            foreach (var item in dc.PaymentTransactions.Where(pt => pt.PaymentHeaderId == paymentId).Where(pt => !pt.Deleted))
            {
                yield return new PaymentTransaction(item);
            }
        }



        internal void SaveDependent(CustomerManagement.DAL.CustomersDataContext dc, DAL.PaymentHeader dalPaymentHeader)
        {
            DAL.PaymentTransaction dalPaymentTransaction = null;

            this.PaymentType.SaveDependent(dc);

            if (this.Id == 0)
            {
                dalPaymentTransaction = new CustomerManagement.DAL.PaymentTransaction();
                map(dc, this, dalPaymentTransaction);
                dalPaymentTransaction.PaymentHeader = dalPaymentHeader;
                this.PaymentId = dalPaymentHeader.Id;
                dalPaymentTransaction.PaymentTypeId = this.PaymentType.Id;
                dc.PaymentTransactions.InsertOnSubmit(dalPaymentTransaction);
            }
            else
            {
                dalPaymentTransaction = findRecord(dc, this.Id);
                map(dc, this, dalPaymentTransaction);
            }

            dc.SubmitChanges();
            this.Id = dalPaymentTransaction.Id;

            this.PaymentStatusCode.SaveDependent(dc, dalPaymentTransaction);
        }

        private void map(DAL.CustomersDataContext dc, PaymentTransaction paymentTransaction, DAL.PaymentTransaction dalPaymentTransaction)
        {
            bool isNew = paymentTransaction.Id == 0;
            bool isModified = false;

            if (dalPaymentTransaction.PaymentTypeId != paymentTransaction.PaymentType.Id)
            {
                dalPaymentTransaction.PaymentTypeId = paymentTransaction.PaymentType.Id;
                isModified = true;
            }

            if (dalPaymentTransaction.PaymentHeaderId != paymentTransaction.PaymentId)
            {
                dalPaymentTransaction.PaymentHeaderId = paymentTransaction.PaymentId;
                isModified = true;
            }

            if (dalPaymentTransaction.Amount != paymentTransaction.Amount)
            {
                dalPaymentTransaction.Amount = paymentTransaction.Amount;
                isModified = true;
            }

            if (dalPaymentTransaction.AuthCode != paymentTransaction.AuthCode)
            {
                dalPaymentTransaction.AuthCode = paymentTransaction.AuthCode;
                isModified = true;
            }

            if (dalPaymentTransaction.StatusCode != Convert.ToInt32(paymentTransaction.PaymentStatusCode.StatusCode))
            {
                dalPaymentTransaction.StatusCode = Convert.ToInt32(paymentTransaction.PaymentStatusCode.StatusCode);
                isModified = true;
            }

            if (isNew)
            {
                dalPaymentTransaction.CreatedBy = paymentTransaction.LastChangedBy;
                dalPaymentTransaction.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalPaymentTransaction.LastChangedBy = paymentTransaction.LastChangedBy;
                dalPaymentTransaction.LastChangedDate = DateTime.Now;
            }
        }

        private static DAL.PaymentTransaction findRecord(DAL.CustomersDataContext dc, int paymentTransactionId)
        {
            return dc.PaymentTransactions.Where(pt => pt.Id == paymentTransactionId).Where(pt => !pt.Deleted).Single();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            PaymentTransaction other = obj as PaymentTransaction;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (!this.PaymentType.Equals(other.PaymentType))
                return false;

            if (this.PaymentId != other.PaymentId)
                return false;

            if (Decimal.Round(this.Amount, 3) != Decimal.Round(other.Amount, 3))
                return false;

            if (this.AuthCode != other.AuthCode)
                return false;
        
            if (!this.PaymentStatusCode.Equals(other.PaymentStatusCode))
                return false;

            return true;       
        }

        internal static void Delete(DAL.CustomersDataContext dc, PaymentTransaction item)
        {
            DAL.PaymentTransaction dalPaymentTransaction = findRecord(dc, item.Id);
            dalPaymentTransaction.Deleted = true;
            dc.SubmitChanges();

            PaymentType.Delete(dc, item.PaymentType);
            PaymentStatusCode.Delete(dc, item.PaymentStatusCode);
        }
    }
}
