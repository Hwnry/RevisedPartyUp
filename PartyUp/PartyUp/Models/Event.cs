using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public DateTime EventDateTime { get; set; }
        public string Details { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public ApplicationUser User { get; set; }
        public virtual ICollection<Attendance> Attendeees { get; set; }
    }
}