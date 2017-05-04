using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PartyUp.Models
{
    /*
    Here are all the binding models that are used to return data to the user in
    a usable form. The event binding model, attendance binding model, and friend
    binding model will be located in this file 
    */

    /*
     The EventBindingModel converts the data received from either the application
     user or retreived from the database to a usable form. Json string returned
     converted to specific types. This will be what the api will return to the user
     when event is requested.
         */
    public class EventBindingModel
    {
        //int representing database unique id of event
        public int EventId;
        //string representing the name of the event
        public string Name;
        //DateTime of the event
        public DateTime EventDateTime;
        //string of the event details
        public string Details;
        //string with the address
        public string Address1;
        //string with additional address info
        public string Address2;
        //string with the event city
        public string City;
        //string with the event state
        public string State;
        //string with the eventy country
        public string Country;
        //string with the event postal code
        public string PostalCode;
        //string with the latitude of the event
        public string Latitude;
        //string with the longitude of the event
        public string Longitude;
        //string with the username of the owner of the event
        public string Username;
    }


    /**
     * The AttendanceBindingModel setsup how the data will be accepted by and returned to the user.
     * This binding model returns event detais such as who the requesting user is, who the owner of
     * the event is, and details about the event.
     * */
    public class AttendanceBindingModel
    {
        //string with the event name
        public string EventName;
        //string with the attending users username/email
        public string UserName;
        //string with the events id
        public int EventId;
        //string with the attending users specific id
        public string UserId;
        //string with the attending users first name
        public string FirstName;
        //string with the attending users last name
        public string LastName;
        //string with the event owners username
        public string EventOwner;
        //string with the details about the event
        public string EventDetails;
    }

    /**
     * The FriendBindingModel is used to define what will the webservice
     * will accept by the user and what the webservice will return. This
     * binding model will return the details about two users establishing
     * or attempting to establish a relationship. This will also return
     * data back showing the current status of the friendship and the details
     * of whichever user is attempting to accept/refuse the friendship
     * */
    public class FriendBindingModel
    {
        //string with the id of the requested friend
        public string newFriendId;
        //boolean representing whether or not the friendship is accepted
        public bool friendshipEstablished;
        //string containing username info of either user
        public string userName;
        //string containing FirstName of either user
        public string FirstName;
        //string containing LastName of either user
        public string LastName;
        //string containing phone number of eithe user
        public string PhoneNumber; 
    }
}