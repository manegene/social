using Kmums.Models.Amin;
using Kmums.Models.Category;
using Microsoft.AspNetCore.Mvc;

namespace Kmums.Pages.Admin
{
    public class CreateModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public CreateModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ParentCategory = _context.Category.ToList();
            return Page();
        }

        [BindProperty]
        public CategoryModel CategoryModel { get; set; }
        
        [BindProperty]
        public CategoryDTO Input { get; set; }

        public List<CategoryModel> ParentCategory { get; set; }

       
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var entity = new CategoryModel
            {
                Name = Input.Name,
                ParentId = Input.ParentCategoryId
            };

            _context.Category.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToPage("./CategoryView");
        }
    }
}
