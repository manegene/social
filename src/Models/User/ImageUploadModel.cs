using System.ComponentModel.DataAnnotations;

namespace Kmums.Models.User
{
    public class ImageUploadModel
    {
        [Required]
        [Display(Name = "image file")]
        public IFormFile FormFile { get; set; }
    }
}
