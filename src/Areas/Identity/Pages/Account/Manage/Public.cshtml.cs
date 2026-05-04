using Kmums.Models.Category;
using Kmums.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Areas.Identity.Pages.Account.Manage
{
    public class PublicModel : PageModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;
        private readonly UserManager<UserModel> _userManager;

        public PublicModel(Kmums.Areas.Identity.Data.DataContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<CategoryModel> Categories { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            UserModel Loggedin = await _userManager.GetUserAsync(User);

            UserPublicModel = _context.PublicProfile.Where(user => user.User == Loggedin).FirstOrDefault();
            Categories = _context.Category.Where(pcat => pcat.ParentId > 0).ToList();

            return Page();
        }

        [BindProperty]
        public UserPublicModel UserPublicModel { get; set; }



        //  public UserCategoryMappingModel UserCategory { get; set; }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            UserModel user = await _userManager.GetUserAsync(User);
            UserPublicModel exists = _context.PublicProfile.Where(us => us.User == user).AsNoTracking().FirstOrDefault();
            //This is an update. Update existing record
            if (exists != null)
            {
                UserPublicModel.LastUpdate = DateTime.UtcNow.ToString();
                UserPublicModel.Created = exists.Created;
                _context.Entry(UserPublicModel).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            //this is a new record. make a new insert
            else
            {
                UserPublicModel.LastUpdate = DateTime.UtcNow.ToString();
                UserPublicModel.Created = DateTime.UtcNow.ToString();
                UserPublicModel.User = user;

                _context.PublicProfile.Add(UserPublicModel);

                await _context.SaveChangesAsync();

            }
            return RedirectToPage("./Index");

        }
    }
}
