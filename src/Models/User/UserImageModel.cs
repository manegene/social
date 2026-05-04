namespace Kmums.Models.User
{
    public class UserImageModel
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public UserModel User { get; set; }
        public virtual UserPublicModel UserProfile { get; set; }
    }
}
