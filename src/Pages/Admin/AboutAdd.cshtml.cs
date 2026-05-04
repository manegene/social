using Kmums.Models.Amin;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class AboutAddModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public AboutAddModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }
        public bool CheCk { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var exist = await _context.About.AnyAsync();
            if(exist)
                CheCk= true;
            return Page();
        }

        [BindProperty]
        public AboutUsModel AboutModel { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            AboutModel.Lastupdated = DateTime.Now.ToString();
            _context.About.Add(AboutModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("../Index");
        }
    }
}
