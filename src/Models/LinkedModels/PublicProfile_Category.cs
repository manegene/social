using Kmums.Models.Category;
using Kmums.Models.User;

namespace Kmums.Models.LinkedModels
{
    public class PublicProfile_Category
    {
        public virtual UserPublicModel Profile { get; set; }
        public virtual CategoryModel Category { get; set; }
    }
}
