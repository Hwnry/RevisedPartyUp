using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    public class Friend
    {
        
        public virtual ApplicationUser Me { get; set; }
        
        public virtual ApplicationUser newFriend { get; set; }

        public bool friendshipEstablished { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("Me")]
        public string myID { get; set; }

        [Key]
        [Column(Order = 2)]
        [ForeignKey("newFriend")]
        public string newFriendId { get; set; }

    }
}