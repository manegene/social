using Kmums.Models.Amin;
using Kmums.Models.Category;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class CategoryViewModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public CategoryViewModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        public IList<CategoryModel> CategoryModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Category != null)
            {
                CategoryModel = await _context.Category.ToListAsync();
            }
        }
    }
}
