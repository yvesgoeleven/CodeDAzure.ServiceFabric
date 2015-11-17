using System;
using System.Runtime.Serialization;

namespace Contracts
{
    [DataContract]
    public class LeaseResult
    {
        [DataMember]
        public bool Acquired { get; set; }

        [DataMember]
        public string ResourceName { get; set; }

        [DataMember]
        public string LeaseId { get; set; }

        [DataMember]
        public DateTime ValidUntilUtc { get; set; }
    }
}