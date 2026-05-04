using Kmums.Areas.Identity.Data;
using Kmums.Models.Contact;
using Kmums.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kmums.Areas.Identity.Pages.Account.Manage
{
    public class SentChatModel : PageModel
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly DataContext _context;

        public SentChatModel(UserManager<UserModel> userManager,DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<ContactModel> Messages { get; set; }
        public async Task<ActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest();
            Messages = await _context.ContactQueue.Where(msend => msend.Sender == user.Email).ToListAsync();
            return Page();
        }
    }
}
