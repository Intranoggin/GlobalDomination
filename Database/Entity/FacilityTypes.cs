using System;
using System.Collections.Generic;

namespace Database.Entity
{
    public partial class FacilityTypes
    {
        public Guid FacilityTypeId { get; set; }
        public string Name { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}
