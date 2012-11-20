using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Phone
    {
        public enum Type { Home, Work, Cell }

        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public int CustomerId { get; private set; }
        [DataMember]
        public String CountryCallingCode { get; set; }
        [DataMember]
        public String Number { get; set; }
        [DataMember]
        public Type PhoneType { get; private set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public Phone(Phone.Type at)
        {
            this.PhoneType = at;
            this.LastChangedBy = 0; // TODO: get this from paymentStatusCodeId parameter
        }

        internal Phone(DAL.Phone dalPhone)
        {
            map(dalPhone, this);
            this.LastChangedBy = 0; // TODO: get this from paymentStatusCodeId parameter
        }

        public static explicit operator DAL.Phone(Phone p)
        {
            DAL.Phone dalPhone = new DAL.Phone();
            map(new DAL.CustomersDataContext(), p, dalPhone);
            return dalPhone;
        }

        internal void SaveDependent(DAL.CustomersDataContext dc, DAL.Customer c)
        {
            DAL.Phone dalPhone = null;

            if (this.Id == 0)
            {
                dalPhone = new CustomerManagement.DAL.Phone();
                map(dc, this, dalPhone);
                dalPhone.Customer = c;
                this.CustomerId = c.Id; // handles the case where the whole graph is saved with one Save() call
                dc.Phones.InsertOnSubmit(dalPhone);
            }
            else
            {
                dalPhone = dc.Phones.Where(record => record.Id == this.Id).Single();
                map(dc, this, dalPhone);
            }

            dc.SubmitChanges();
            this.Id = dalPhone.Id;
        }

        private static void map(DAL.Phone dalPhone, Phone phone)
        {
            phone.CountryCallingCode = dalPhone.CountryCallingCode;
            phone.Number = dalPhone.Number;
            phone.PhoneType = (Type)Enum.Parse(typeof(Type), dalPhone.PhoneType.TypeName);
            phone.Id = dalPhone.Id;
            phone.CustomerId = dalPhone.CustomerId;
        }

        private static void map(DAL.CustomersDataContext dc, Phone p, DAL.Phone dalPhone)
        {
            bool isNew = p.Id == 0;
            bool isModified = false;

            if (dalPhone.CountryCallingCode != p.CountryCallingCode)
            {
                dalPhone.CountryCallingCode = p.CountryCallingCode;
                isModified = true;
            }

            if (dalPhone.Number != p.Number)
            {
                dalPhone.Number = p.Number;
                isModified = true;
            }

            DAL.PhoneType pt = p.PhoneType.findPhoneType(dc);
            if ((dalPhone.PhoneType == null) || (dalPhone.PhoneType.Id != pt.Id))
            {
                dalPhone.PhoneType = pt;
                isModified = true;
            }

            if (dalPhone.CustomerId != p.CustomerId)
            {
                dalPhone.CustomerId = p.CustomerId;
                isModified = true;
            }

            if (isNew)
            {
                dalPhone.CreatedBy = p.LastChangedBy;
                dalPhone.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalPhone.LastChangedBy = p.LastChangedBy;
                dalPhone.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Phone other = obj as Phone;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.CustomerId != other.CustomerId)
                return false;

            if (this.CountryCallingCode != other.CountryCallingCode)
                return false;

            if (this.Number != other.Number)
                return false;

            if (this.PhoneType != other.PhoneType)
                return false;

            if (this.LastChangedBy != other.LastChangedBy)
                return false;

            return true;
        }

        internal static IEnumerable<Phone> LoadWithCustomerId(DAL.CustomersDataContext dc, int customerId)
        {
            foreach (var item in dc.Phones.Where(p => p.CustomerId == customerId).Where(p => !p.Deleted))
            {
                yield return new Phone(item);
            }
        }

        internal static void Delete(DAL.CustomersDataContext dc, Phone item)
        {
            DAL.Phone dalPhone = dc.Phones.Where(p => p.Id == item.Id).Where(p => !p.Deleted).Single();
            dalPhone.Deleted = true;
            dc.SubmitChanges();
        }
    }

    public static class PhoneTypeExtension
    {
        public static DAL.PhoneType findPhoneType(this Phone.Type ct, DAL.CustomersDataContext dc)
        {
            // this must exist. If it doesn't, it's because the CustomerType table in the DB is out of sync with the enum below.
            return dc.PhoneTypes.Where(rec => rec.TypeName == Enum.GetName(typeof(Phone.Type), ct)).Single();
        }
    }
}