using Kmums.Models.User;

namespace Kmums.Models.LinkedModels
{
    public class User_Roles
    {
        public int Id { get; set; }
        public UserModel User { get; set; }
        public RolesModel Roles { get; set; }
    }
}
