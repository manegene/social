using Kmums.Models.Amin;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class HomeCreateModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public HomeCreateModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }
        public bool HomeExists { get; set; }

        public async Task<IActionResult> OnGet()
        {
            bool query = await _context.Home.AnyAsync();
            if (query)
                HomeExists = true;
            return Page();
        }

        [BindProperty]
        public HomeModel HomeModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Home.Add(HomeModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
