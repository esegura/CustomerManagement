using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;

namespace CustomerManagement.Model
{
    
    [DataContract, Serializable]
    public abstract class Address: TrackableEntity
    {
        public enum TypeEnum {Default, Home, Other, BillTo, ShipTo } 
        public enum CountryCode         // INTERNATIONAL ABBREVIATION CODES: http://mic.imtc.gatech.edu/archive_directory_1/country.htm
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

        [DataMember]
        public string Note { get; set; }
        [DataMember]
        public Customer Customer { get; set; }
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
            //set { this._state = Enum.Parse(this.GetStateEnumType(), value).ToString(); }
            set { this._state = value; }
        }
        

        [DataMember]
        public String ZipCode { get; set; }

        public string country;
        [DataMember]
        public CountryCode Country
        {
            get
            {
                return (CountryCode)Enum.Parse(typeof(CountryCode), country, true);
            }
            set
            {
                this.country = value.ToString();
            }
        }
        
        public String addressType {get; set;}
        [DataMember]
        public TypeEnum AddressType
        {
            get
            {
                return (TypeEnum)Enum.Parse(typeof(TypeEnum), addressType, true);
            }
            set
            {
                this.addressType = value.ToString();
            }
        }

        protected Address() { }

        public abstract System.Type GetStateEnumType();  //?


    }
 
}
