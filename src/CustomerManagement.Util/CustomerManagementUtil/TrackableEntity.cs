using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CustomerManagement.Util
{
    public class TrackableEntity
    {
        [DataMember]
        public virtual long Id { get; set; }

        [DataMember]
        public virtual bool IsDeleted { get; set; }

        //Hide for now
        //[DataMember]
        public virtual DateTimeOffset CreatedDate { get; set; }

        //[DataMember]
        public virtual int CreatedBy { get; set; }

        //[DataMember]
        public virtual DateTimeOffset LastChangedDate { get; set; }

        //[DataMember]
        public virtual int LastChangedBy { get; set; }
    }
}
