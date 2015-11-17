using System;
using System.Runtime.Serialization;

namespace LeaseManager
{
   // [DataContract]
    public class Lease
    {
     //   [DataMember]
        public string ResourceName { get; set; }

      //  [DataMember]
        public string LeaseId { get; set; }

      //  [DataMember]
        public DateTime ValidUntilUtc { get; set; }
    }
}