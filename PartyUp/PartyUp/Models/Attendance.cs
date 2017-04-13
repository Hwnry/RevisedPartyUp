using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    public class Attendance
    {
        
        public virtual Event Event { get; set; }
        public ApplicationUser Attendee { get; set; }

        [Key]
        [Column(Order = 1)]
        public int EventId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string AttendeeId { get; set; }
    }
}