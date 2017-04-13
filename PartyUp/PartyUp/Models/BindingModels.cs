using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    public class EventBindingModel
    {
        public int EventId;
        public string Name;
        public DateTime EventDateTime;
        public string Details;
        public string Address1;
        public string Address2;
        public string City;
        public string State;
        public string Country;
        public string PostalCode;
        public string Latitude;
        public string Longitude;
        public string Username;
    }

    public class AttendanceBindingModel
    {
        public string EventName;
        public string UserName;
        public int EventId;
        public string UserId;
    }

    public class FriendBindingModel
    {
        public string newFriendId;
        public bool friendshipEstablished;
        public string userName;
        public string FirstName;
        public string LastName;
    }
}