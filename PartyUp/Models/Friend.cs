using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    /*
    Th Friend class will represent the data persisted by the database.
    Friends will have two foreign keys (sender and receiver) and a 
    boolean to show friendship has been established. This will ensure
    each friendship is unique and accessible by both parties
    */
    public class Friend
    {
        //the application user who initiates friendship
        public virtual ApplicationUser Me { get; set; }
        //the application user who receives friendship request
        public virtual ApplicationUser newFriend { get; set; }
        //boolean that ensures friendship is established
        public bool friendshipEstablished { get; set; }

        //This is establishing the first key for the intiator of the friendship
        [Key]
        [Column(Order = 1)]
        [ForeignKey("Me")]
        public string myID { get; set; }

        //This is establishing the second key for the receiver of the friendship
        [Key]
        [Column(Order = 2)]
        [ForeignKey("newFriend")]
        public string newFriendId { get; set; }

    }
}