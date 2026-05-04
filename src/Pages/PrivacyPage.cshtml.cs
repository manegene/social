using Kmums.Areas.Identity.Data;
using Kmums.Models.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmums.Pages
{
    [AllowAnonymous]
    public class PrivacyPageModel : PageModel
    {
        private readonly ILogger<PrivacyPageModel> _logger;
        private readonly DataContext _dataContext;

        public PrivacyPageModel(ILogger<PrivacyPageModel> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        public PrivacyUsModel PrivacyUsModel { get; set; }
        public void OnGet()
        {
            PrivacyUsModel = _dataContext.Privacy.FirstOrDefault();
        }
    }
}