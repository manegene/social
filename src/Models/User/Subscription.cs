namespace Kmums.Models.User
{
    //class to register, manage and control user subscription
    public class Subscription
    {
        public int Id { get; set; }
        public string SubStaDate { get; set; }
        public string SubEndDate { get; set; }
        public bool IsSubscribed { get; set; }
        public string TransId { get; set; }
        public string JsonTrans { get; set; }
        public UserPublicModel UserPublicProfile { get; set; }
        public UserModel UserProfile { get; set; }
    }
}
