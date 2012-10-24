using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    [DataContract, Serializable]
    public class Login
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember]
        public String FrontDoor { get; set; }
        [DataMember]
        private int LastChangedBy { get; set; }

        public Login()
        {
            this.LastChangedBy = 0; // TODO: get this from paymentStatusCodeId parameter
        }

        internal Login(DAL.Login dalLogin)
        {
            map(dalLogin, this);
            this.LastChangedBy = 0; // TODO: get this from paymentStatusCodeId parameter
        }

        public static explicit operator DAL.Login(Login l)
        {
            DAL.Login dalLogin = new DAL.Login();
            map(l, dalLogin);
            return dalLogin;
        }

        internal void SaveDependent(DAL.CustomersDataContext dc, DAL.Customer c)
        {
            DAL.Login dalLogin = null;

            if (this.Id == 0)
            {
                dalLogin = new CustomerManagement.DAL.Login();
                map(this, dalLogin);
                dalLogin.Customer = c;
                this.CustomerId = c.Id; // handles the case where the whole graph is saved with one Save() call
                dc.Logins.InsertOnSubmit(dalLogin);
            }
            else
            {
                dalLogin = dc.Logins.Where(record => record.Id == this.Id).Single();
                map(this, dalLogin);
            }

            dc.SubmitChanges();
            this.Id = dalLogin.Id;
        }

        private static void map(Login login, DAL.Login dalLogin)
        {
            bool isNew = login.Id == 0;
            bool isModified = false;

            if (dalLogin.CustomerId != login.CustomerId)
            {
                dalLogin.CustomerId = login.CustomerId;
                isModified = true;
            }

            if (dalLogin.FrontDoor != login.FrontDoor)
            {
                dalLogin.FrontDoor = login.FrontDoor;
                isModified = true;
            }

            if (isNew)
            {
                dalLogin.CreatedBy = login.LastChangedBy;
                dalLogin.CreatedDate = DateTime.Now;
            }

            if (isModified)
            {
                dalLogin.LastChangedBy = login.LastChangedBy;
                dalLogin.LastChangedDate = DateTime.Now;
            }
        }

        private void map(DAL.Login dalLogin, Login login)
        {
            login.Id = dalLogin.Id;
            login.CustomerId = dalLogin.CustomerId;
            login.FrontDoor = dalLogin.FrontDoor;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Login other = obj as Login;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.CustomerId != other.CustomerId)
                return false;

            if (this.LastChangedBy != other.LastChangedBy)
                return false;

            return true;
        }

        internal static IEnumerable<Login> LoadWithCustomerId(DAL.CustomersDataContext dc, int customerId)
        {
            foreach (var item in dc.Logins.Where(p => p.CustomerId == customerId).Where(p => !p.Deleted))
            {
                yield return new Login(item);
            }
        }

        internal static void Delete(DAL.CustomersDataContext dc, Login item)
        {
            DAL.Login dalLogin = dc.Logins.Where(l => l.Id == item.Id).Where(l => !l.Deleted).Single();
            dalLogin.Deleted = true;
            dc.SubmitChanges();
        }
    }
}
