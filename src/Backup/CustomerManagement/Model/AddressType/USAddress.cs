using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model.AddressType
{
    [DataContract, Serializable]
    internal class USAddress : Address
    {
        internal enum USStateCode
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

        public override Type GetStateEnumType()
        {
            return typeof(USStateCode);
        }
    }
}
