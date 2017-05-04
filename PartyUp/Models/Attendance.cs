using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    /**
     * The attendance model is designed to be a join table between 
     * events and users. The purpose of the join table is to break
     * the otherwise many to many relationship between the specified
     * two tables. This attendance model will contain the vent id
     * and the id of the person attending the event.
     * */
    public class Attendance
    {
        //allows for the information about the event to be requested
        public virtual Event Event { get; set; }
        //allows for the information about the attendee to be requested
        public ApplicationUser Attendee { get; set; }

        //this establishes that the eventid is a foreign key of this table
        [Key]
        [Column(Order = 1)]
        public int EventId { get; set; }

        //this establishes that the attendee id is a foreign key of this table
        [Key]
        [Column(Order = 2)]
        public string AttendeeId { get; set; }
    }
}