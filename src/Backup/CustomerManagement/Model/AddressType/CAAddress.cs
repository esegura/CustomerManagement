using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model.AddressType
{
    [DataContract, Serializable]
    internal class CAAddress : Address
    {
        internal enum CAStateCode
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

        public override Type GetStateEnumType()
        {
            return typeof(CAStateCode);
        }
    }
}
