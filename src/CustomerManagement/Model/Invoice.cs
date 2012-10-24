using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Invoice
    {
        public enum Type { Invoice, Credit, Refund, Winnings }

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public Type InvoiceType { get; set; }
        [DataMember]
        public List<InvoiceDetail> InvoiceDetails { get; set; }
        [DataMember]
        public int LastChangedBy { get; set; }

        public static Invoice Load(int invoiceId)
        {
            Invoice bo = null;
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.InvoiceHeader ih = findRecord(dc, invoiceId);

            if (ih != null)
            {
                bo = new Invoice((Type)Enum.Parse(typeof(Type), ih.InvoiceType));
                map(ih, InvoiceDetail.LoadWithInvoiceId(dc, invoiceId), bo);
            }

            return bo;
        }

        public Invoice(Invoice.Type it)
        {
            this.InvoiceType = it;
            this.InvoiceDetails = new List<InvoiceDetail>();
            this.LastChangedBy = 0; // TODO: get this from paymentStatusCodeId parameter
        }

        public void Save()
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.InvoiceHeader dalInvoiceHeader = null;

            if (this.Id == 0)
            {
                dalInvoiceHeader = new DAL.InvoiceHeader();
                map(this, dalInvoiceHeader);
                dc.InvoiceHeaders.InsertOnSubmit(dalInvoiceHeader);
            }
            else
            {
                dalInvoiceHeader = findRecord(dc, this.Id);
                map(this, dalInvoiceHeader);
            }

            dc.SubmitChanges();
            this.Id = dalInvoiceHeader.Id;

            // save all new objects and update existing ones if modified
            foreach (var item in this.InvoiceDetails)
            {
                item.SaveDependent(dc, dalInvoiceHeader);
            }
            // delete all missing objects
            foreach (var item in InvoiceDetail.LoadWithInvoiceId(dc, this.Id))
            {
                if (!this.InvoiceDetails.Contains(item))
                    InvoiceDetail.Delete(dc, item);
            }
        }

        private static void map(Invoice invoice, DAL.InvoiceHeader dalInvoiceHeader)
        {
            bool isNew = invoice.Id == 0;
            bool isModified = false;

            if (dalInvoiceHeader.CustomerId != invoice.CustomerId)
            {
                dalInvoiceHeader.CustomerId = invoice.CustomerId;
                isModified = true;
            }

            if (dalInvoiceHeader.InvoiceType != Enum.GetName(typeof(Type), invoice.InvoiceType))
            {
                dalInvoiceHeader.InvoiceType = Enum.GetName(typeof(Type), invoice.InvoiceType);
                isModified = true;
            }

            if (isNew)
            {
                dalInvoiceHeader.CreatedBy = invoice.LastChangedBy;
                dalInvoiceHeader.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalInvoiceHeader.LastChangedBy = invoice.LastChangedBy;
                dalInvoiceHeader.LastChangedDate = DateTime.Now;
            }
        }

        private static void map(DAL.InvoiceHeader ih, IEnumerable<InvoiceDetail> InvoiceDetails, Invoice i)
        {
            i.Id = ih.Id;
            i.CustomerId = ih.CustomerId;
            i.InvoiceType = (Type)Enum.Parse(typeof(Type), ih.InvoiceType);

            i.InvoiceDetails = new List<InvoiceDetail>(InvoiceDetails);
        }

        public void Delete()
        {
            if (this.Id == 0)
                throw new InvalidOperationException("Object has not been persisted, and thus cannot be deleted");

            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.InvoiceHeader dalInvoiceHeader = findRecord(dc, this.Id);
            dalInvoiceHeader.Deleted = true;
            dc.SubmitChanges();

            // delete all missing objects
            foreach (var item in this.InvoiceDetails)
            {
                InvoiceDetail.Delete(dc, item);
            }
        }

        private static DAL.InvoiceHeader findRecord(DAL.CustomersDataContext dc, int id)
        {
            return dc.InvoiceHeaders.Where(ih => ih.Id == id).Where(ih => !ih.Deleted).Single();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Invoice other = obj as Invoice;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.CustomerId != other.CustomerId)
                return false;

            if (this.InvoiceType != other.InvoiceType)
                return false;

            if (this.InvoiceDetails.Count != other.InvoiceDetails.Count)
                return false;

            foreach (var item in this.InvoiceDetails)
            {
                if (!other.InvoiceDetails.Contains(item))
                    return false;
            }

            return true;
        }

    }
}
