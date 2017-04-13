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
using PartyUp.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Migrations;

namespace PartyUp.Controllers
{
    [Authorize]
    public class FriendsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Friends
        public IEnumerable<FriendBindingModel> GetFriends()
        {
            var currentUser = RequestContext.Principal.Identity.GetUserId();

            // Get the data from the database
            var query = db.Friends.Include(f => f.newFriend).ToList();
            var query1 = query.Where(f => f.myID == currentUser);
            var query2 = query.Where(f => f.newFriendId == currentUser);


            List<FriendBindingModel> friendsList = new List<FriendBindingModel>();

            // if query1 is not empty
            if (query1.Any())
            {
                foreach (var f in query1)
                {
                    friendsList.Add(new FriendBindingModel
                    {
                        newFriendId = f.newFriendId,
                        friendshipEstablished = f.friendshipEstablished,
                        userName = f.newFriend.Email

                    });
                }
            }
            else
            {
                foreach (var f in query2)
                {
                    friendsList.Add(new FriendBindingModel
                    {
                        newFriendId = f.myID,
                        friendshipEstablished = f.friendshipEstablished,
                        userName = f.Me.Email

                    });
                }
            }

            return friendsList;
        }

        //// GET: api/Friends/5
        //[ResponseType(typeof(Friend))]
        //public IHttpActionResult GetFriend(string id)
        //{
        //    Friend friend = db.Friends.Find(id);
        //    if (friend == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(friend);
        //}

        //// PUT: api/Friends/5
        //[ResponseType(typeof(void))]
        //public IHttpActionResult PutFriend(string id, Friend friend)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != friend.myID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(friend).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!FriendExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Friends
        [ResponseType(typeof(Friend))]
        public IHttpActionResult PostFriend(Friend friend)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            //get user id
            var currentUser = RequestContext.Principal.Identity.GetUserId();
            
            //check to see if friendship is in database
            var data = db.Friends.Any(f =>
                (f.newFriendId == currentUser && f.myID == friend.newFriendId));

            //if friendship is in database
            if (data)
            {

                //friendship is being confirmed
                //set the boolean to true

                Friend confirmation = db.Friends.Single(f => (f.newFriendId == friend.newFriendId && f.myID == currentUser)||
                (f.newFriendId == currentUser && f.myID == friend.newFriendId));

                confirmation.friendshipEstablished = true; 
                db.Friends.AddOrUpdate(confirmation);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (FriendExists(friend.myID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok();
            }

            else
            {
                currentUser = RequestContext.Principal.Identity.GetUserId();
                friend.myID = currentUser;
                friend.friendshipEstablished = false;
                db.Friends.AddOrUpdate(friend);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    if (FriendExists(friend.myID))
                    {
                        return Conflict();
                    }
                    else
                    {
                        throw;
                    }
                }

                return CreatedAtRoute("DefaultApi", new { id = friend.myID }, friend);
            }

        }

        // DELETE: api/Friends/5
        [ResponseType(typeof(Friend))]
        [Route("api/Friends/{id}")]
        public IHttpActionResult DeleteFriend(string id)
        {
            var currentUser = RequestContext.Principal.Identity.GetUserId();
            Friend friend = db.Friends.Single(f => (f.myID == currentUser && f.newFriendId == id) ||
            (f.myID == id && f.newFriendId == currentUser));

            if (friend == null)
            {
                return NotFound();
            }

            db.Friends.Remove(friend);
            db.SaveChanges();

            return Ok(friend);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FriendExists(string id)
        {
            return db.Friends.Count(e => e.myID == id) > 0;
        }
    }
}