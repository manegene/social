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
    public class IndexModel(ILogger<IndexModel> logger, DataContext dataContext, IEmailSender emailSender, UserManager<UserModel> userManager) : PageModel
    {
        private readonly ILogger<IndexModel> _logger = logger;
        private readonly DataContext _dataContext = dataContext;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly UserManager<UserModel> _userManager = userManager;

        private const int MAX_BODY_LENGTH = 5000;
        private const int MAX_JSON_LENGTH = 10000;

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

            UserModel user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            bool ownsProfile = await _dataContext.PublicProfile
                .AnyAsync(p => p.Id == profileId && p.User == user);

            if (!ownsProfile)
                return Forbid();

            SelectedUserImages = await _dataContext.Images.Where(id => id.UserProfile.Id == profileId).ToListAsync();


            return new JsonResult(SelectedUserImages.ToJson());
        }

        public async Task<IActionResult> OnPostSendEmail()

        {
            if (EmailDetails == null)
                return BadRequest();

            if (!int.TryParse(EmailDetails.Receiver, out int receiverId))
                return BadRequest("Invalid receiver");

            if (string.IsNullOrWhiteSpace(EmailDetails.Body) ||
                EmailDetails.Body.Length > MAX_BODY_LENGTH)
                return BadRequest("Invalid message body");

            UserModel senderUser = await _userManager.GetUserAsync(User);
            if (senderUser == null)
                return Unauthorized();

            string senderEmail = senderUser.Email;

            UserModel destinationUser = await _dataContext.PublicProfile
                .Where(p => p.Id == receiverId)
                .Select(p => p.User)
                .FirstOrDefaultAsync();

            if (destinationUser == null)
                return NotFound("Receiver not found");

            string receiverEmail = await _dataContext.Users
                .Where(u => u == destinationUser)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            await _emailSender.SendEmailAsync(receiverEmail, EmailDetails.Title, EmailDetails.Body);

            await _emailSender.SendEmailAsync(senderEmail, EmailDetails.Title, "Your message was sent successfully");

            ResponseMessage = "email message sent successfully";
            return RedirectToPage();
        }

        public async Task<ActionResult> OnPostUpdateSubscriptionAsync(string TransactionID, string JsonData)
        {
            if (string.IsNullOrWhiteSpace(TransactionID))
                return new JsonResult("error: operation not allowed");

            if (!string.IsNullOrEmpty(JsonData) && JsonData.Length > MAX_JSON_LENGTH)
                return new JsonResult("error: payload too large");

            UserModel user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            UserPublicModel publicUser = await _dataContext.PublicProfile
                .FirstOrDefaultAsync(p => p.User == user);

            if (publicUser == null)
                return NotFound("unknown user profile");

            bool exists = await _dataContext.Subscriptions
                .AnyAsync(s => s.TransId == TransactionID);

            if (exists)
                return new JsonResult("duplicate transaction");

            Subscription subscription = new Subscription
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

            int result = await _dataContext.SaveChangesAsync();

            if (result <= 0)
                return new JsonResult("user error: Error saving the values");

            return new JsonResult("subscription update successful");
        }
    }
}