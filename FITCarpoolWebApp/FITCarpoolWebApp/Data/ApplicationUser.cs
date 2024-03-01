using Microsoft.AspNetCore.Identity;

namespace FITCarpoolWebApp.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } = "Rider";
       

    }

}
