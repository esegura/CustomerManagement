using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Model.AddressType
{
    [DataContract, Serializable]
    internal class AUAddress : Address
    {
        internal enum AUStateCode
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

        internal AUAddress() { }

        public override Type GetStateEnumType()
        {
            return typeof(AUStateCode);
        }
    }
}
