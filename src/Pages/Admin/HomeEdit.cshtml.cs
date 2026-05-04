using Kmums.Models.Amin;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class HomeEditModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public HomeEditModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public HomeModel HomeModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Home == null)
            {
                //return NotFound();
                id = 1;
            }

            HomeModel homemodel = await _context.Home.FirstOrDefaultAsync(m => m.Id == id);
            if (homemodel == null)
            {
                return NotFound();
            }
            HomeModel = homemodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(HomeModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HomeModelExists(HomeModel.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("../Index");
        }

        private bool HomeModelExists(int id)
        {
            return _context.Home.Any(e => e.Id == id);
        }
    }
}
