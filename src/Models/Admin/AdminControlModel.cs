using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kmums.Models.Amin
{
    [Authorize(Roles = "admin")]
    public class AdminControlModel : PageModel
    {

    }
}
