using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace Kmums.Models.User
{
    public class UserModel : IdentityUser<Guid>
    {
        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("Gender expression")]
        public string Gender { get; set; }
    }
}
