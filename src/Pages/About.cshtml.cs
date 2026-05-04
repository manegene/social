using Kmums.Models.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages
{
    [AllowAnonymous]
    public class AboutModel : PageModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public AboutModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        public AboutUsModel About { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.About != null)
            {
                About = await _context.About.FirstOrDefaultAsync();
            }
        }
    }
}
