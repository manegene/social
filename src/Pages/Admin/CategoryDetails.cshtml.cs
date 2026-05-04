using Kmums.Models.Category;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class DetailsModel : PageModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public DetailsModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

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
    }
}
