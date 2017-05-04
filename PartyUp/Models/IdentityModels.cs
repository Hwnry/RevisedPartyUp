using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace PartyUp.Models
{
    /*
    This class inherits from IdentityUsers, which creates the basic fields of the user profile.
    Some changes have been made ot ensure that the user can have a collection of friends and basic fields of 
    firstName and lastName. Security has been changed froom cookie authentification to Oauth Token.
    */
    public class ApplicationUser : IdentityUser
    {
        //This is the authentification that will be required before actions can be made
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            //return the user
            return userIdentity;
        }

        //create the firstname field
        public string firstName { get; set; }
        //create the lastname field
        public string lastName { get; set; }
        //create a collection of friends
        public virtual ICollection<ApplicationUser> Friends { get; set; }
    }


    /*
    This class will implement the required methods for the inherited class of IdentityDBContext.
    This class will allow for the database to establish relationships between multiple tables. 
    */
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        /*
        Base function that must be implemented by the inherited class. 
        */
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        /*
        Create() allows for the application to create a database context. 
        */
        public static ApplicationDbContext Create()
        {
            //return the new instance of aaplication database context
            return new ApplicationDbContext();
        }

        //Allow for users to have events
        public DbSet<Event> Events { get; set; }

        //Allow for the user to keep track of all attendances
        public DbSet<Attendance> Attendances { get; set; }

        //Allow for the user to create multiple friends
        public DbSet<Friend> Friends { get; set; }


        /*
         OnModel create takes in a DBModelBuilder type defined by entity framework.
         This function is designed to prevent cascading deletion with a friendship has been deleted.
         Without this function, the deletion function will delete the friend and everything associated
         with the friend.
         */
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //For every friend that has the relationship of me in it, do not delete me and all my associations
            modelBuilder.Entity<Friend>().HasRequired(f => f.Me).WithMany().WillCascadeOnDelete(false);
            //For every friend that has the relationship of newFriend, do not delete that friend and all their associations
            modelBuilder.Entity<Friend>().HasRequired(f => f.newFriend).WithMany().WillCascadeOnDelete(false);
            //Call the model builder
            base.OnModelCreating(modelBuilder);
        }
    }
}