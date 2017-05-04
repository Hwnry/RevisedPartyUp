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


/***
 * This controller specifically handles all of the friend related requests.
 * 
 * GetFriends.
 * This controller gets all of the friends or pending friends associated with
 * the user who sent the request.
 * 
 * PostFriend.
 * This controller is used to intiated a request as well as confirm a request.
 * If the user sends a request and a friendships has not already been established,
 * friendship request will be extended to the target user.
 * If the friendship already exists and this is the targetted user, friendship will 
 * be confirmed.
 * Only data about the relationship will be returned if the requesting party attempts
 * to confirm the friendship, frienship will not be confirmed until receiving party
 * accepts.
 * 
 * DeleteFriend.
 * This controller deletes a friend specifically detailed by their unique ID.
 * This will remove only the frienship details, cascade deletion will not
 * delete profiles, events, etc.
 * 
 * GetFriendsAll.
 * This controller is used to get all the potential future friends, i.e. every
 * user in the database that is not already friends with the requesting user.
 * 
 * Dispose.
 * Garbage collection of this controller 
 * 
 * FriendsExist.
 * Simple function to determine if a friendship exists in the database
 */
namespace PartyUp.Controllers
{
    [Authorize]
    public class FriendsController : ApiController
    {
        //creates the context for accessing the database
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Friends
        public IEnumerable<FriendBindingModel> GetFriends()
        {
            //determines the identity of the requesting user
            var currentUser = RequestContext.Principal.Identity.GetUserId();

            // Get the data from the database
            //gets all of the friends
            var query = db.Friends.Include(f => f.newFriend).ToList();
            //include friends I am/attempting to be friends with
            var query1 = query.Where(f => f.myID == currentUser);
            //include friends who are/attempt to add me as a friend
            var query2 = query.Where(f => f.newFriendId == currentUser);

            //create a binding model for each friend to be returned
            List<FriendBindingModel> friendsList = new List<FriendBindingModel>();

            // if query1 is not empty/ friends are found 
            if (query1.Any())
            {
                //for every person found 
                foreach (var f in query1)
                {
                    //add that persons details to a friend binding model
                    friendsList.Add(new FriendBindingModel
                    {
                        //add basic friend details 
                        newFriendId = f.newFriendId,
                        friendshipEstablished = f.friendshipEstablished,
                        userName = f.newFriend.Email,
                        FirstName = f.newFriend.firstName,
                        LastName = f.newFriend.lastName,
                        PhoneNumber = f.newFriend.PhoneNumber
                        

                    });
                }
            }

            //if query2 is not empty/ friends are found
            if(query2.Any())
            {
                //for every friend found
                foreach (var f in query2)
                {
                    //place their data into a friendbindingmodel
                    friendsList.Add(new FriendBindingModel
                    {
                        //place the specific data into a binding model
                        newFriendId = f.myID,
                        friendshipEstablished = f.friendshipEstablished,
                        userName = f.Me.Email,
                        FirstName = f.Me.firstName,
                        LastName = f.Me.lastName,
                        PhoneNumber = f.Me.PhoneNumber
                        
                    });
                }
            }

            //return the list of friends that have been associated with you
            return friendsList;
        }

       
        // POST: api/Friends
        [ResponseType(typeof(Friend))]
        public IHttpActionResult PostFriend(Friend friend)
        {
            //check to see if the model state of the request is in the expected form
            if (!ModelState.IsValid)
            {
                //if not in expected form, reply with bad request
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
                Friend confirmation = db.Friends.Single(f => (f.newFriendId == friend.newFriendId && f.myID == currentUser)||
                (f.newFriendId == currentUser && f.myID == friend.newFriendId));
                //set friendship status to true or confirmed
                confirmation.friendshipEstablished = true; 
                //update the database details
                db.Friends.AddOrUpdate(confirmation);

                try
                {
                    //attempt to save changes to the database
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    //if database throws an exception
                    //check to see if requesting party attempting to confirm 
                    if (FriendExists(friend.myID))
                    {
                        //return that there is a conflict with the users request
                        return Conflict();
                    }
                    else
                    {
                        //unknown error thrown back
                        throw;
                    }
                }
                //return ok if request was completed successfully
                return Ok();
            }

            else
            {
                //confirm the current user's identity
                currentUser = RequestContext.Principal.Identity.GetUserId();
                //establish the requesting parties identity
                friend.myID = currentUser;
                //set the friendship to false or unconfirmed
                friend.friendshipEstablished = false;
                //update the database
                db.Friends.AddOrUpdate(friend);

                try
                {
                    //save changes to the database
                    db.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    //if friendship has a conflict
                    if (FriendExists(friend.myID))
                    {
                        //return the conflict
                        return Conflict();
                    }
                    else
                    {
                        //throw to end the request
                        throw;
                    }
                }
                //return details confirming that request was sent
                return CreatedAtRoute("DefaultApi", new { id = friend.myID }, friend);
            }

        }

        // DELETE: api/Friends/5
        [ResponseType(typeof(Friend))]
        [Route("api/Friends/{id}")]
        public IHttpActionResult DeleteFriend(string id)
        {
            //confirm the id of the requesting user
            var currentUser = RequestContext.Principal.Identity.GetUserId();
            //find the specific friendship containing the two user's ids
            Friend friend = db.Friends.Single(f => (f.myID == currentUser && f.newFriendId == id) ||
            (f.myID == id && f.newFriendId == currentUser));
            
            //if the relationship cannot be found
            if (friend == null)
            {
                //return error message not found
                return NotFound();
            }
            //otherwise remove the friend
            db.Friends.Remove(friend);
            //save changes to the database
            db.SaveChanges();
            //return status okay and details about the removed friend
            return Ok(friend);
        }

        // GET: api/Friends/All
        [ResponseType(typeof(FriendBindingModel))]
        [Route("api/Friends/All")]
        public IEnumerable<FriendBindingModel> GetFriendsAll()
        {
            //confirm the identity of the current user
            var currentUser = RequestContext.Principal.Identity.GetUserId();

            // Get the data from the database, except current user's data
            var query = db.Users.Where(u => u.Id != currentUser);
 

            //create a list of all the friends
            List<FriendBindingModel> friendsList = new List<FriendBindingModel>();

            // if query is not empty
            if (query.Any())
            {
                //for every friend found
                foreach (var f in query)
                {
                    //return the information about that user
                    friendsList.Add(new FriendBindingModel()
                    {
                        //details about the friend
                       newFriendId = f.Id,
                       userName = f.Email,
                       FirstName = f.firstName,
                       LastName = f.lastName
                    });
                }
            }

            //return the list of friends with all their details
            return friendsList;
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
            //returns if any friends with this id were found
            return db.Friends.Count(e => e.myID == id) > 0;
        }
    }
}