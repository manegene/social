using Kmums.Models.Amin;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class PrivacyAddModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public PrivacyAddModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        public bool CheckModel { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var notEmpty= await _context.Privacy.AnyAsync();
            if(notEmpty)
                CheckModel= true;
            return Page();
        }

        [BindProperty]
        public PrivacyUsModel PrivacyModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            PrivacyModel.Lastupdate = DateTime.UtcNow.ToString();
            _context.Privacy.Add(PrivacyModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
