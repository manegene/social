using Kmums.Areas.Identity.Data;
using Kmums.Models.Contact;
using Kmums.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Kmums.Areas.Identity.Pages.Account.Manage
{
    public class ChatModel : PageModel
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly IEmailSender _emailSender;

        public ChatModel(DataContext dataContext,UserManager<UserModel> userManager, IEmailSender emailSender)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [BindProperty]
        public List<ContactModel> SavedChats { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var user= await _userManager.GetUserAsync(User);
            if(user != null)
            {
                SavedChats = await _dataContext.ContactQueue.
                      Where(uid => (uid.Receiver == user.Email) && (!uid.Sender.Contains("kilimanimums.ke"))).ToListAsync();  
            }
            return Page();
        }
        [BindProperty]
        [DisplayName("reply message")]
        [MaxLength(300,ErrorMessage ="maximum length is 300 chacters")]
        public string RespMsg { get; set; }

        [BindProperty]
        public int MsgId { get; set; }

        public ContactModel Reply { get; set; }

        public async Task<ActionResult> OnPostReplyAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (string.IsNullOrEmpty(RespMsg))
            {
                ModelState.AddModelError("RespMsg", "You cannot send an empty message. Please try again with a message in the body");
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            var mthread = await _dataContext.ContactQueue.Where(mid => mid.Id == MsgId).FirstOrDefaultAsync();

            Reply = new()
            {
                Receiver=mthread.Sender,
                Title=mthread.Title,
                Body=RespMsg

            };

            //send message reply
            await _emailSender.SendEmailAsync(Reply.Receiver, Reply.Title, Reply.Body);

            //update the sent email send address
            Reply.Sender=user.Email;
            Reply.Sent = DateTime.Now.ToString();
            await _dataContext.AddAsync(Reply);
            await _dataContext.SaveChangesAsync();

            return RedirectToPage();
            
        }
    }
}
