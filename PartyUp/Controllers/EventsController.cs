using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.AspNet.Identity;
using PartyUp.Models;
using WebGrease.Css.Extensions;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Migrations;

/****
 * This controller specifically handles all of the requests pertaining to events.
 * 
 * GetEvents.
 * This controller returns all of the events found in the database.
 * 
 * GetEvents w/Id
 * This controller returns only the details about the event associated with the id.
 * 
 * PostEvent.
 * This is the controller that gets called when the user wants to create a new event.
 * Details are verified and placed into the database.
 * 
 * DeleteEvent.
 * This controller is called by the user who owns the event to delete the event.
 * After the user is verified, details about the events and the attending list
 * are removed from the database. 
 * 
 * Dispose.
 * Garbage collection for this controller
 * 
 * Event Exists.
 * Function to see if the event exists in the database
 */
namespace PartyUp.Controllers
{
    [Authorize]
    public class EventsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // Modified ///////////////////////////////////////////////////
        // GET: api/Events
        public IEnumerable<EventBindingModel> GetEvents()
        {
            // Create a list of events using a binding model
            List<EventBindingModel> eventList = new List<EventBindingModel>();

            // Loop through each event and add to the binding model. Make sure to include the user table in query.
            foreach (var evt in db.Events.Include(e => e.User))
            {
                //put all of the data about the current event into a binding model
                eventList.Add(new EventBindingModel
                {
                    EventId = evt.EventId,
                    Name = evt.Name,
                    EventDateTime = evt.EventDateTime,
                    Details = evt.Details,
                    Username = evt.User.UserName,
                    Address1 = evt.Address1,
                    Address2 = evt.Address2,
                    City = evt.City,
                    State = evt.State,
                    Country = evt.Country,
                    PostalCode = evt.PostalCode,
                    Longitude = evt.Longitude,
                    Latitude = evt.Latitude

                });
            }

            // return the binding model
            return eventList;
        }

        // GET: api/Events/5
        [ResponseType(typeof(Event))]
        public EventBindingModel GetEvent(int id)
        {
            //create a eventbinding model
            EventBindingModel temp = null;

            try
            {
                //include all of the details about the specified event
                Event @event = db.Events.Include(e => e.User).Single(e => e.EventId == id);

                //capture all of the event details
                temp = new EventBindingModel()
                {

                    EventId = @event.EventId,
                    Name = @event.Name,
                    EventDateTime = @event.EventDateTime,
                    Details = @event.Details,
                    Username = @event.User.UserName,
                    Address1 = @event.Address1,
                    Address2 = @event.Address2,
                    City = @event.City,
                    State = @event.State,
                    Country = @event.Country,
                    PostalCode = @event.PostalCode,
                    Longitude = @event.Longitude,
                    Latitude = @event.Latitude
                };

                //return these details
                return temp;
            }

            catch (Exception)
            {
                //return null, person does not have access to event
                return temp;
            }


        }

        //// GET: api/Event/Attendees/id


        // Modified /////////////////////////////////////////////////////
        // POST: api/Events
        [ResponseType(typeof(Event))]
        public IHttpActionResult PostEvent(Event @event)
        {
            //check to see if the modelstate is valid
            if (!ModelState.IsValid)
            {
                //return the request is bad
                return BadRequest(ModelState);
            }

            // Get the logged in User Id
            var currentUser = RequestContext.Principal.Identity.GetUserId();

            // Find the application user with id
            ApplicationUser user = db.Users.Single(u => u.Id == currentUser);

            // Add the user to the event
            @event.User = user;

            // Add the new event
            db.Events.AddOrUpdate(@event);

            // Save the changes to the database
            db.SaveChanges();

            return Ok();
        }

        // DELETE: api/Events/5
        [ResponseType(typeof(Event))]
        public IHttpActionResult DeleteEvent(int id)
        {
            //check who the requesting user is
            string currentUser = RequestContext.Principal.Identity.GetUserId();
            //get details about the owner of the event
            var owner = db.Events.Include(e => e.User).Single(e => e.EventId == id);
            //if the credentials match
            if (owner.User.Id == currentUser)
            {
                //find the event
                Event @event = db.Events.Find(id);
                //if the event is not found
                if (@event == null)
                {
                    //return error message not found
                    return NotFound();
                }
                //remove the specified event
                db.Events.Remove(@event);
                //save changes to the database
                db.SaveChanges();
                //return success code and deleted event details
                return Ok(@event);
            }
            //return not found to unauthorized users
            return NotFound();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EventExists(int id)
        {
            //return if the event has been found
            return db.Events.Count(e => e.EventId == id) > 0;
        }
    }
}