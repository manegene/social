using AspNetCore.ReCaptcha;
using Kmums.Areas.Identity.Data;
using Kmums.Models.Category;
using Kmums.Models.Contact;
using Kmums.Models.LinkedModels;
using Kmums.Models.Store;
using Kmums.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Kmums.Pages
{
    [ValidateReCaptcha]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<UserModel> _userManager;


        public IndexModel(ILogger<IndexModel> logger, DataContext dataContext, IEmailSender emailSender, UserManager<UserModel> userManager)
        {
            _logger = logger;
            _dataContext = dataContext;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public HomeModel Home { get; set; }
        public List<UserPublicModel> Users { get; set; }
        public List<CategoryModel> Categories { get; set; }
        public List<PublicProfile_Category> PublicProfiles { get; set; }

        [BindProperty]
        public ContactModel EmailDetails { get; set; }

        [BindProperty]
        public int ProfId { get; set; }

        public List<UserImageModel> UImage { get; set; }

        [BindProperty]
        public List<UserImageModel> SelectedUserImages { get; set; }

        public UserModel LoggedIn { get; set; }
        public bool IsSubscribed { get; set; }

        [TempData]
        public string ResponseMessage { get; set; }
        public async Task<ActionResult> OnGet()
        {
            try
            {

                LoggedIn = await _userManager.GetUserAsync(User);
                if (LoggedIn != null)
                {
                    IsSubscribed = await _dataContext.Subscriptions.AnyAsync(usr => usr.UserProfile == LoggedIn);
                }
                ;
                //public user profiles
                PublicProfiles = [.. from UserPublicModel upm in _dataContext.PublicProfile
                         join CategoryModel cm in _dataContext.Category
                         on upm.CategoryId equals cm.Id
                         select new PublicProfile_Category
                         {
                             Profile=upm,
                             Category=cm

                         }];


                //get site home information
                Home = await _dataContext.Home.FirstOrDefaultAsync();

                //images for all users
                UImage = await _dataContext.Images.ToListAsync();


                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the home page.");
                ResponseMessage = "An error occurred while loading the page. Please try again later." + ex.Message;
                return Page();
            }
        }

        //get user specific photos
        public async Task<ActionResult> OnGetSelectedUserAsync(int profileId)
        {
            if (profileId <= 0)
            {
                throw new InvalidOperationException("operation not allowed");
            }
            SelectedUserImages = await _dataContext.Images.Where(id => id.UserProfile.Id == profileId).ToListAsync();

            return new JsonResult(SelectedUserImages.ToJson());
        }

        public async Task<IActionResult> OnPostSendEmail()

        {

            //get user by using the public profile id 
            UserModel destinationuser = await _dataContext.PublicProfile.Where(profid => profid.Id == Convert.ToInt32(EmailDetails.Receiver)).Select(usr => usr.User).FirstOrDefaultAsync();

            //now retrieve the receiver email address
            EmailDetails.Receiver = await _dataContext.Users.Where(uid => uid == destinationuser).Select(uemail => uemail.Email).FirstOrDefaultAsync();

            await _emailSender.SendEmailAsync(EmailDetails.Receiver, EmailDetails.Title, EmailDetails.Body);

            //send acknowledgement message to sender
            string senderReponse = "Your message was sent successfully";
            await _emailSender.SendEmailAsync(EmailDetails.Sender, EmailDetails.Title, senderReponse);

            //lastly
            //update sender email address for the just sent email
            ContactModel CurrMsg = await _dataContext.ContactQueue.
                Where(em => (em.Receiver == EmailDetails.Receiver) && (em.Title == EmailDetails.Title) && (em.Body == EmailDetails.Body)).
                FirstOrDefaultAsync();
            if (CurrMsg != null)
            {
                CurrMsg.Sender = EmailDetails.Sender;
                await _dataContext.SaveChangesAsync();
            }

            ResponseMessage = "email message sent successfully";
            return RedirectToPage();
        }

        //update user subscription
        public async Task<ActionResult> OnPostUpdateSubscriptionAsync(string TransactionID, string JsonData)
        {

            UserModel user = await _userManager.GetUserAsync(User);

            UserPublicModel publicUser = await _dataContext.PublicProfile.Where(puid => puid.User == user).FirstOrDefaultAsync();

            Subscription subscription = new()
            {
                SubStaDate = DateTime.UtcNow.ToString(),
                SubEndDate = DateTime.UtcNow.AddYears(1).ToString(),
                IsSubscribed = true,
                TransId = TransactionID,
                JsonTrans = JsonData,
                UserProfile = user,
                UserPublicProfile = publicUser
            };

            await _dataContext.AddAsync(subscription);
            Task<int> posted = _dataContext.SaveChangesAsync();

            if (posted.Result <= 0)
                return new JsonResult("user error: Error saving the values");

            return new JsonResult("subscriptin update successful");
        }
    }
}