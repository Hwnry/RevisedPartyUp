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

namespace PartyUp.Controllers
{
    [Authorize]
    public class AttendancesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Modified /////////////////////////////////////////////////
        // GET: api/Attendances
        public IEnumerable<AttendanceBindingModel> GetAttendances()
        {
            // Create a list of events using a binding model
            List<AttendanceBindingModel> attendeeList = new List<AttendanceBindingModel>();

            // Loop through each event and add to the binding model. Make sure to include the user table and event table in query.
            foreach (var at in db.Attendances.Include(a => a.Event).Include(a => a.Attendee))
            {
                attendeeList.Add(new AttendanceBindingModel
                {
                    EventName = at.Event.Name,
                    UserName = at.Attendee.UserName,
                    EventId = at.EventId,
                    UserId = at.AttendeeId
                });
            }
            return attendeeList;
        }

        // GET: api/Attendances/5
        [ResponseType(typeof(List<AttendanceBindingModel>))]
        [Route("api/Attendances/{eventId}")]
        public List<AttendanceBindingModel> GetEventAttendees(int eventId)
        {

            var aList = db.Events.Include(e => e.Attendeees).Include(e => e.User).Single(e => e.EventId == eventId);
            List<AttendanceBindingModel> temp = new List<AttendanceBindingModel>();
            foreach (var a in aList.Attendeees)
            {
                temp.Add(new AttendanceBindingModel { UserName = db.Users.Single(u => u.Id == a.AttendeeId).Email });
            }
            return temp;
        }

        // PUT: api/Attendances/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAttendance(int id, Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != attendance.EventId)
            {
                return BadRequest();
            }

            db.Entry(attendance).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttendanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // Modified //////////////////////////////////////////////////
        // POST: api/Attendances
        [ResponseType(typeof(Attendance))]
        public IHttpActionResult PostAttendance(Attendance attendance)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Attendances.Add(attendance);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (AttendanceExists(attendance.EventId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            // Changed return type to OK
            return Ok();
        }

        // DELETE: api/Attendances
        [Route("api/attendances/{eventId}/{userId}")]
        [ResponseType(typeof(Attendance))]
        public IHttpActionResult DeleteAttendance(int eventId, string userId)
        {
            string currentUser = RequestContext.Principal.Identity.GetUserId();
            var owner = db.Events.Include(e => e.User).Single(e => e.EventId == eventId);
            if ( owner.User.Id == currentUser || currentUser == userId)
            {
                Attendance attendance = db.Attendances.Single(a => a.EventId == eventId && a.AttendeeId == userId);
                if (attendance == null)
                {
                    return NotFound();
                }

                db.Attendances.Remove(attendance);
                db.SaveChanges();

                return Ok(attendance);
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

        private bool AttendanceExists(int id)
        {
            return db.Attendances.Count(e => e.EventId == id) > 0;
        }
    }
}