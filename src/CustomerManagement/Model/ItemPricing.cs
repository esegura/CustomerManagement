using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.DAL;

namespace CustomerManagement.Model
{
    public class ItemPricing
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public String OverrideGlacctno { get; set; }
        [DataMember]
        public Decimal UnitPrice { get; set; }
        [DataMember]
        public String Description { get; set; }
        [DataMember]
        public String PromoCode { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public ItemPricing()
        {
            this.LastChangedBy = 0; // TODO: get this from somewhere
        }

        private ItemPricing(DAL.ItemPricing dalItemPricing):this()
        {
            map(dalItemPricing, this);
        }

        private void map(DAL.ItemPricing dalItemPricing, ItemPricing itemPricing)
        {
            itemPricing.Id = dalItemPricing.Id;
            itemPricing.ItemId = dalItemPricing.ItemId;
            itemPricing.StartDate = dalItemPricing.StartDate;
            itemPricing.EndDate = dalItemPricing.EndDate;
            itemPricing.OverrideGlacctno = dalItemPricing.OverrideGlacctno;
            itemPricing.UnitPrice = dalItemPricing.UnitPrice;
            itemPricing.Description = dalItemPricing.Description;
            itemPricing.PromoCode = dalItemPricing.PromoCode;
        }

        internal static IEnumerable<ItemPricing> LoadWithItemId(int itemId)
        {
            using (var dc = new CustomersDataContext())
               return LoadWithItemId(dc, itemId).ToList(); 
            // need the tolist because the datacontext will be disposed on return
            // causing the ienumerable to throw disposed if ennumerated later.
        }

        internal static IEnumerable<ItemPricing> LoadWithItemId(DAL.CustomersDataContext dc, int itemId)
        {
            foreach (var item in dc.ItemPricings.Where(ip => ip.ItemId == itemId).Where(ip => !ip.Deleted))
            {
                yield return new ItemPricing(item);
            }
        }

        internal void SaveDependent(DAL.CustomersDataContext dc, DAL.Item dalItem)
        {
            DAL.ItemPricing dalItemPricing = null;

            if (this.Id == 0)
            {
                dalItemPricing = new DAL.ItemPricing();
                map(this, dalItemPricing);
                dalItemPricing.Item = dalItem;
                this.ItemId = dalItem.Id;
                dc.ItemPricings.InsertOnSubmit(dalItemPricing);
            }
            else
            {
                dalItemPricing = findRecord(dc, this.Id);
                map(this, dalItemPricing);
            }

            dc.SubmitChanges();
            this.Id = dalItemPricing.Id;
        }

        private void map(ItemPricing itemPricing, DAL.ItemPricing dalItemPricing)
        {
            bool isNew = itemPricing.Id == 0;
            bool isModified = false;

            if (dalItemPricing.ItemId != itemPricing.ItemId)
            {
                dalItemPricing.ItemId = itemPricing.ItemId;
                isModified = true;
            }

            if (dalItemPricing.StartDate != itemPricing.StartDate)
            {
                dalItemPricing.StartDate = itemPricing.StartDate;
                isModified = true;
            }

            if (dalItemPricing.EndDate != itemPricing.EndDate)
            {
                dalItemPricing.EndDate = itemPricing.EndDate;
                isModified = true;
            }

            if (dalItemPricing.OverrideGlacctno != itemPricing.OverrideGlacctno)
            {
                dalItemPricing.OverrideGlacctno = itemPricing.OverrideGlacctno;
                isModified = true;
            }

            if (dalItemPricing.UnitPrice != itemPricing.UnitPrice)
            {
                dalItemPricing.UnitPrice = itemPricing.UnitPrice;
                isModified = true;
            }

            if (dalItemPricing.Description != itemPricing.Description)
            {
                dalItemPricing.Description = itemPricing.Description;
                isModified = true;
            }

            if (dalItemPricing.PromoCode != itemPricing.PromoCode)
            {
                dalItemPricing.PromoCode = itemPricing.PromoCode;
                isModified = true;
            }

            if (isNew)
            {
                dalItemPricing.CreatedBy = itemPricing.LastChangedBy;
                dalItemPricing.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalItemPricing.LastChangedBy = itemPricing.LastChangedBy;
                dalItemPricing.LastChangedDate = DateTime.Now;
            }
        }

        internal static void Delete(DAL.CustomersDataContext dc, ItemPricing itemPricing)
        {
            DAL.ItemPricing dalItemPricing = findRecord(dc, itemPricing.Id);
            dalItemPricing.Deleted = true;
            dc.SubmitChanges();
        }

        private static DAL.ItemPricing findRecord(CustomerManagement.DAL.CustomersDataContext dc, int itemPricingId)
        {
            return dc.ItemPricings.Where(ip => ip.Id == itemPricingId).Where(ip => !ip.Deleted).Single();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            ItemPricing other = obj as ItemPricing;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.ItemId != other.ItemId)
                return false;

            if (this.StartDate.Ticks/10000000 != other.StartDate.Ticks/10000000)
                return false;

            if (this.EndDate.Ticks/10000000 != other.EndDate.Ticks/10000000)
                return false;

            if (this.OverrideGlacctno != other.OverrideGlacctno)
                return false;

            if (this.UnitPrice != other.UnitPrice)
                return false;

            if (this.Description != other.Description)
                return false;

            if (this.PromoCode != other.PromoCode)
                return false;

            return true;
        }


    }
}
