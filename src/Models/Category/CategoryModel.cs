using Microsoft.Build.Framework;

namespace Kmums.Models.Category
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int ParentId { get; set; }

    }
}
