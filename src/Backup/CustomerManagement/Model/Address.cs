using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model
{
    
    [DataContract, Serializable]
    public abstract class Address
    {
        public enum TypeEnum { Home, BillTo }
        public enum CountryCode
        {
            #region Country codes
            AC,
            AD,
            AE,
            AF,
            AG,
            AI,
            AL,
            AM,
            AN,
            AO,
            AQ,
            AR,
            AS,
            AT,
            AU,
            AW,
            AX,
            AZ,
            BA,
            BB,
            BD,
            BE,
            BF,
            BG,
            BH,
            BI,
            BJ,
            BM,
            BN,
            BO,
            BR,
            BS,
            BT,
            BV,
            BW,
            BY,
            BZ,
            CA,
            CC,
            CD,
            CF,
            CG,
            CH,
            CI,
            CK,
            CL,
            CM,
            CN,
            CO,
            CR,
            CS,
            CU,
            CV,
            CX,
            CY,
            CZ,
            DE,
            DJ,
            DK,
            DM,
            DO,
            DZ,
            EC,
            EE,
            EG,
            EH,
            ER,
            ES,
            ET,
            FI,
            FJ,
            FK,
            FM,
            FO,
            FR,
            FX,
            GA,
            GB,
            GD,
            GE,
            GF,
            GH,
            GI,
            GL,
            GM,
            GN,
            GP,
            GQ,
            GR,
            GS,
            GT,
            GU,
            GW,
            GY,
            HK,
            HM,
            HN,
            HR,
            HT,
            HU,
            ID,
            IE,
            IL,
            IM,
            IN,
            IO,
            IQ,
            IR,
            IS,
            IT,
            JE,
            JM,
            JO,
            JP,
            KE,
            KG,
            KH,
            KI,
            KM,
            KN,
            KP,
            KR,
            KW,
            KY,
            KZ,
            LA,
            LB,
            LC,
            LI,
            LK,
            LR,
            LS,
            LT,
            LU,
            LV,
            LY,
            MA,
            MC,
            MD,
            ME,
            MG,
            MH,
            MK,
            ML,
            MM,
            MN,
            MO,
            MP,
            MQ,
            MR,
            MS,
            MT,
            MU,
            MV,
            MW,
            MX,
            MY,
            MZ,
            NA,
            NC,
            NE,
            NF,
            NG,
            NI,
            NL,
            NO,
            NP,
            NR,
            NT,
            NU,
            NZ,
            OM,
            PA,
            PE,
            PF,
            PG,
            PH,
            PK,
            PL,
            PM,
            PN,
            PR,
            PS,
            PT,
            PW,
            PY,
            QA,
            RE,
            RO,
            RS,
            RU,
            RW,
            SA,
            SB,
            SC,
            SD,
            SE,
            SG,
            SH,
            SI,
            SJ,
            SK,
            SL,
            SM,
            SN,
            SO,
            SR,
            ST,
            SU,
            SV,
            SY,
            SZ,
            TC,
            TD,
            TF,
            TG,
            TH,
            TJ,
            TK,
            TM,
            TN,
            TO,
            TP,
            TR,
            TT,
            TV,
            TW,
            TZ,
            UA,
            UG,
            UK,
            UM,
            US,
            UY,
            UZ,
            VA,
            VC,
            VE,
            VG,
            VI,
            VN,
            VU,
            WF,
            WS,
            YE,
            YT,
            YU,
            ZA,
            ZM,
            ZR,
            ZW
            #endregion
        }
        // INTERNATIONAL ABBREVIATION CODES: http://mic.imtc.gatech.edu/archive_directory_1/country.htm

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CustomerId { get; set; }
        [DataMember] 
        public String Line1 { get; set; }
        [DataMember] 
        public String Line2 { get; set; }
        [DataMember] 
        public String City { get; set; }
        [DataMember]
        private string _state;
        public string State
        {
            get { return _state; }
            set { this._state = Enum.Parse(this.GetStateEnumType(), value).ToString(); }
        }
        [DataMember]
        public String ZipCode { get; set; }
        [DataMember]
        public CountryCode Country { get; set; }
        [DataMember]
        public TypeEnum AddressType { get; set; }

        //private Address(DAL.Address dalAddress)
        //{
        //    map(dalAddress, this);
        //}

        protected Address() { }
        //protected Address(System.Type stateEnumType, string state)
        //{
        //    if (stateEnumType.BaseType != typeof(System.Enum))
        //        throw new ArgumentException("StateEnumType must inherit from Enum. Use enums to enforce types on state strings");

        //    this.State = Enum.Parse(stateEnumType, state, true).ToString(); // Check the value against possible valid values
        //}

        public static explicit operator DAL.Address(Address a)
        {
            DAL.Address dalAddress = new DAL.Address();
            map(new DAL.CustomersDataContext(), a, dalAddress, -1);
            return dalAddress;
        }

        public abstract System.Type GetStateEnumType();

        internal void SaveDependent(DAL.CustomersDataContext dc, DAL.Customer c, int actionPerformerId)
        {
            DAL.Address dalAddress = null;

            if (this.Id == 0)
            {
                dalAddress = new CustomerManagement.DAL.Address();
                map(dc, this, dalAddress, actionPerformerId);
                dalAddress.Customer = c;
                this.CustomerId = c.Id; // handles the case where the whole graph is saved with one Save() call
                dc.Addresses.InsertOnSubmit(dalAddress);
            }
            else
            {
                dalAddress = dc.Addresses.Where(record => record.Id == this.Id).Single();
                map(dc, this, dalAddress, actionPerformerId);
            }

            dc.SubmitChanges();
            this.Id = dalAddress.Id;
       }

        private static void map(DAL.CustomersDataContext dc, Address a, DAL.Address dalAddress, int actionPerformerId)
        {
            bool isNew = a.Id == 0;
            bool isModified = false;

            if (dalAddress.Line1 != a.Line1)
            {
                dalAddress.Line1 = a.Line1;
                isModified = true;
            }

            if (dalAddress.Line2 != a.Line2)
            {
                dalAddress.Line2 = a.Line2;
                isModified = true;
            }

            if (dalAddress.City != a.City)
            {
                dalAddress.City = a.City;
                isModified = true;
            }

            //if (dalAddress.State != Enum.GetName(typeof(StateCode), a.State))
            //{
            //    dalAddress.State = Enum.GetName(typeof(StateCode), a.State);
            //    isModified = true;
            //}

            if (dalAddress.State != a.State)
            {
                dalAddress.State = a.State;
                isModified = true;
            }

            if (dalAddress.ZipCode != a.ZipCode)
            {
                dalAddress.ZipCode = a.ZipCode;
                isModified = true;
            }

            if (dalAddress.CountryCode != Enum.GetName(typeof(CountryCode), a.Country))
            {
                dalAddress.CountryCode = Enum.GetName(typeof(CountryCode), a.Country);
                isModified = true;
            }

            DAL.AddressType at = a.AddressType.findAddressType(dc);
            if ((dalAddress.AddressType == null) || (dalAddress.AddressType.Id != at.Id))
            {
                dalAddress.AddressType = at;
                isModified = true;
            }

            if (dalAddress.CustomerId != a.CustomerId)
            {
                dalAddress.CustomerId = a.CustomerId;
                isModified = true;
            }

            DateTime now = DateTime.Now;

            if (isNew)
            {
                dalAddress.CreatedBy = actionPerformerId;
                dalAddress.CreatedDate = now;
            }

            if (isModified)
            {
                dalAddress.LastChangedBy = actionPerformerId;
                dalAddress.LastChangedDate = now;
            }
        }

        internal static void map(DAL.Address dalAddress, Address address)
        {
            address.Line1 = dalAddress.Line1;
            address.Line2 = dalAddress.Line2;
            address.City = dalAddress.City;
//            address.State = (StateCode)Enum.Parse(typeof(StateCode), dalAddress.State);
            address.State = dalAddress.State;
            address.ZipCode = dalAddress.ZipCode;
            address.Country = (CountryCode)Enum.Parse(typeof(CountryCode), dalAddress.CountryCode);
            address.AddressType = (TypeEnum)Enum.Parse(typeof(TypeEnum), dalAddress.AddressType.TypeName);
            address.Id = dalAddress.Id;
            address.CustomerId = dalAddress.CustomerId;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            Address other = obj as Address;

            if (other == null)
                return false;

            if (this.Id != other.Id)
                return false;

            if (this.CustomerId != other.CustomerId)
                return false;

            if (this.Line1 != other.Line1)
                return false;

            if (this.Line2 != other.Line2)
                return false;

            if (this.City != other.City)
                return false;

            if (this.State != other.State)
                return false;

            if (this.ZipCode != other.ZipCode)
                return false;

            if (this.Country != other.Country)
                return false;

            if (this.AddressType != other.AddressType)
                return false;

            //if (this.LastChangedBy != other.LastChangedBy)
            //    return false;

            return true;
        }

        internal static IEnumerable<Address> LoadWithCustomerId(DAL.CustomersDataContext dc, int customerId)
        {
            foreach (var item in dc.Addresses.Where(a => a.CustomerId == customerId).Where(a => !a.Deleted))
            {
                //yield return new Address(item);
                yield return AddressFactory.Create(item);
            }
        }

        internal static void Delete(DAL.CustomersDataContext dc, Address item)
        {
            DAL.Address dalAddress = dc.Addresses.Where(a => a.Id == item.Id).Where(a => !a.Deleted).Single();
            dalAddress.Deleted = true;
            dc.SubmitChanges();
        }
    }

    public static class AddressTypeExtension
    {
        public static DAL.AddressType findAddressType(this Address.TypeEnum ct, DAL.CustomersDataContext dc)
        {
            // this must exist. If it doesn't, it's because the CustomerType table in the DB is out of sync with the enum below.
            return dc.AddressTypes.Where(rec => rec.TypeName == Enum.GetName(ct.GetType(), ct)).Single();
        }
    }
}
