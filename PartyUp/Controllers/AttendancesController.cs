﻿using System;
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

/**
 * This controller deals specifically with inviting/uninviting friends as well as
 * listing who is going.
 * 
 * GetAttendances.
 * Gets all of the the events the current user has been invited to. 
 * 
 * GetAttendances w/id.
 * Returns all of the details associated with the specific event
 * 
 * PutAttendance.
 * Allows users to update details about attendance.
 * 
 * PostAttendance.
 * This controller is called when the user wants to invite a friend to an event. 
 * 
 * DeleteAttendance.
 * This is the controller that is called to uninvite friends from events.
 * 
 * Dispose.
 * Garbage collection for this controller.
 * 
 * AttendanceExists.
 * Function used to determine if the attendance relationship exists.
 */

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

            //get current user
            string currentUser = RequestContext.Principal.Identity.GetUserId();

            // Loop through each event and add to the binding model. Make sure to include the user table and event table in query.
            foreach (var at in db.Attendances.Include(a => a.Event).Include(a => a.Attendee).Include(a => a.Event.User))
            {
                //check to see if the current user is invited
                if (at.AttendeeId == currentUser)
                {
                    //return the details the attending user needs about the event
                    attendeeList.Add(new AttendanceBindingModel
                    {
                        //populate event details
                        EventName = at.Event.Name,
                        UserName = at.Attendee.UserName,
                        EventId = at.EventId,
                        UserId = at.AttendeeId,
                        FirstName = at.Attendee.firstName,
                        LastName = at.Attendee.lastName,
                        EventOwner = at.Event.User.Email,
                        EventDetails = at.Event.Details
                    });
                }
            }
            //return the list of attending details
            return attendeeList;
        }

        // GET: api/Attendances/5
        [ResponseType(typeof(List<AttendanceBindingModel>))]
        [Route("api/Attendances/{eventId}")]
        public List<AttendanceBindingModel> GetEventAttendees(int eventId)
        {
            //access the details about the event
            var aList = db.Events.Include(e => e.Attendeees).Include(e => e.User).Single(e => e.EventId == eventId);
            //create a list for who is going to the event
            List<AttendanceBindingModel> temp = new List<AttendanceBindingModel>();
            //for each user found
            foreach (var a in aList.Attendeees)
            {
                //populate the attendance details
                temp.Add(new AttendanceBindingModel
                {
                    UserName = db.Users.Single(u => u.Id == a.AttendeeId).Email,
                    FirstName = db.Users.Single(u => u.Id == a.AttendeeId).firstName,
                    LastName = db.Users.Single(u => u.Id == a.AttendeeId).lastName,
                    UserId = db.Users.Single(u => u.Id == a.AttendeeId).Id
                });
            }
            //return the created list.
            return temp;
        }

        // PUT: api/Attendances/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAttendance(int id, Attendance attendance)
        {
            //check to see if the model state is bad
            if (!ModelState.IsValid)
            {
                //return bad request
                return BadRequest(ModelState);
            }

            //if the id doesn't match the event
            if (id != attendance.EventId)
            {
                //return bad request
                return BadRequest();
            }
            //update database
            db.Entry(attendance).State = EntityState.Modified;

            try
            {
                //attempt to save changes to the database
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                //if the id does not exist
                if (!AttendanceExists(id))
                {
                    //return not found 
                    return NotFound();
                }
                else
                {
                    //throw an error for unknown problem
                    throw;
                }
            }

            //return the status code okay if it worked
            return StatusCode(HttpStatusCode.NoContent);
        }

        // Modified //////////////////////////////////////////////////
        // POST: api/Attendances
        [ResponseType(typeof(Attendance))]
        public IHttpActionResult PostAttendance(Attendance attendance)
        {
            //if the model state is not valid
            if (!ModelState.IsValid)
            {
                //return bad request
                return BadRequest(ModelState);
            }

            //add the attendance details to the database
            db.Attendances.Add(attendance);

            try
            {
                //attempt to save changes
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                //if the data does not meet the search criteria
                if (AttendanceExists(attendance.EventId))
                {
                    //return conflict message
                    return Conflict();
                }
                else
                {
                    //throw for unknown errors 
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
            //get the current user
            string currentUser = RequestContext.Principal.Identity.GetUserId();
            //get event owner information
            var owner = db.Events.Include(e => e.User).Single(e => e.EventId == eventId);
            //if the current user is the owner
            if ( owner.User.Id == currentUser || currentUser == userId)
            {
                //remove the attendance specifically 
                Attendance attendance = db.Attendances.Single(a => a.EventId == eventId && a.AttendeeId == userId);
                //if the attendance is null
                if (attendance == null)
                {
                    //return not found
                    return NotFound();
                }
                //remove the attendance from the database
                db.Attendances.Remove(attendance);
                //save changes to the database
                db.SaveChanges();
                //return ok status with attendance details
                return Ok(attendance);
            }
            //return not found 
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