using Kmums.Models.Amin;
using Kmums.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Pages.Admin
{
    public class ListRolesModel : AdminControlModel
    {
        private readonly Kmums.Areas.Identity.Data.DataContext _context;

        public ListRolesModel(Kmums.Areas.Identity.Data.DataContext context)
        {
            _context = context;
        }

        public IList<RolesModel> RolesModel { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Roles != null)
            {
                RolesModel = await _context.Roles.ToListAsync();
            }
        }
    }
}
