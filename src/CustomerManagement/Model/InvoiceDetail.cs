using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    public class InvoiceDetail
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int InvoiceHeaderId { get; set; }
        [DataMember]
        public int ItemPricingId { get; set; }
        [DataMember]
        public int ItemUnits { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        private InvoiceDetail()
        {
            this.LastChangedBy = 0; // TODO: get this from somewhere
        }

        public InvoiceDetail(ItemPricing ip):this()
        {
            this.ItemPricingId = ip.Id;
        }

        internal static IEnumerable<InvoiceDetail> LoadWithInvoiceId(DAL.CustomersDataContext dc, int invoiceId)
        {
            List<InvoiceDetail> invoiceDetails = new List<InvoiceDetail>();

            foreach (var item in dc.InvoiceDetails.Where(id => id.InvoiceHeaderId == invoiceId).Where(id => !id.Deleted))
            {
                InvoiceDetail id = new InvoiceDetail();
                map(item, id);
                invoiceDetails.Add(id);
            }

            return invoiceDetails;
        }

        private static void map(DAL.InvoiceDetail dalInvoiceDetail, InvoiceDetail id)
        {
            id.Id = dalInvoiceDetail.Id;
            id.InvoiceHeaderId = dalInvoiceDetail.InvoiceHeaderId;
            id.ItemPricingId = dalInvoiceDetail.ItemPricingId;
            id.ItemUnits = dalInvoiceDetail.ItemUnits;
        }

        internal static void Delete(DAL.CustomersDataContext dc, InvoiceDetail item)
        {
            DAL.InvoiceDetail dalInvoiceDetail = dc.InvoiceDetails.Where(id => id.Id == item.Id).Where(id => !id.Deleted).Single();
            dalInvoiceDetail.Deleted = true;
            dc.SubmitChanges();
        }

        internal void SaveDependent(DAL.CustomersDataContext dc, DAL.InvoiceHeader dalInvoiceHeader)
        {
            DAL.InvoiceDetail dalInvoiceDetail = null;

            if (this.Id == 0)
            {
                dalInvoiceDetail = new DAL.InvoiceDetail();
                map(this, dalInvoiceDetail);
                dalInvoiceDetail.InvoiceHeader = dalInvoiceHeader;
                this.InvoiceHeaderId = dalInvoiceHeader.Id; // handles the case where the whole graph is saved with one Save() call
                dc.InvoiceDetails.InsertOnSubmit(dalInvoiceDetail);
            }
            else
            {
                dalInvoiceDetail = dc.InvoiceDetails.Where(record => record.Id == this.Id).Where(id => !id.Deleted).Single();
                map(this, dalInvoiceDetail);
            }

            dc.SubmitChanges();
            this.Id = dalInvoiceDetail.Id;
        }

        private static void map(InvoiceDetail invoiceDetail, DAL.InvoiceDetail dalInvoiceDetail)
        {
            bool isNew = invoiceDetail.Id == 0;
            bool isModified = false;

            if (dalInvoiceDetail.InvoiceHeaderId != invoiceDetail.InvoiceHeaderId)
            {
                dalInvoiceDetail.InvoiceHeaderId = invoiceDetail.InvoiceHeaderId;
                isModified = true;
            }

            if (dalInvoiceDetail.ItemPricingId != invoiceDetail.ItemPricingId)
            {
                dalInvoiceDetail.ItemPricingId = invoiceDetail.ItemPricingId;
                isModified = true;
            }

            if (dalInvoiceDetail.ItemUnits != invoiceDetail.ItemUnits)
            {
                dalInvoiceDetail.ItemUnits = invoiceDetail.ItemUnits;
                isModified = true;
            }

            if (isNew)
            {
                dalInvoiceDetail.CreatedBy = invoiceDetail.LastChangedBy;
                dalInvoiceDetail.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalInvoiceDetail.LastChangedBy = invoiceDetail.LastChangedBy;
                dalInvoiceDetail.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            InvoiceDetail other = obj as InvoiceDetail;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.InvoiceHeaderId != other.InvoiceHeaderId)
                return false;

            if (this.ItemPricingId != other.ItemPricingId)
                return false;

            if (this.ItemUnits != other.ItemUnits)
                return false;

            return true;
        }
    }
}
