using Microsoft.AspNetCore.Identity;

namespace RapidPayAPI.Models
{
    public class User:IdentityUser
    {
        
        public string FirstName { get; set; } = default!;
         public string LastName { get; set; } = default!;
    }
}