using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Payment
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int InvoiceId { get; set; }
        [DataMember]
        public short LastFourDigitsOfCreditCard { get; set; }
        [DataMember]
        public byte ExpirationMonth { get; set; }
        [DataMember]
        public short ExpirationYear { get; set; }
        [DataMember]
        public List<PaymentTransaction> PaymentTransactions { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public Payment()
        {
            this.PaymentTransactions = new List<PaymentTransaction>();
            this.LastChangedBy = 0; // TODO: get this from somewhere
        }

        // Load will return null if the dalItem is not found, or throw an exception if an error happens
        public static Payment Load(int paymentId)
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.PaymentHeader ph = findRecord(dc, paymentId);

            Payment bo = new Payment();
            map(ph, PaymentTransaction.LoadWithPaymentId(dc, paymentId), bo);

            return bo;
        }

        private static void map(DAL.PaymentHeader ph, IEnumerable<PaymentTransaction> paymentTransactions, Payment bo)
        {
            bo.Id = ph.Id;
            bo.InvoiceId = ph.InvoiceHeaderId;
            bo.LastFourDigitsOfCreditCard = ph.LastFourDigitsOfCreditCard;
            bo.ExpirationMonth = ph.ExpirationMonth;
            bo.ExpirationYear = ph.ExpirationYear;

            bo.PaymentTransactions = new List<PaymentTransaction>(paymentTransactions);
        }

        private static DAL.PaymentHeader findRecord(DAL.CustomersDataContext dc, int paymentId)
        {
            return dc.PaymentHeaders.Where(ph => ph.Id == paymentId).Where(ph => !ph.Deleted).Single();
        }

        public void Save()
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.PaymentHeader dalPaymentHeader = null;

            if (this.Id == 0)
            {
                dalPaymentHeader = new DAL.PaymentHeader();
                map(dc, this, dalPaymentHeader);
                dc.PaymentHeaders.InsertOnSubmit(dalPaymentHeader);
            }
            else
            {
                dalPaymentHeader = findRecord(dc, this.Id);
                map(dc, this, dalPaymentHeader);
            }

            dc.SubmitChanges();
            this.Id = dalPaymentHeader.Id;

            foreach (var item in this.PaymentTransactions)
            {
                item.SaveDependent(dc, dalPaymentHeader);
            }
            foreach (var item in PaymentTransaction.LoadWithPaymentId(dc, dalPaymentHeader.Id))
            {
                if (!this.PaymentTransactions.Contains(item))
                    PaymentTransaction.Delete(dc, item);
            }
        }

        private void map(DAL.CustomersDataContext dc, Payment payment, DAL.PaymentHeader dalPaymentHeader)
        {
            bool isNew = payment.Id == 0;
            bool isModified = false;

            if (dalPaymentHeader.InvoiceHeaderId != payment.InvoiceId)
            {
                dalPaymentHeader.InvoiceHeaderId = payment.InvoiceId;
                isModified = true;
            }

            if (dalPaymentHeader.LastFourDigitsOfCreditCard != payment.LastFourDigitsOfCreditCard)
            {
                dalPaymentHeader.LastFourDigitsOfCreditCard = payment.LastFourDigitsOfCreditCard;
                isModified = true;
            }

            if (dalPaymentHeader.ExpirationMonth != payment.ExpirationMonth)
            {
                dalPaymentHeader.ExpirationMonth = payment.ExpirationMonth;
                isModified = true;
            }

            if (dalPaymentHeader.ExpirationYear != payment.ExpirationYear)
            {
                dalPaymentHeader.ExpirationYear = payment.ExpirationYear;
                isModified = true;
            }

            if (isNew)
            {
                dalPaymentHeader.CreatedBy = payment.LastChangedBy;
                dalPaymentHeader.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalPaymentHeader.LastChangedBy = payment.LastChangedBy;
                dalPaymentHeader.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Payment other = obj as Payment;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.InvoiceId != other.InvoiceId)
                return false;

            if (this.LastFourDigitsOfCreditCard != other.LastFourDigitsOfCreditCard)
                return false;

            if (this.ExpirationMonth != other.ExpirationMonth)
                return false;

            if (this.ExpirationYear != other.ExpirationYear)
                return false;

            if (this.PaymentTransactions.Count != other.PaymentTransactions.Count)
                return false;

            foreach (var item in this.PaymentTransactions)
            {
                if (!other.PaymentTransactions.Contains(item))
                    return false;
            }

            return true;
        }

        public void Delete()
        {
            if (this.Id == 0)
                throw new InvalidOperationException("Object has not been persisted, and thus cannot be deleted");

            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.PaymentHeader dalPaymentHeader = findRecord(dc, this.Id);
            dalPaymentHeader.Deleted = true;
            dc.SubmitChanges();

            // delete all dependent objects
            foreach (var item in this.PaymentTransactions)
            {
                PaymentTransaction.Delete(dc, item);
            }
        }
    }
}
