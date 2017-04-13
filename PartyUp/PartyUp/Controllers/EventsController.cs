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

            EventBindingModel temp = null;

            try
            {
                Event @event = db.Events.Include(e => e.User).Single(e => e.EventId == id);

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

                     return temp;
            }

            catch (Exception)
            {

                return temp; 
            } 

           
        }

        //// GET: api/Event/Attendees/id


        // Modified /////////////////////////////////////////////////////
        // POST: api/Events
        [ResponseType(typeof(Event))]
        public IHttpActionResult PostEvent(Event @event)
        {
            if (!ModelState.IsValid)
            {
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
            string currentUser = RequestContext.Principal.Identity.GetUserId();
            var owner = db.Events.Include(e => e.User).Single(e => e.EventId == id);
            if (owner.User.Id == currentUser)
            {
                Event @event = db.Events.Find(id);
                if (@event == null)
                {
                    return NotFound();
                }

                db.Events.Remove(@event);
                db.SaveChanges();

                return Ok(@event);
            }

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
            return db.Events.Count(e => e.EventId == id) > 0;
        }
    }
}