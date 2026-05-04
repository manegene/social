using Kmums.Models.Amin;
using Kmums.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class DeleteModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public DeleteModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CategoryModel CategoryModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            CategoryModel categorymodel = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

            if (categorymodel == null)
            {
                return NotFound();
            }
            else
            {
                CategoryModel = categorymodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }
            CategoryModel categorymodel = await _context.Category.FindAsync(id);

            if (categorymodel != null)
            {
                CategoryModel = categorymodel;
                _context.Category.Remove(CategoryModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./CategoryView");
        }
    }
}
