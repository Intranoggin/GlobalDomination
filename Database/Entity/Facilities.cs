using System;
using System.Collections.Generic;

namespace Database.Entity
{
    public partial class Facilities
    {
        public Guid FacilityId { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? DeactivationDate { get; set; }
        public DateTime? LastModifiedUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string Telephone { get; set; }
        public string County { get; set; }
        public string Directions { get; set; }
        public Guid? FacilityTypeId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
