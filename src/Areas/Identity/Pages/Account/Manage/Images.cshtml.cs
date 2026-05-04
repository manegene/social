using Kmums.Areas.Identity.Data;
using Kmums.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Areas.Identity.Pages.Account.Manage
{
    public class ImagesModel : PageModel
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<UserModel> _userManager;
        public ImagesModel(DataContext dataContext, UserManager<UserModel> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        [BindProperty]
        public ImageUploadModel Images { get; set; }

        public List<UserImageModel> SavedImages { get; set; }

        public bool UserhasProfile { get; set; }

        public async Task<ActionResult> OnGet()
        {
            UserModel user = await _userManager.GetUserAsync(User);

            SavedImages = await _dataContext.Images.Where(us => us.User == user).ToListAsync();
            UserhasProfile = _dataContext.PublicProfile.Any(us => us.User == user);

            return Page();
        }

        //only file type allowed fr images upload
        private string[] permittedExtensions = { ".png" };

        public async Task<ActionResult> OnPostImagesendAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //get the uploaded file name to validate the extension
            string ext = Path.GetExtension(Images.FormFile.FileName).ToLowerInvariant();

            //if file extension is not in the list,stop further file processing
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return BadRequest("file format not allowed");
            }

            //get the logged on user
            UserModel user = await _userManager.GetUserAsync(User);
            UserPublicModel userprofile = await _dataContext.PublicProfile.Where(us => us.User == user).FirstOrDefaultAsync();
            //process image now that its a verified file format
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await Images.FormFile.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    UserImageModel file = new UserImageModel()
                    {
                        Image = memoryStream.ToArray(),
                        User = user,
                        UserProfile = userprofile

                    };
                    _dataContext.Images.Add(file);

                    await _dataContext.SaveChangesAsync();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }
            }

            //after saving the file. redirect to the images page
            return RedirectToPage("Images");
        }
    }
}
