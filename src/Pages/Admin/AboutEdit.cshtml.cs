using Kmums.Models.Amin;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class AboutEditModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public AboutEditModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AboutUsModel AboutUsModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            id = _context.About.FirstOrDefault().Id;

            AboutUsModel aboutusmodel = await _context.About.FirstOrDefaultAsync(m => m.Id == id);
            if (aboutusmodel == null)
            {
                return NotFound();
            }
            AboutUsModel = aboutusmodel;
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
            AboutUsModel.Lastupdated = DateTime.Now.ToString();
            _context.Attach(AboutUsModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AboutUsModelExists(AboutUsModel.Id))
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

        private bool AboutUsModelExists(int id)
        {
            return _context.About.Any(e => e.Id == id);
        }
    }
}
