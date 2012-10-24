using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Item
    {
        [DataMember]
        public int Id { get; private set; }
        [DataMember]
        public String Glacctno { get; set; }
        [DataMember]
        public int SubscriptionDays { get; set; }
        [DataMember]
        public String ItemClass { get; set; }
        [DataMember]
        public List<ItemPricing> ItemPricings { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public Item()
        {
            this.ItemPricings = new List<ItemPricing>();
        }

        /// <summary>
        /// Loads the object with the given id. Throws an exception if not found
        /// </summary>
        /// <param name="itemId">the id of the item to load</param>
        /// <returns>The object with the given id</returns>
        public static Item Load(int itemId)
        {
            Item bo = new Item();
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Item dalItem = findRecord(dc, itemId);
            map(dalItem, ItemPricing.LoadWithItemId(dc, itemId), bo);
            return bo;
        }

        private static DAL.Item findRecord(DAL.CustomersDataContext dc, int itemId)
        {
            return dc.Items.Where(record => record.Id == itemId).Where(record => !record.Deleted).Single();
        }

        /// <summary>
        /// Loads all the objects that are not in the deleted state. Throws an exception if
        /// none have be retrieved.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Item> LoadAllItems()
        {
            return findAllRecords(new DAL.CustomersDataContext());
        }

        private static IEnumerable<Item> findAllRecords(DAL.CustomersDataContext dc)
        {
            List<Item> allRecords = new List<Item>();

            foreach (var item in dc.Items.Where(record => !record.Deleted))
            {
                Item bo = new Item();
                map(item, ItemPricing.LoadWithItemId(dc, item.Id), bo);
                allRecords.Add(bo);
            }
            return allRecords;
        }

        public void Save()
        {
            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Item dalItem = null;

            if (this.Id == 0)
            {
                dalItem = new CustomerManagement.DAL.Item();
                map(this, dalItem);
                dc.Items.InsertOnSubmit(dalItem);
            }
            else
            {
                dalItem = findRecord(dc, this.Id);
                map(this, dalItem);
            }

            dc.SubmitChanges();
            this.Id = dalItem.Id;

            // save all new objects and update existing ones if modified
            foreach (var item in this.ItemPricings)
            {
                item.SaveDependent(dc, dalItem);
            }
            // delete all missing objects
            foreach (var item in ItemPricing.LoadWithItemId(dc, this.Id))
            {
                if (!this.ItemPricings.Contains(item))
                    ItemPricing.Delete(dc, item);
            }
        }

        public void Delete()
        {
            if (this.Id == 0)
                throw new InvalidOperationException("Object has not been persisted, and thus cannot be deleted");

            DAL.CustomersDataContext dc = new DAL.CustomersDataContext();
            DAL.Item dalItem = findRecord(dc, this.Id);
            dalItem.Deleted = true;
            dc.SubmitChanges();

            foreach (var item in this.ItemPricings)
            {
                ItemPricing.Delete(dc, item);
            }
        }

        // Used for mapping values coming from the DB. Note that the ID is copied
        private static void map(DAL.Item dalItem, IEnumerable<ItemPricing> itemPricings, Item i)
        {
            i.Id = dalItem.Id;
            i.Glacctno = dalItem.Glacctno;
            i.SubscriptionDays = dalItem.SubscriptionDays;
            i.ItemClass = dalItem.ItemClass;

            i.ItemPricings = new List<ItemPricing>(itemPricings);
        }

        // Used for storing user-supplied values that will be stored in the db. Caller needs to create DAL objects
        private static void map(Item i, DAL.Item dalItem)
        {
            bool isNew = dalItem.Id == 0;
            bool isModified = false;

            if (dalItem.Glacctno != i.Glacctno)
            {
                dalItem.Glacctno = i.Glacctno;
                isModified = true;
            }

            if (dalItem.SubscriptionDays != i.SubscriptionDays)
            {
                dalItem.SubscriptionDays = i.SubscriptionDays;
                isModified = true;
            }

            if (dalItem.ItemClass != i.ItemClass)
            {
                dalItem.ItemClass = i.ItemClass;
                isModified = true;
            }

            if (isNew)
            {
                dalItem.CreatedBy = i.LastChangedBy;
                dalItem.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalItem.LastChangedBy = i.LastChangedBy;
                dalItem.LastChangedDate = DateTime.Now;
            }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Item other = obj as Item;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.Glacctno != other.Glacctno)
                return false;

            if (this.ItemClass != other.ItemClass)
                return false;

            if (this.SubscriptionDays != other.SubscriptionDays)
                return false;

            if (this.ItemPricings.Count != other.ItemPricings.Count)
                return false;

            foreach (var item in this.ItemPricings)
            {
                if (!other.ItemPricings.Contains(item))
                    return false;
            }

            return true;
        }
    }
}
