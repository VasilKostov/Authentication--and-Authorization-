using Microsoft.AspNetCore.Identity;

namespace Authentication_and_Authorization.Models.Domain
{
    public class ApplicationUser:IdentityUser
    {
        public string Name { get; set; }
        public string ? ProfilePicture { get; set; }
    }
}
