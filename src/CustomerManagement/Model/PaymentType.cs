using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class PaymentType
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string CardType { get; set; }
        [DataMember]
        public string Glacctno { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public PaymentType()
        {
            this.LastChangedBy = 0; //TODO: get this
        }

        internal static PaymentType Load(int paymentTypeId)
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.PaymentType dalPaymentType = findRecord(dc, paymentTypeId);

            PaymentType bo = new PaymentType();
            map(dalPaymentType, bo);
            return bo;
        }

        private static void map(DAL.PaymentType dalPaymentType, PaymentType bo)
        {
            bo.Id = dalPaymentType.Id;
            bo.CardType = dalPaymentType.CardType;
            bo.Glacctno = dalPaymentType.Glacctno;
        }

        private static DAL.PaymentType findRecord(DAL.CustomersDataContext dc, int paymentTypeId)
        {
            return dc.PaymentTypes.Where(pt => pt.Id == paymentTypeId).Where(pt => !pt.Deleted).Single();
        }

        internal void SaveDependent(DAL.CustomersDataContext dc)
        {
            DAL.PaymentType dalPaymentType = null;

            if (this.Id == 0)
            {
                dalPaymentType = new DAL.PaymentType();
                map(dc, this, dalPaymentType);
                dc.PaymentTypes.InsertOnSubmit(dalPaymentType);
            }
            else
            {
                dalPaymentType = findRecord(dc, this.Id);
                map(dc, this, dalPaymentType);
            }

            dc.SubmitChanges();
            this.Id = dalPaymentType.Id;
        }

        private static void map(DAL.CustomersDataContext dc, PaymentType paymentType, DAL.PaymentType dalPaymentType)
        {
            bool isNew = paymentType.Id == 0;
            bool isModified = false;

            if (dalPaymentType.CardType != paymentType.CardType)
            {
                dalPaymentType.CardType = paymentType.CardType;
                isModified = true;
            }

            if (dalPaymentType.Glacctno != paymentType.Glacctno)
            {
                dalPaymentType.Glacctno = paymentType.Glacctno;
                isModified = true;
            }

            if (isNew)
            {
                dalPaymentType.CreatedBy = paymentType.LastChangedBy;
                dalPaymentType.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalPaymentType.LastChangedBy = paymentType.LastChangedBy;
                dalPaymentType.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            PaymentType other = obj as PaymentType;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.CardType != other.CardType)
                return false;

            if (this.Glacctno != other.Glacctno)
                return false;

            return true;
        }


        internal static void Delete(CustomerManagement.DAL.CustomersDataContext dc, PaymentType paymentType)
        {
            DAL.PaymentType dalPaymentType = findRecord(dc, paymentType.Id);
            dalPaymentType.Deleted = true;
            dc.SubmitChanges();
        }
    }
}
