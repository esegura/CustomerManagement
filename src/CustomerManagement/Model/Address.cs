using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CustomerManagement.Util;
using CustomerManagement.Model.AddressType;

namespace CustomerManagement.Model
{
    
    [DataContract, Serializable]
    public class Address: TrackableEntity //Removed AddressFactory and changed from abstract class
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

        public enum USStateCode
        {
            AK,
            AL,
            AR,
            AS,
            AZ,
            CA,
            CO,
            CT,
            DC,
            DE,
            FL,
            GA,
            HI,
            IA,
            ID,
            IL,
            IN,
            KS,
            KY,
            LA,
            MA,
            MD,
            ME,
            MI,
            MN,
            MO,
            MP,
            MS,
            MT,
            NC,
            ND,
            NE,
            NH,
            NJ,
            NM,
            NV,
            NY,
            OH,
            OK,
            OR,
            PA,
            RI,
            SC,
            SD,
            TN,
            TX,
            UT,
            VA,
            VT,
            WA,
            WI,
            WV,
            WY,
        };

        public enum UKStateCode
        {
            BNE,
            BNS,
            BAS,
            BDF,
            BEX,
            BIR,
            BBD,
            BPL,
            BOL,
            BMH,
            BRC,
            BRD,
            BEN,
            BNH,
            BST,
            BRY,
            BKM,
            BUR,
            CLD,
            CAM,
            CMD,
            CHS,
            CON,
            COV,
            CRY,
            CMA,
            DAL,
            DER,
            DBY,
            DEV,
            DNC,
            DOR,
            DUD,
            DUR,
            EAL,
            ERY,
            ESX,
            ENF,
            ESS,
            GAT,
            GLS,
            GRE,
            HCK,
            HAL,
            HMF,
            HAM,
            HRY,
            HRW,
            HPL,
            HAV,
            HEF,
            HRT,
            HIL,
            HNS,
            IOW,
            IOS,
            ISL,
            KEC,
            KEN,
            KHL,
            KTT,
            KIR,
            KWL,
            LBH,
            LAN,
            LDS,
            LCE,
            LEC,
            LEW,
            LIN,
            LIV,
            LND,
            LUT,
            MAN,
            MDW,
            MRT,
            MDB,
            MIK,
            NET,
            NWM,
            NFK,
            NEL,
            NLN,
            NSM,
            NTY,
            NYK,
            NTH,
            NBL,
            NGM,
            NTT,
            OLD,
            OXF,
            PTE,
            PLY,
            POL,
            POR,
            RDG,
            RDB,
            RCC,
            RIC,
            RCH,
            ROT,
            RUT,
            SHN,
            SLF,
            SAW,
            SFT,
            SHF,
            SHR,
            SLG,
            SOL,
            SOM,
            SGC,
            STY,
            STH,
            SOS,
            SWK,
            STS,
            SKP,
            STT,
            STE,
            SFK,
            SND,
            SRY,
            STN,
            SWD,
            TAM,
            TFW,
            THR,
            TOB,
            TWH,
            TRF,
            WKF,
            WLL,
            WFT,
            WND,
            WRT,
            WAR,
            WBK,
            WSX,
            WSM,
            WGN,
            WIL,
            WNM,
            WRL,
            WOK,
            WLV,
            WOR,
            YOR,

            // Scotland

            ABE,
            ABD,
            ANS,
            AGB,
            CLK,
            DGY,
            DND,
            EAY,
            EDU,
            ELN,
            ERW,
            EDH,
            ELS,
            FAL,
            FIF,
            GLG,
            HLD,
            IVC,
            NAY,
            NLK,
            ORK,
            PKN,
            MLN,
            MRY,
            RFW,
            SCB,
            ZET,
            SAY,
            SLK,
            STG,
            WDU,
            WLN,

            // Northern Ireland

            ANT,
            ARD,
            ARM,
            BLA,
            BLY,
            BNB,
            BFS,
            CKF,
            CSR,
            CLR,
            CKT,
            CGV,
            DRY,
            DOW,
            DGN,
            FER,
            LRN,
            LMV,
            LSB,
            MFT,
            MYL,
            NYM,
            NTA,
            NDN,
            OMH,
            STB,

            // Wales

            BGW,
            BGE,
            CAY,
            CRF,
            CMN,
            CGN,
            CWY,
            DEN,
            FLN,
            GWN,
            AGY,
            MTY,
            MON,
            NTL,
            NWP,
            PEM,
            POW,
            RCT,
            SWA,
            TOF,
            VGL,
            WRX
        };

        public enum CAStateCode
        {
            AB,
            BC,
            MB,
            NB,
            NL,
            NS,
            NT,
            NU,
            ON,
            PE,
            QC,
            SK,
            YT,
        };

        public enum AUStateCode
        {
            CT,
            NS,
            NT,
            QL,
            SA,
            TS,
            VI,
            WA
        };


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

        private string _state;
        [DataMember]
        public string State
        {
            get { return _state; }
            set { this._state = value; }
        }
        
        [DataMember]
        public String ZipCode { get; set; }

        public string country {get; set;}
        
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
        
        public string addressType {get; set;}
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

        public Address()
        {
        }

        public System.Type GetStateEnumType()
        {

            var c = this.Country;
            Type result;

            switch (c)
            {
                case Address.CountryCode.US:
                    result = typeof(USStateCode);
                    break;
                case Address.CountryCode.UK:
                    result = typeof(UKStateCode);
                    break;
                case Address.CountryCode.CA:
                    result = typeof(CAStateCode);
                    break;
                case Address.CountryCode.AU:
                    result = typeof(AUAddress);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("country");

            }
            return result;
        }
 


        //protected Address() { }

        //public abstract System.Type GetStateEnumType();  
/*
        public static Address ConvertToSubClass(Address address) { // use to convert a loaded object from db into the correct subclass type. Next time put this in config file so can dynamically do this. See Ed's computer
  
            var country = address.Country;
            Address result;

            switch (country)
            {
                case Address.CountryCode.US:
                    result= (USAddress)address;
                    break;
                case Address.CountryCode.UK:
                    result= (UKAddress)address;
                    break;
                case Address.CountryCode.CA:
                    result= (CAAddress)address;
                    break;
                case Address.CountryCode.AU:
                   result= (AUAddress)address;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("country");

            }

            return address;
        }
*/
    }
 
}
