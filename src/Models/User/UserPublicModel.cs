using System.ComponentModel;

namespace Kmums.Models.User
{
    public class UserPublicModel
    {
        public int Id { get; set; }
        public UserModel User { get; set; }

        [DisplayName("Service category")]
        public int CategoryId { get; set; }

        [DisplayName("Public title")]
        public string Heading { get; set; }

        [DisplayName("Your public information")]
        public string About { get; set; }
        public string Created { get; set; }
        public string LastUpdate { get; set; }

        public virtual List<UserImageModel> Images { get; set; }
    }
}
