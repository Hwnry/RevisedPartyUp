using System.Collections.Generic;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PartyUp.Models;

namespace PartyUp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(PartyUp.Models.ApplicationDbContext dbContext)
        {
            // Create a user manager
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext));

            // Add users
            var users = new List<ApplicationUser>
            {
                // Add users here if more are needed
                new ApplicationUser {UserName = "admin@example.com", Email = "admin@example.com"},
                new ApplicationUser {UserName = "friend1@example.com", Email = "friend1@example.com"},
                new ApplicationUser {UserName = "friend2@example.com", Email = "friend2@example.com"},
                new ApplicationUser {UserName = "friend3@example.com", Email = "friend3@example.com"},
                new ApplicationUser {UserName = "friend4@example.com", Email = "friend4@example.com"}

            };

            // Password for each user
            const string userPassword = "Password@123";

            // Use user manager to add list of users.
            foreach (ApplicationUser u in users)
            {
                var usr = manager.FindByName(u.UserName);
                if (usr == null)
                {
                    var result = manager.Create(u, userPassword);
                    result = manager.SetLockoutEnabled(u.Id, false);
                }
            }

            // Add Event
            var events = new List<Event>
            {
                new Event {Name = "Event One", EventDateTime = DateTime.Today, User = manager.FindByEmail("admin@example.com"), Address1 = "1555 Sky Valley Drive", Address2 = "AA202", City = "Reno", Country="United States", PostalCode="89523", Longitude="39.521354", Latitude="-119.851310"},
                new Event {Name = "Event Two", EventDateTime = DateTime.Today, User = manager.FindByEmail("friend1@example.com"), Address1 = "1664 N Virginia St", City = "Reno", Country="United States", PostalCode="89557"},
                new Event {Name = "Event Three", EventDateTime = DateTime.Today, User = manager.FindByEmail("friend2@example.com"), Longitude = "39.523832", Latitude ="-119.780849"}

            };
            events.ForEach(e => dbContext.Events.AddOrUpdate(e));
            dbContext.SaveChanges();

            //Add Attendees
            var attendees = new List<Attendance>
            {
                new Attendance { AttendeeId = manager.FindByEmail("friend1@example.com").Id, EventId = 1},
                new Attendance { AttendeeId = manager.FindByEmail("friend2@example.com").Id, EventId = 1},
                new Attendance { AttendeeId = manager.FindByEmail("friend3@example.com").Id, EventId = 1},
                new Attendance { AttendeeId = manager.FindByEmail("friend3@example.com").Id, EventId = 2},
                new Attendance { AttendeeId = manager.FindByEmail("friend4@example.com").Id, EventId = 1},
            };
            attendees.ForEach(a => dbContext.Attendances.Add(a));
            dbContext.SaveChanges();

            //Add Friends
            var friends = new List<Friend>
            {
                new Friend { friendshipEstablished = true, myID = manager.FindByEmail("admin@example.com").Id ,
                newFriendId = manager.FindByEmail("friend1@example.com").Id},
                new Friend { friendshipEstablished = true, myID = manager.FindByEmail("admin@example.com").Id ,
                newFriendId = manager.FindByEmail("friend2@example.com").Id},
                new Friend { friendshipEstablished = true, myID = manager.FindByEmail("admin@example.com").Id ,
                newFriendId = manager.FindByEmail("friend3@example.com").Id},
            };

            friends.ForEach(f => dbContext.Friends.Add(f));
            dbContext.SaveChanges();
        }
    }
}
